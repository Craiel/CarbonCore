namespace CarbonCore.Applications.CrystalBuild.Logic.Processors
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;

    using CarbonCore.Utils.Compat.IO;

    using CrystalBuild.Contracts.Processors;

    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;

    public class ExcelProcessor : ContentProcessor, IExcelProcessor
    {
        private const string DataPrefixJava = @"declare(""GameData"", function() { return {";
        private const string DataSuffixJava = @"}; });";

        private const string DataPrefixUnity = @"{";
        private const string DataSuffixUnity = @"}";

        private const string IdFieldKey = "id";

        private const string JavaDataDelimiter = "        ";

        private const string DataEndMarker = "-END-";

        private readonly IList<string> sectionDuplicateCheck;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ExcelProcessor()
        {
            this.sectionDuplicateCheck = new List<string>();
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
                        IDictionary<string, IDictionary<string, string>> data = this.BuildDataEntrySection(sheet);
                        if (data == null)
                        {
                            continue;
                        }

                        switch (this.Context.TargetPlatform)
                        {
                            case BuildTargetPlatform.Java:
                                {
                                    this.FormatDataForJava(sheet.SheetName, data);
                                    break;
                                }

                            case BuildTargetPlatform.Unity:
                                {
                                    this.FormatDataForUnity(sheet.SheetName, data);
                                    break;
                                }
                        }
                    }
                    else
                    {
                        IList<string> data = this.BuildDataEntryArray(sheet);
                        switch (this.Context.TargetPlatform)
                        {
                                case BuildTargetPlatform.Java:
                                {
                                    this.FormatDataForJava(sheet.SheetName, data);
                                    break;
                                }

                            case BuildTargetPlatform.Unity:
                                {
                                    this.FormatDataForUnity(sheet.SheetName, data);
                                    break;
                                }
                        }
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

            string prefix;
            string suffix;
            switch (this.Context.TargetPlatform)
            {
                case BuildTargetPlatform.Java:
                    {
                        prefix = DataPrefixJava;
                        suffix = DataSuffixJava;
                        break;
                    }

                case BuildTargetPlatform.Unity:
                    {
                        prefix = DataPrefixUnity;
                        suffix = DataSuffixUnity;
                        break;
                    }

                default:
                    {
                        throw new NotImplementedException();
                    }
            }

            return string.Concat(prefix, formattedData, Environment.NewLine, suffix);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void FormatDataForJava(string sheetName, IDictionary<string, IDictionary<string, string>> data)
        {
            this.AppendFormatLine("    {0}: {{", sheetName);

            IList<string> keys = new List<string>(data.Keys);
            for (var i = 0; i < keys.Count; i++)
            {
                string id = keys[i];

                IList<string> columns = new List<string>(data[id].Keys);

                if (columns.Count <= 0)
                {
                    continue;
                }

                if (columns.Count == 1)
                {
                    this.AppendFormat(
                        "{0}\"{1}\": {{{2}: {3}}}",
                        JavaDataDelimiter,
                        id.Trim('"'),
                        columns[0],
                        data[id][columns[0]]);
                }
                else
                {
                    // Write the key for the data
                    this.AppendFormatLine("{0}\"{1}\": {{", JavaDataDelimiter, id.Trim('"'));

                    this.AppendFormat("{0}    {1}: \"{2}\",", JavaDataDelimiter, IdFieldKey, id);
                    for (var n = 0; n < columns.Count; n++)
                    {
                        this.AppendFormat("{0}    {1}: {2}", JavaDataDelimiter, columns[n], data[id][columns[n]]);
                        if (n < columns.Count - 1)
                        {
                            this.AppendLine(",");
                        }
                        else
                        {
                            this.AppendLine();
                        }
                    }

                    this.AppendFormat("{0}}}", JavaDataDelimiter);
                }

                if (i < keys.Count - 1)
                {
                    this.AppendLine(",");
                }
                else
                {
                    this.AppendLine();
                }
            }

            this.AppendLine(@"    },");
        }

        private void FormatDataForJava(string sheetName, IList<string> data)
        {
            this.AppendFormat("    {0}: [", sheetName);

            for (var i = 0; i < data.Count; i++)
            {
                this.AppendFormat(@"""{0}""", data[i]);
                if (i < data.Count - 1)
                {
                    this.Append(", ");
                }
            }

            this.AppendLine(@" ],");
        }

        private void FormatDataForUnity(string sheetName, IDictionary<string, IDictionary<string, string>> data)
        {
            bool isSingleEntry = data.Count == 1;

            this.AppendFormatLine("    {0}: {1}", sheetName, isSingleEntry ? "{" : "[");

            IList<string> keys = new List<string>(data.Keys);
            for (var i = 0; i < keys.Count; i++)
            {
                string id = keys[i];

                IList<string> columns = new List<string>(data[id].Keys);

                if (columns.Count <= 0)
                {
                    continue;
                }

                if (columns.Count == 1)
                {
                    this.AppendFormat(
                        "{0}\"{1}\": {{{2}: {3}}}",
                        JavaDataDelimiter,
                        id.Trim('"'),
                        columns[0],
                        data[id][columns[0]]);
                }
                else
                {
                    // Write the key for the data
                    this.AppendFormatLine("{0}{1}", JavaDataDelimiter, isSingleEntry ? string.Empty : "{");
                    
                    for (var n = 0; n < columns.Count; n++)
                    {
                        this.AppendFormat("{0}    {1}: {2}", JavaDataDelimiter, columns[n], data[id][columns[n]]);
                        if (n < columns.Count - 1)
                        {
                            this.AppendLine(",");
                        }
                        else
                        {
                            this.AppendLine();
                        }
                    }

                    this.AppendFormat("{0}{1}", JavaDataDelimiter, isSingleEntry ? string.Empty : "}");
                }

                if (i < keys.Count - 1)
                {
                    this.AppendLine(",");
                }
                else
                {
                    this.AppendLine();
                }
            }

            this.AppendLine(string.Format("    {0},", isSingleEntry ? "}" : "]"));
        }

        private void FormatDataForUnity(string sheetName, IList<string> data)
        {
            this.AppendFormat("    {0}: [", sheetName);

            for (var i = 0; i < data.Count; i++)
            {
                this.AppendFormat(@"""{0}""", data[i]);
                if (i < data.Count - 1)
                {
                    this.Append(", ");
                }
            }

            this.AppendLine(@" ],");
        }

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

        private IList<string> BuildDataEntryArray(ISheet sheet)
        {
            IList<string> results = new List<string>();

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
                results.Add(entries[i]);
            }

            return results;
        }

        private IDictionary<string, IDictionary<string, string>> BuildDataEntrySection(ISheet sheet)
        {
            IDictionary<string, IDictionary<string, string>> results = new Dictionary<string, IDictionary<string, string>>();

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
            IList<string> primaryKeyCheck = new List<string>();
            while (rowEnum.MoveNext())
            {
                row = (XSSFRow)rowEnum.Current;
                cellEnum = row.CellIterator();
                
                string header;
                string value;
                if (!this.GetNextCell(cellEnum, headers, out header, out value))
                {
                    // Reached the end of the cell
                    break;
                }

                string key = value.Trim('"');
                if (key.Equals(DataEndMarker, StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                string idString = string.Format(@"""{0}""", key);
                if (key.Contains(" "))
                {
                    // Quote the key if it contains whitespace
                    key = idString;
                }

                if (primaryKeyCheck.Contains(key))
                {
                    this.Context.AddWarning("Duplicate or invalid primary key data in sheet {0}: {1}", sheet.SheetName, key);
                    continue;
                }

                primaryKeyCheck.Add(key);

                IList<string> rowHeaders = new List<string>();
                IList<string> rowValues = new List<string>();
                while(this.GetNextCell(cellEnum, headers, out header, out value))
                {
                    rowHeaders.Add(header);
                    rowValues.Add(value);
                }

                IDictionary<string, string> entry = new Dictionary<string, string>();
                if (rowValues.Count > 0)
                {
                    for (var i = 0; i < rowHeaders.Count; i++)
                    {
                        entry.Add(rowHeaders[i], rowValues[i]);
                    }
                }

                results.Add(idString, entry);
            }

            this.AppendLine();

            return results;
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
