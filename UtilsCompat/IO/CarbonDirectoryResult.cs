namespace CarbonCore.Utils.Compat.IO
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    public class CarbonDirectoryResult
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public CarbonDirectory Relative { get; set; }
        public CarbonDirectory Absolute { get; set; }

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
            var typed = obj as CarbonDirectoryResult;
            if (typed == null)
            {
                return false;
            }

            return this.Absolute.Equals(typed.Absolute);
        }
    }
}
