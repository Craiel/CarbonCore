namespace CarbonCore.Processing.Processors.Excel
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    public class ExcelData
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string FileName { get; set; }

        public IList<ExcelDataSheet> Sheets { get; set; }

        public void AddSheet(ExcelDataSheet data)
        {
            if (this.Sheets == null)
            {
                this.Sheets = new List<ExcelDataSheet>();
            }

            this.Sheets.Add(data);
        }
    }
}
