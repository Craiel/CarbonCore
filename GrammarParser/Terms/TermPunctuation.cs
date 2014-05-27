namespace CarbonCore.GrammarParser.Terms
{
    public class TermPunctuation : BaseTerm
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TermPunctuation(object tag = null, params char[] entries)
            : base("Punctuation", tag)
        {
            this.Entries = entries;
            this.Type = TermType.Puncutation;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public char[] Entries { get; private set; }
    }
}
