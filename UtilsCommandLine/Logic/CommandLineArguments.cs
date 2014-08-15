namespace CarbonCore.UtilsCommandLine.Logic
{
    using System.Collections.Generic;
    using System.Text;

    using CarbonCore.GrammarParser.Contracts.Grammars;
    using CarbonCore.GrammarParser.Grammars;
    using CarbonCore.GrammarParser.Tokenize;
    using CarbonCore.Utils;
    using CarbonCore.UtilsCommandLine.Contracts;

    public class CommandLineArguments : ICommandLineArguments
    {
        private readonly ICommandLineGrammar grammar;

        private readonly IList<ICommandLineSwitchDefinition> definitions;
        private readonly IDictionary<string, ICommandLineSwitch> activeSwitchDictionary;
        private readonly List<ICommandLineSwitch> activeSwitches;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public CommandLineArguments(ICommandLineGrammar grammar)
        {
            this.grammar = grammar;
            this.definitions = new List<ICommandLineSwitchDefinition>();
            this.activeSwitchDictionary = new Dictionary<string, ICommandLineSwitch>();
            this.activeSwitches = new List<ICommandLineSwitch>();
        }

        private enum TranslationMode
        {
            None,
            GotSwitch,
            ExpectSwitch,
            ExpectArgument
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IReadOnlyCollection<ICommandLineSwitch> ActiveSwitches
        {
            get
            {
                return this.activeSwitches.AsReadOnly();
            }
        }

        public bool ParseArguments(string arguments)
        {
            // Clear out the old results
            this.activeSwitchDictionary.Clear();
            this.activeSwitches.Clear();

            if (string.IsNullOrWhiteSpace(arguments))
            {
                return true;
            }

            var tokenizer = new Tokenizer();
            IList<Token> tokens = tokenizer.Tokenize(this.grammar, arguments);
            if (tokens.Count == 0)
            {
                return true;
            }

            if (!this.TranslateTokens(tokens))
            {
                return false;
            }

            return this.EvaluateDefines();
        }

        public bool ParseCommandLineArguments()
        {
            // Clear out the old results
            this.activeSwitchDictionary.Clear();
            this.activeSwitches.Clear();
            
            var builder = new StringBuilder();
            string[] args = System.Environment.GetCommandLineArgs();
            if (args.Length <= 0)
            {
                return true;
            }

            for (int i = 1; i < args.Length; i++)
            {
                builder.Append(args[i] + " ");
            }

            return this.ParseArguments(builder.ToString().TrimEnd());
        }

        public bool HasSwitch(string key)
        {
            return this.activeSwitchDictionary.ContainsKey(key);
        }

        public ICommandLineSwitch GetSwitch(string key)
        {
            if (this.activeSwitchDictionary.ContainsKey(key))
            {
                return this.activeSwitchDictionary[key];
            }

            return null;
        }

        public ICommandLineSwitchDefinition Define(string shortString, CommandLineSwitchSetDelegate action)
        {
            var definition = new CommandLineSwitchDefinition(shortString, null, action);
            this.definitions.Add(definition);
            return definition;
        }

        public ICommandLineSwitchDefinition Define(string shortString, string longString, CommandLineSwitchSetDelegate action)
        {
            var definition = new CommandLineSwitchDefinition(shortString, longString, action);
            this.definitions.Add(definition);
            return definition;
        }

        public void ClearDefines()
        {
            this.definitions.Clear();
        }

        public void PrintArgumentUse()
        {
            System.Diagnostics.Trace.TraceInformation("Argument use: ");
            foreach (ICommandLineSwitchDefinition definition in this.definitions)
            {
                System.Diagnostics.Trace.TraceInformation(" {0} / {1} - {2} {3}", definition.Short, definition.Long, definition.Description, definition.RequireArgument ? "(required)" : string.Empty);
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private bool TranslateTokens(IList<Token> tokens)
        {
            int currentToken = 0;
            var mode = TranslationMode.None;
            ICommandLineSwitch currentSwitch = new CommandLineSwitch();
            while (currentToken < tokens.Count)
            {
                Token token = tokens[currentToken++];
                if (token.Term == null || token.Term.Tag == null)
                {
                    System.Diagnostics.Trace.TraceError("Argument parsing failed for token {0}", token);
                    continue;
                }

                var key = (CommandLineTermKey)token.Term.Tag;
                switch (key)
                {
                    case CommandLineTermKey.Slash:
                    case CommandLineTermKey.Dash:
                    case CommandLineTermKey.Dash2:
                        {
                            if (mode != TranslationMode.None && mode != TranslationMode.GotSwitch)
                            {
                                System.Diagnostics.Trace.TraceError("Unexpected switch term: {0}", token);
                                return false;
                            }

                            mode = TranslationMode.ExpectSwitch;
                            break;
                        }

                    case CommandLineTermKey.Equals:
                    case CommandLineTermKey.Colon:
                        {
                            if (mode != TranslationMode.GotSwitch)
                            {
                                System.Diagnostics.Trace.TraceError("Unexpected argument assignment: {0}", token);
                                return false;
                            }

                            mode = TranslationMode.ExpectArgument;
                            break;
                        }

                    case CommandLineTermKey.String:
                        {
                            if (mode != TranslationMode.ExpectArgument && mode != TranslationMode.GotSwitch)
                            {
                                System.Diagnostics.Trace.TraceError("Unexpected string: {0}", token);
                                return false;
                            }

                            this.UpdateSwitchArguments(currentSwitch, token.Contents);
                            mode = TranslationMode.None;
                            break;
                        }

                    case CommandLineTermKey.Identifier:
                        {
                            if (mode == TranslationMode.ExpectArgument || mode == TranslationMode.GotSwitch)
                            {
                                this.UpdateSwitchArguments(currentSwitch, token.Contents);
                                mode = TranslationMode.None;
                                continue;
                            }

                            if (mode == TranslationMode.ExpectSwitch)
                            {
                                currentSwitch = this.RegisterSwitch(token.Contents);
                                mode = TranslationMode.GotSwitch;
                                continue;
                            }

                            System.Diagnostics.Trace.TraceError("Unexpected identifier: {0}", token);
                            return false;
                        }

                    case CommandLineTermKey.Pipe:
                        {
                            // Abort parsing, this is probably no longer for us
                            return true;
                        }
                }
            }
            
            return true;
        }

        private ICommandLineSwitch RegisterSwitch(string contents)
        {
            if (!this.activeSwitchDictionary.ContainsKey(contents))
            {
                this.activeSwitchDictionary.Add(contents, new CommandLineSwitch { Switch = contents });
                this.activeSwitches.Add(this.activeSwitchDictionary[contents]);
            }

            return this.activeSwitchDictionary[contents];
        }

        private void UpdateSwitchArguments(ICommandLineSwitch @switch, string argument)
        {
            if (string.IsNullOrWhiteSpace(argument))
            {
                return;
            }

            if (@switch.Arguments == null)
            {
                @switch.Arguments = new List<string>();
            }

            @switch.Arguments.Add(argument);
        }

        private bool EvaluateDefines()
        {
            if (this.activeSwitches.Count <= 0)
            {
                return false;
            }

            foreach (ICommandLineSwitchDefinition definition in this.definitions)
            {
                ICommandLineSwitch activeLong = this.GetSwitch(definition.Short);
                ICommandLineSwitch activeShort = this.GetSwitch(definition.Long);
                if (activeLong == null && activeShort == null)
                {
                    if (definition.Required)
                    {
                        System.Diagnostics.Trace.TraceError("Required argument was not present: {0}", definition.Long ?? definition.Short);
                        return false;
                    }

                    continue;
                }

                IList<string> arguments = new List<string>();
                if (activeLong != null && activeLong.Arguments != null)
                {
                    arguments.AddRange(activeLong.Arguments);
                }

                if (activeShort != null && activeShort.Arguments != null)
                {
                    arguments.AddRange(activeShort.Arguments);
                }

                if (arguments.Count > 1 && !definition.AllowMultiple)
                {
                    System.Diagnostics.Trace.TraceError("Argument does not allow multiple values: {0}", definition.Long ?? definition.Short);
                    return false;
                }

                if (arguments.Count <= 0 && definition.RequireArgument)
                {
                    System.Diagnostics.Trace.TraceError("Argument needs value: {0}", definition.Long ?? definition.Short);
                    return false;
                }

                if (!definition.RequireArgument)
                {
                    definition.Set(null);
                }
                else
                {
                    foreach (string argument in arguments)
                    {
                        definition.Set(argument);
                    }
                }
            }

            return true;
        }
    }
}
