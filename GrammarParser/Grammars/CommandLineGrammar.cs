namespace CarbonCore.GrammarParser.Grammars
{
    using CarbonCore.GrammarParser.Contracts.Grammars;
    using CarbonCore.GrammarParser.Terms;

    public class CommandLineGrammar : Grammar, ICommandLineGrammar
    {
        private static readonly int[][] RangeValidIdentStart = new[]
                                                                   {
                                                                       new[] { 0, 33 }, new[] { 35, 37 },
                                                                       new[] { 39, 44 }, new[] { 46, 60 },
                                                                       new[] { 62, 123 }, new[] { 125, 65510 }
                                                                   };

        private static readonly int[][] RangeValidIdent = new[]
                                                              {
                                                                  new[] { 0, 33 }, new[] { 35, 37 },
                                                                  new[] { 39, 44 }, new[] { 46, 60 },
                                                                  new[] { 62, 123 }, new[] { 125, 65510 }
                                                              };

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public CommandLineGrammar()
        {
            var stringDefault = new TermString(CommandLineTermKey.String.ToString(), '"', '\\', CommandLineTermKey.String);
            this.AddTerm(stringDefault);

            this.Punctuation = new TermPunctuation();

            var validStartCharacters = InitializeCharacterSet(RangeValidIdentStart);
            var validCharacters = InitializeCharacterSet(RangeValidIdent);
            this.Identifier = new TermIdentifier(CommandLineTermKey.Identifier.ToString(), validStartCharacters, validCharacters, CommandLineTermKey.Identifier);

            this.ToTerm("-", CommandLineTermKey.Dash);
            this.ToTerm("--", CommandLineTermKey.Dash2);
            this.ToTerm("/", CommandLineTermKey.Slash);
            this.ToTerm("=", CommandLineTermKey.Equals);
            this.ToTerm(":", CommandLineTermKey.Colon);
            this.ToTerm("|", CommandLineTermKey.Pipe);
        }
    }
}
