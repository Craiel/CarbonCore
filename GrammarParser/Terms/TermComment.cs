namespace CarbonCore.GrammarParser.Terms
{
    using System;

    public class TermComment : BaseTerm
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TermComment(string name, string start, object tag = null, params string[] end)
            : base(name, tag)
        {
            if (string.IsNullOrWhiteSpace(start) || end == null || end.Length <= 0)
            {
                throw new ArgumentException();
            }

            this.Start = start;
            this.End = end;
            this.Type = TermType.Comment;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Start { get; private set; }
        public string[] End { get; private set; }
    }
}
