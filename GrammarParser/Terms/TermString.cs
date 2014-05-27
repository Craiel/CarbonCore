namespace CarbonCore.GrammarParser.Terms
{
    public class TermString : BaseTerm
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TermString(string name, char character, char? escapeChar = null, object tag = null)
            : base(name, tag)
        {
            this.Character = character;
            this.EscapeChar = escapeChar;

            this.Type = TermType.String;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public char Character { get; private set; }
        public char? EscapeChar { get; private set; }
    }
}
