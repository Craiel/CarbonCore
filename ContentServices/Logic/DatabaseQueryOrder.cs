namespace CarbonCore.ContentServices.Logic
{
    using CarbonCore.Utils.Diagnostics;

    public class DatabaseQueryOrder
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DatabaseQueryOrder(string name)
        {
            Diagnostic.Assert(!string.IsNullOrEmpty(name));

            this.Name = name;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name { get; private set; }

        public bool Ascending { get; set; }

        public override int GetHashCode()
        {
            return this.Ascending.GetHashCode();
        }
    }
}
