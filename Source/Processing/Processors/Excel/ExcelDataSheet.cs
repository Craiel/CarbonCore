namespace CarbonCore.Processing.Processors.Excel
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    public class ExcelDataSheet
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name { get; set; }

        public bool IsArrayData { get; set; }

        public IList<string> Columns { get; set; }

        public IList<IList<string>> Data { get; set; }

        public void AddData(IList<string> values)
        {
            if (this.Data == null)
            {
                this.Data = new List<IList<string>>();
            }

            this.Data.Add(values);
        }

        public void AddColumn(string column)
        {
            if (this.Columns == null)
            {
                this.Columns = new List<string>();
            }

            this.Columns.Add(column);
        }
    }
}
