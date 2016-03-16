namespace CarbonCore.GrammarParser
{
    public abstract class BaseTerm
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected BaseTerm(string name, object tag)
        {
            this.Name = name;
            this.Tag = tag;
            this.Type = TermType.Undefined;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name { get; private set; }

        public object Tag { get; private set; }

        public TermType Type { get; protected set; }
    }
}
