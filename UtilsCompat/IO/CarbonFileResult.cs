namespace CarbonCore.Utils.Compat.IO
{
    public class CarbonFileResult
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public CarbonDirectory Root { get; set; }

        public CarbonFile Relative { get; set; }
        public CarbonFile Absolute { get; set; }

        public override int GetHashCode()
        {
            return this.Absolute.GetHashCode();
        }

        public override string ToString()
        {
            return this.Absolute.ToString();
        }

        public override bool Equals(object obj)
        {
            var typed = obj as CarbonFileResult;
            if (typed == null)
            {
                return false;
            }

            return this.Absolute.Equals(typed.Absolute);
        }
    }
}
