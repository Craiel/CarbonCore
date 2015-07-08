namespace CarbonCore.Applications.CrystalBuild.Logic.Processors.Excel
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using CarbonCore.Processing.Processors.Excel;
    using CarbonCore.Utils.Compat;

    public class ExcelFormatterJava : ExcelFormatter
    {
        private const string IdFieldKey = "id";

        private const string DataDelimiter = "        ";


        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override string DataPrefix
        {
            get
            {
                return @"declare(""GameData"", function() { return {";
            }
        }

        protected override string DataSuffix
        {
            get
            {
                return @"}; });";
            }
        }

        protected override void FormatData(ExcelDataSheet sheet, StringBuilder target)
        {
            target.AppendFormatLine("    {0}: {{", sheet.Name);

            bool hasIdColumn = sheet.Columns.Any(x => x.Equals(IdFieldKey));

            for (var i = 0; i < sheet.Data.Count; i++)
            {
                string id = sheet.Data[i][0];
                
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
                    target.AppendFormatLine("{0}\"{1}\": {{", DataDelimiter, id.Trim('"'));

                    // Auto-add the ID field if we don't have it and assign it the default value
                    //  Note: This is mostly for easier access in Javascript projects
                    if (!hasIdColumn)
                    {
                        target.AppendFormatLine("{0}    {1}: {2},", DataDelimiter, IdFieldKey, id);
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

                    target.AppendFormat("{0}}}", DataDelimiter);
                }

                if (i < sheet.Data.Count - 1)
                {
                    target.AppendLine(",");
                }
                else
                {
                    target.AppendLine();
                }
            }

            target.Append(@"    }");
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

            target.Append(@" ]");
        }
    }
}
