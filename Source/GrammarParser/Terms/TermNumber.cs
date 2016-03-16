namespace CarbonCore.GrammarParser.Terms
{
    using System.Collections.Generic;
    using System.Linq;

    public enum TermNumberTypeCode
    {
        Undefined,
        Int,
        Int64,
        Float,
        Hex,
        Double
    }

    public class TermNumber : BaseTerm
    {
        private readonly Dictionary<string, TermNumberTypeCode> extensions;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TermNumber(string name, char floatDelimiter = '.', object tag = null)
            : base(name, tag)
        {
            this.FloatDelimiter = floatDelimiter;
            
            this.extensions = new Dictionary<string, TermNumberTypeCode>();

            this.DefaultType = TermNumberTypeCode.Int;
            this.DefaultFloatType = TermNumberTypeCode.Float;

            this.Type = TermType.Number;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public char FloatDelimiter { get; private set; }

        public TermNumberTypeCode DefaultType { get; set; }
        public TermNumberTypeCode DefaultFloatType { get; set; }

        public IList<string> Extensions
        {
            get
            {
                return this.extensions.Keys.ToList();
            }
        }

        public void AddExtension(string value, TermNumberTypeCode type)
        {
            this.extensions.Add(value, type);
        }
    }
}
