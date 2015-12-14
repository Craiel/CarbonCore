namespace Assets.Scripts.Data
{
    using Newtonsoft.Json;
    
    [JsonObject(MemberSerialization.OptOut)]
    public class SaveData
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SaveData()
        {
            this.ResetToDefault();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public float Version { get; set; }

        public void ResetToDefault()
        {
            this.Version = Constants.Version;
        }
    }
}
