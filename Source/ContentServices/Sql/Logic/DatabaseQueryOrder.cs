namespace CarbonCore.ContentServices.Sql.Logic
{
    using System.Diagnostics;
    
    public class DatabaseQueryOrder
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DatabaseQueryOrder(string name)
        {
            Debug.Assert(!string.IsNullOrEmpty(name));

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
