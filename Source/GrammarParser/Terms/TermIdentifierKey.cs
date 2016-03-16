namespace CarbonCore.GrammarParser.Terms
{
    using System.Diagnostics;

    [DebuggerDisplay("Keyword={Keyword}")]
    public class TermIdentifierKey : BaseTerm
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TermIdentifierKey(string name, string keyword, object tag = null)
            : base(name, tag)
        {
            this.Keyword = keyword;

            this.Type = TermType.IdentifierKey;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Keyword { get; private set; }
    }
}
