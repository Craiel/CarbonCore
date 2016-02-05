namespace CarbonCore.Applications.CrystalBuild.Logic.Processors.Excel
{
    using System.Collections.Generic;
    using System.Text;

    using CarbonCore.Applications.CrystalBuild.Contracts.Processors;
    using CarbonCore.Processing.Processors.Excel;
    using CarbonCore.Utils.Diagnostics;

    public abstract class ExcelFormatter : IExcelFormatter
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Format(IList<ExcelData> data)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(this.DataPrefix);

            for (var i = 0; i < data.Count; i++)
            {
                this.Format(data[i], builder);

                if (i < data.Count - 1)
                {
                    builder.AppendLine(",");
                }
                else
                {
                    builder.AppendLine();
                }
            }

            builder.AppendLine(this.DataSuffix);
            return builder.ToString();
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected abstract string DataPrefix { get; }

        protected abstract string DataSuffix { get; }

        protected abstract void FormatData(ExcelDataSheet sheet, StringBuilder target);

        protected abstract void FormatArrayData(ExcelDataSheet sheet, StringBuilder target);

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void Format(ExcelData data, StringBuilder target)
        {
            for (int i = 0; i < data.Sheets.Count; i++)
            {
                ExcelDataSheet sheet = data.Sheets[i];
                if (sheet.Data == null || sheet.Data.Count <= 0)
                {
                    Diagnostic.Warning(
                        "Invalid or empty Excel Sheet Data in {0}.{1}",
                        data.FileName,
                        sheet.Name);
                    continue;
                }

                if (sheet.IsArrayData)
                {
                    this.FormatArrayData(sheet, target);
                }
                else
                {
                    if (sheet.Columns == null || sheet.Columns.Count <= 0)
                    {
                        Diagnostic.Warning(
                            "Invalid or empty columns for Sheet Data in {0}.{1}",
                            data.FileName,
                            sheet.Name);
                        continue;
                    }

                    this.FormatData(sheet, target);
                }

                if (i < data.Sheets.Count - 1)
                {
                    target.AppendLine(",");
                }
            }
        }
    }
}
