namespace CarbonCore.Applications.CrystalBuild.Logic.Processors
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    using CarbonCore.Utils;
    using CarbonCore.Utils.I18N;
    using CarbonCore.Utils.IO;

    using CrystalBuild.Contracts.Processors;

    public enum ProcessingInstructions
    {
        Debug
    }

    public class JavaScriptProcessor : ContentProcessor, IJavaScriptProcessor
    {
        public static readonly Regex IncludeRegex = new Regex(@"\s+(include\((['""]\w+['""])\);)", RegexOptions.IgnoreCase);
        public static readonly Regex ProcessingRegex = new Regex(@"// #If([\w]+)");
        public static readonly Regex StringHashRegex = new Regex(@"\s(StrSha\(['""](.*?)['""]\))");
        public static readonly Regex StringLocRegex = new Regex(@"\s(StrLoc\(['""](.*?)['""]\))");

        private readonly IDictionary<string, string> hashCollisionTest;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public JavaScriptProcessor()
        {
            this.hashCollisionTest = new Dictionary<string, string>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool IsDebug { get; set; }

        public override void Process(CarbonFile file)
        {
            string content = file.ReadAsString();

            this.ProcessSource(file.FileNameWithoutExtension, ref content);
            
            // In debug mode append the file name of the source
            if (this.IsDebug)
            {
                this.AppendFormatLine("// {0}", file.FileNameWithoutExtension);
            }

            this.AppendLine(content);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void ProcessSource(string sourceName, ref string source)
        {
            var processingDirectiveStack = new Stack<ProcessingInstructions>();

            string[] lines = source.Split('\n');
            var trimmedContent = new StringBuilder(lines.Length);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                string trimmed = line.TrimStart();
                if (trimmed.StartsWith(@"//"))
                {
                    if (trimmed.StartsWith("// #EndIf", StringComparison.OrdinalIgnoreCase))
                    {
                        processingDirectiveStack.Pop();
                    }
                    else if (ProcessingRegex.IsMatch(trimmed))
                    {
                        string instructionString = ProcessingRegex.Match(trimmed).Groups[1].ToString();
                        ProcessingInstructions instruction;
                        if (Enum.TryParse(instructionString, out instruction))
                        {
                            processingDirectiveStack.Push(instruction);
                        }
                        else
                        {
                            System.Diagnostics.Trace.TraceWarning("Unknown processing instruction: {0} on line {1}", instructionString, i);
                        }
                    }

                    continue;
                }

                if (processingDirectiveStack.Contains(ProcessingInstructions.Debug) && !this.IsDebug)
                {
                    continue;
                }

                // Replace some things we don't care about
                line = line.Replace("\r", string.Empty);
                line = line.Replace("\t", " ");

                // Fix includes to the proper format
                Match match = IncludeRegex.Match(line);
                if (match.Success)
                {
                    string entry = match.Groups[1].ToString();
                    string name = match.Groups[2].ToString();
                    string varName = string.Concat(char.ToLower(name[1]), name.Substring(2, name.Length - 3));
                    line = line.Replace(entry, string.Format("var {0} = {1}", varName, entry));
                    line = line.Replace(name, string.Format("{0},'{1}'", name, sourceName));
                }

                // Replace StrLoc() with plain localized string
                match = StringLocRegex.Match(line);
                if (match.Success)
                {
                    string localized = match.Groups[2].ToString().Localized();
                    line = line.Replace(match.Groups[1].ToString(), string.Format("\"{0}\"", localized));
                }

                // Replace StrSha() with hash value of the string
                match = StringHashRegex.Match(line);
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
                            System.Diagnostics.Trace.TraceWarning(
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
                    line = line.Replace(expression, string.Format("\"{0}\"", hash));
                }

                trimmedContent.AppendLine(line);
            }

            source = trimmedContent.ToString();
        }
    }
}
