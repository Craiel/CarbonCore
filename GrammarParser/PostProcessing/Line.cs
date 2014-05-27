namespace CarbonCore.GrammarParser.PostProcessing
{
    using System.Collections.Generic;

    using CarbonCore.GrammarParser.Contracts;

    public class Line : ILine
    {
        private List<string> precedingContent;
        private List<string> subsequentContent;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Line(string line, string next, int lineNumber)
        {
            this.Current = line;
            this.Processed = line;
            this.Next = next;
            this.TrimmedCurrent = line.Trim();
            
            this.LineNumber = lineNumber;
            this.IsEmpty = string.IsNullOrWhiteSpace(this.Current);

            this.UpdateMode = LineUpdateMode.None;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IReadOnlyCollection<string> PrecedingContent
        {
            get
            {
                if (this.precedingContent == null)
                {
                    return null;
                }

                return this.precedingContent.AsReadOnly();
            }
        }

        public IReadOnlyCollection<string> SubsequentContent
        {
            get
            {
                if (this.subsequentContent == null)
                {
                    return null;
                }

                return this.subsequentContent.AsReadOnly();
            }
        }

        public string Current { get; private set; }
        public string Next { get; private set; }
        public string Previous { get; set; }
        public string TrimmedCurrent { get; private set; }

        public string Processed { get; set; }

        public bool IsEmpty { get; private set; }

        public int LineNumber { get; private set; }
        public int ProcessedLineNumber { get; set; }
        
        public LineUpdateMode UpdateMode { get; set; }

        public int ContentBegin { get; set; }
        public int ContentEnd { get; set; }

        public void AddPrecedingContent(string content)
        {
            if (this.precedingContent == null)
            {
                this.precedingContent = new List<string>();
            }

            this.precedingContent.Add(content);
        }

        public void AddSubsequentContent(string content)
        {
            if (this.subsequentContent == null)
            {
                this.subsequentContent = new List<string>();
            }

            this.subsequentContent.Add(content);
        }
    }
}
