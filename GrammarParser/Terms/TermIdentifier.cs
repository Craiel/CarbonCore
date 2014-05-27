namespace CarbonCore.GrammarParser.Terms
{
    using System.Collections.Generic;

    public class TermIdentifier : BaseTerm
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TermIdentifier(string name, HashSet<char> startCharacters, HashSet<char> fullCharacters, object tag = null)
            : base(name, tag)
        {
            this.StartCharacters = startCharacters;
            this.FullCharacters = fullCharacters;

            this.Type = TermType.Identifier;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public HashSet<char> StartCharacters { get; private set; }
        public HashSet<char> FullCharacters { get; private set; }
    }
}
