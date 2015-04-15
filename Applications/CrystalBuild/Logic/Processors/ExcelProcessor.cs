namespace CarbonCore.Applications.CrystalBuild.Logic.Processors
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;

    using CarbonCore.Utils.IO;

    using CrystalBuild.Contracts.Processors;

    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;

    public class ExcelProcessor : ContentProcessor, IExcelProcessor
    {
        private const string DataPrefix = @"declare(""GameData"", function() { return {";
        private const string DataSuffix = @"}; });";

        private readonly IList<string> sectionDuplicateCheck;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ExcelProcessor()
        {
            this.sectionDuplicateCheck = new List<string>();
            this.AppendLine(DataPrefix);
        }
        
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void DoProcess(CarbonFile source)
        {
            using (FileStream stream = source.OpenRead())
            {
                var workBook = new XSSFWorkbook(stream);
                System.Diagnostics.Trace.TraceInformation("  - {0} Sheet(s)", workBook.NumberOfSheets);
                for (var i = 0; i < workBook.NumberOfSheets; i++)
                {
                    ISheet sheet = workBook.GetSheetAt(i);
                    if (sheet.PhysicalNumberOfRows <= 1)
                    {
                        this.Context.AddWarning("Sheet has insufficient rows!");
                        continue;
                    }

                    if (this.sectionDuplicateCheck.Contains(sheet.SheetName))
                    {
                        this.Context.AddWarning("Skipping Duplicate sheet name: {0} in {1}", sheet.SheetName, source.FileName);
                        continue;
                    }

                    int columns = this.GetColumnCount(sheet);
                    if (columns > 1)
                    {
                        this.AppendFormatLine("    {0}: {{", sheet.SheetName);
                        this.BuildDataEntrySection(sheet, "        ");
                        this.AppendLine(@"    },");
                    }
                    else
                    {
                        this.AppendFormat("    {0}: [", sheet.SheetName);
                        this.BuildDataEntryArray(sheet);
                        this.AppendLine(@" ],");
                    }

                    this.sectionDuplicateCheck.Add(sheet.SheetName);
                }
            }
        }

        protected override string PostProcessData(string data)
        {
            string formattedData = data;
            string dataEnd = string.Concat(",", Environment.NewLine);
            if (formattedData.EndsWith(dataEnd))
            {
                formattedData = formattedData.Substring(0, formattedData.Length - dataEnd.Length);
            }

            return string.Concat(formattedData, Environment.NewLine, DataSuffix);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private int GetColumnCount(ISheet sheet)
        {
            IEnumerator rowEnum = sheet.GetRowEnumerator();
            rowEnum.MoveNext();
            var row = (XSSFRow)rowEnum.Current;
            IEnumerator cellEnum = row.CellIterator();
            int count = 0;
            while (cellEnum.MoveNext())
            {
                count++;
            }

            return count;
        }

        private void BuildDataEntryArray(ISheet sheet)
        {
            // Get the row enum
            IEnumerator rowEnum = sheet.GetRowEnumerator();

            // Read the entries
            IList<string> entries = new List<string>();
            while (rowEnum.MoveNext())
            {
                var row = (XSSFRow)rowEnum.Current;
                if (row.Cells.Count <= 0)
                {
                    continue;
                }

                entries.Add(row.Cells[0].StringCellValue);
            }

            for (var i = 0; i < entries.Count; i++)
            {
                this.AppendFormat(@"""{0}""", entries[i]);
                if (i < entries.Count - 1)
                {
                    this.Append(", ");
                }
            }
        }

        private void BuildDataEntrySection(ISheet sheet, string delimiter)
        {
            // Get the row enum
            IEnumerator rowEnum = sheet.GetRowEnumerator();

            // Read the headers
            rowEnum.MoveNext();
            var row = (XSSFRow)rowEnum.Current;
            IList<string> headers = new List<string>();
            IEnumerator cellEnum = row.CellIterator();
            while (cellEnum.MoveNext())
            {
                headers.Add(cellEnum.Current.ToString());
            }

            // Read the data
            bool concatSection = false;
            IList<string> primaryKeyCheck = new List<string>();
            while (rowEnum.MoveNext())
            {
                row = (XSSFRow)rowEnum.Current;
                cellEnum = row.CellIterator();

                if (concatSection)
                {
                    this.AppendLine(",");
                }

                string header;
                string value;
                if (!this.GetNextCell(cellEnum, headers, out header, out value))
                {
                    this.Context.AddWarning("Sheet has no columns, skipping!");
                    return;
                }

                value = value.Trim('"');
                if (primaryKeyCheck.Contains(value) || value.Contains(" "))
                {
                    this.Context.AddWarning("Duplicate or invalid primary key data in sheet {0}: {1}", sheet.SheetName, value);
                    continue;
                }

                primaryKeyCheck.Add(value);

                // We use the first column as key for the data
                this.AppendFormatLine("{0}{1}: {{", delimiter, value);
                this.AppendFormat("{0}    id: '{1}'", delimiter, value);

                while (this.GetNextCell(cellEnum, headers, out header, out value))
                {
                    this.AppendLine(",");
                    this.AppendFormat("{0}    {1}: {2}", delimiter, header, value);
                }

                this.AppendLine();
                this.AppendFormat("{0}}}", delimiter);
                concatSection = true;
            }

            this.AppendLine();
        }

        private bool GetNextCell(IEnumerator cellEnum, IList<string> headers, out string header, out string value)
        {
            header = null;
            value = null;

            if (cellEnum.MoveNext())
            {
                var cellData = (XSSFCell)cellEnum.Current;
                if (headers.Count < cellData.ColumnIndex + 1 || string.IsNullOrEmpty(headers[cellData.ColumnIndex]))
                {
                    return false;
                }

                header = headers[cellData.ColumnIndex];
                value = this.BuildDataEntryValue(cellData.ToString());
                return true;
            }

            return false;
        }

        private string BuildDataEntryValue(string source)
        {
            int intResult;
            if (int.TryParse(source, out intResult))
            {
                return source;
            }

            string invariantSource = source.ToLowerInvariant();

            bool boolResult;
            if (bool.TryParse(invariantSource, out boolResult))
            {
                return boolResult ? "true" : "false";
            }

            // Libre office does this case:
            if (invariantSource == "true()")
            {
                return "true";
            }

            float floatResult;
            if (float.TryParse(source, out floatResult))
            {
                return source.Replace(",", ".");
            }

            return string.Format(@"""{0}""", source);
        }
    }
}
