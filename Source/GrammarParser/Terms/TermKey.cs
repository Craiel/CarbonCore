namespace CarbonCore.GrammarParser.Terms
{
    using System.Diagnostics;

    [DebuggerDisplay("Keyword={Keyword}")]
    public class TermKey : BaseTerm
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TermKey(string name, string keyword, object tag = null)
            : base(name, tag)
        {
            this.Keyword = keyword;

            this.Type = TermType.Key;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Keyword { get; private set; }
    }
}
