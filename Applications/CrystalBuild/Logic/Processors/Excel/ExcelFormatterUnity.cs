namespace CarbonCore.Applications.CrystalBuild.Logic.Processors.Excel
{
    using System.Text;

    using CarbonCore.Processing.Processors.Excel;
    using CarbonCore.Utils;

    public class ExcelFormatterUnity : ExcelFormatter
    {
        private const string DataDelimiter = "        ";
        
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override string DataPrefix => @"{";

        protected override string DataSuffix => @"}";

        protected override void FormatData(ExcelDataSheet sheet, StringBuilder target)
        {
            bool isSingleEntry = sheet.Data.Count == 1;

            target.AppendFormatLine("    {0}: {1}", sheet.Name, isSingleEntry ? "{" : "[");

            for (var i = 0; i < sheet.Data.Count; i++)
            {
                if (sheet.Columns.Count == 1)
                {
                    target.AppendFormat(
                        "{0}\"{1}\": {{{2}: {1}}}",
                        DataDelimiter,
                        sheet.Data[i][0],
                        sheet.Columns[0]);
                }
                else
                {
                    // Write the key for the data
                    if (!isSingleEntry)
                    {
                        target.AppendFormatLine("{0}{{", DataDelimiter);
                    }

                    for (var n = 0; n < sheet.Columns.Count; n++)
                    {
                        // Ignore null values
                        if (string.IsNullOrEmpty(sheet.Data[i][n]))
                        {
                            continue;
                        }

                        target.AppendFormat("{0}    {1}: {2}", DataDelimiter, sheet.Columns[n], sheet.Data[i][n]);
                        if (n < sheet.Columns.Count - 1)
                        {
                            target.AppendLine(",");
                        }
                        else
                        {
                            target.AppendLine();
                        }
                    }

                    if (!isSingleEntry)
                    {
                        target.AppendFormat("{0}}}", DataDelimiter);

                        if (i < sheet.Data.Count - 1)
                        {
                            target.AppendLine(",");
                        }
                        else
                        {
                            target.AppendLine();
                        }
                    }
                }
            }

            target.AppendFormat("    {0}", isSingleEntry ? "}" : "]");
        }

        protected override void FormatArrayData(ExcelDataSheet sheet, StringBuilder target)
        {
            target.AppendFormat("    {0}: [", sheet.Name);

            for (var i = 0; i < sheet.Data.Count; i++)
            {
                target.AppendFormat(@"""{0}""", sheet.Data[i][0]);
                if (i < sheet.Data.Count - 1)
                {
                    target.Append(", ");
                }
            }

            target.AppendFormat(@" ]");
        }
    }
}
