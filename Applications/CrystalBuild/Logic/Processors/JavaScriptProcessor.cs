namespace CarbonCore.Applications.CrystalBuild.Logic.Processors
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;
    
    using CarbonCore.Utils.Edge;
    using CarbonCore.Utils.I18N;
    using CarbonCore.Utils.IO;

    using CrystalBuild.Contracts.Processors;

    public enum ProcessingInstructions
    {
        Debug
    }

    public class JavaScriptProcessor : ContentProcessor, IJavaScriptProcessor
    {
        private const string IncludeTestRegex = @"\W{0}\W";

        public static readonly Regex IncludeRegex = new Regex(@"\s+(include\((['""]\w+['""])\);)", RegexOptions.IgnoreCase);
        public static readonly Regex ProcessingRegex = new Regex(@"// #If([\w]+)");
        public static readonly Regex StringHashRegex = new Regex(@"\s(StrSha\(['""](.*?)['""]\))");
        public static readonly Regex StringLocRegex = new Regex(@"\s(StrLoc\(['""](.*?)['""]\))");
        public static readonly Regex ResourceImageRegex = new Regex(@"[\W](ResImg\((\w+)\))", RegexOptions.IgnoreCase);

        private readonly IDictionary<string, string> hashCollisionTest;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public JavaScriptProcessor()
        {
            this.hashCollisionTest = new Dictionary<string, string>();
        }
        
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void DoProcess(CarbonFile source)
        {
            // Skip template scripts
            if (source.FileNameWithoutExtension.EndsWith("template", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var localContext = new JavaScriptProcessingContext(source);
            string content = source.ReadAsString();

            this.ProcessSource(localContext, ref content);
            
            // In debug mode append the file name of the source
            if (this.Context.IsDebug)
            {
                this.AppendFormatLine("// {0}", source.FileNameWithoutExtension);
            }

            this.AppendLine(content);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void ProcessSource(JavaScriptProcessingContext context, ref string source)
        {
            string[] lines = source.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var trimmedContent = new StringBuilder(lines.Length);
            for (int i = 0; i < lines.Length; i++)
            {
                context.SetLine(i, lines[i]);

                if (this.ProcessComment(context))
                {
                    continue;
                }

                if (context.DirectiveStack.Contains(ProcessingInstructions.Debug) && !this.Context.IsDebug)
                {
                    continue;
                }

                // Order is somewhat important, resources change what gets included
                this.ProcessResources(context);
                this.ProcessIncludes(context);
                this.ProcessLocalization(context);
                this.ProcessHash(context);
                this.ProcessIncludeUsage(context);

                trimmedContent.AppendLine(context.OutputLine);
            }

            this.TraceIncludeUsage(context);

            source = trimmedContent.ToString();
        }

        private bool ProcessComment(JavaScriptProcessingContext context)
        {
            if (context.CurrentTrimmedLine.StartsWith(@"//"))
            {
                if (context.CurrentTrimmedLine.StartsWith("// #EndIf", StringComparison.OrdinalIgnoreCase))
                {
                    context.DirectiveStack.Pop();
                }
                else if (ProcessingRegex.IsMatch(context.CurrentTrimmedLine))
                {
                    string instructionString = ProcessingRegex.Match(context.CurrentTrimmedLine).Groups[1].ToString();
                    ProcessingInstructions instruction;
                    if (Enum.TryParse(instructionString, out instruction))
                    {
                        context.DirectiveStack.Push(instruction);
                    }
                    else
                    {
                        this.Context.AddWarning("Unknown processing instruction: {0} on line {1}", instructionString, context.CurrentLineIndex);
                    }
                }

                return true;
            }

            return false;
        }

        private void ProcessIncludes(JavaScriptProcessingContext context)
        {
            // Fix includes to the proper format
            Match match = IncludeRegex.Match(context.OutputLine);
            if (match.Success)
            {
                string entry = match.Groups[1].ToString();
                string name = match.Groups[2].ToString();
                string varName = string.Concat(char.ToLower(name[1]), name.Substring(2, name.Length - 3));
                context.OutputLine = context.OutputLine.Replace(entry, $"var {varName} = {entry}");
                context.OutputLine = context.OutputLine.Replace(name, $"{name},'{context.SourceName}'");
                if (context.UsingVars.ContainsKey(varName))
                {
                    this.Context.AddError("Duplicate using: {0} in {1}", varName, context.SourceName);
                }
                else
                {
                    context.UsingVars.Add(varName, 0);
                }
            }
        }

        private void ProcessLocalization(JavaScriptProcessingContext context)
        {
            // Replace StrLoc() with plain localized string
            Match match = StringLocRegex.Match(context.OutputLine);
            if (match.Success)
            {
                string localized = match.Groups[2].ToString().Localized();
                context.OutputLine = context.OutputLine.Replace(match.Groups[1].ToString(), $"\"{localized}\"");
            }
        }

        private void ProcessHash(JavaScriptProcessingContext context)
        {
            // Replace StrSha() with hash value of the string
            Match match = StringHashRegex.Match(context.OutputLine);
            if (match.Success)
            {
                string expression = match.Groups[1].ToString();
                string content = match.Groups[2].ToString();
                string hash = HashFileName.GetHashFileName(content).Value;
                if (this.hashCollisionTest.ContainsKey(hash))
                {
                    // If the contents do not match we have a hash collision
                    if (!string.Equals(this.hashCollisionTest[hash], content))
                    {
                        this.Context.AddWarning(
                            "Hash Collision for {0}, \"{1}\" <-> \"{2}\"",
                            hash,
                            this.hashCollisionTest[hash],
                            content);
                    }
                }
                else
                {
                    this.hashCollisionTest.Add(hash, content);
                }

                // Todo: need to actually hash the string with something like .Obfuscate(Constants.ObfuscationValue))
                context.OutputLine = context.OutputLine.Replace(expression, $"\"{hash}\"");
            }
        }

        private void ProcessResources(JavaScriptProcessingContext context)
        {
            MatchCollection matches = ResourceImageRegex.Matches(context.OutputLine);
            foreach (Match match in matches)
            {
                string expression = match.Groups[1].ToString();
                string key = match.Groups[2].Value;
                if (!this.Context.Cache.Images.ContainsKey(key))
                {
                    this.Context.AddError("Missing Image for Key: {0} in file {1} on line {2}", key, context.CurrentLineIndex, context.SourceName);
                    context.OutputLine = context.OutputLine.Replace(expression, "#IMGNOTFOUND:" + key);
                    continue;
                }

                this.Context.Cache.RegisterImageUse(key);
                context.OutputLine = context.OutputLine.Replace(expression, this.Context.Cache.Images[key]);
            }
        }

        private void ProcessIncludeUsage(JavaScriptProcessingContext context)
        {
            IList<string> vars = new List<string>(context.UsingVars.Keys);
            foreach (string @var in vars)
            {
                Regex regex = new Regex(string.Format(IncludeTestRegex, @var));
                if (regex.IsMatch(context.OutputLine))
                {
                    context.UsingVars[@var]++;
                }
            }
        }

        private void TraceIncludeUsage(JavaScriptProcessingContext context)
        {
            foreach (string @var in context.UsingVars.Keys)
            {
                if (context.UsingVars[@var] <= 0)
                {
                    this.Context.AddWarning("Include potentially not used: {0} in {1}", @var, context.SourceName);
                }
            }
        }

        internal class JavaScriptProcessingContext
        {
            // -------------------------------------------------------------------
            // Constructor
            // -------------------------------------------------------------------
            public JavaScriptProcessingContext(CarbonFile source)
            {
                this.DirectiveStack = new Stack<ProcessingInstructions>();
                this.UsingVars = new Dictionary<string, int>();

                this.SourceName = source.FileNameWithoutExtension;
            }

            // -------------------------------------------------------------------
            // Public
            // -------------------------------------------------------------------
            public int CurrentLineIndex { get; private set; }

            public string SourceName { get; }
            public string OutputLine { get; set; }
            public string CurrentLine { get; private set; }
            public string CurrentTrimmedLine { get; private set; }

            public Stack<ProcessingInstructions> DirectiveStack { get; }

            public IDictionary<string, int> UsingVars { get; }

            public void SetLine(int index, string line)
            {
                this.CurrentLineIndex = index;
                this.CurrentLine = line;
                this.OutputLine = line;
                this.CurrentTrimmedLine = line.TrimStart();

                // Replace some things we don't care about
                this.CurrentLine = this.CurrentLine.Replace("\r", string.Empty);
                this.CurrentLine = this.CurrentLine.Replace("\t", " ");
            }
        }
    }
}
