namespace CarbonCore.Processing.Processors.Excel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    
    using CarbonCore.Utils.IO;

    using NLog;

    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;

    public static class ExcelProcessor
    {
        private const string DataEndMarker = "-END-";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static ExcelData Process(CarbonFile source)
        {
            using (FileStream stream = source.OpenRead())
            {
                var workBook = new XSSFWorkbook(stream);
                Logger.Info("  - {0} Sheet(s)", workBook.NumberOfSheets);
                ExcelData data = new ExcelData { FileName = source.FileName };

                for (var i = 0; i < workBook.NumberOfSheets; i++)
                {
                    ISheet sheet = workBook.GetSheetAt(i);
                    if (sheet.PhysicalNumberOfRows <= 1)
                    {
                        Logger.Warn("Sheet has insufficient rows: {0}", source.FileName);
                        continue;
                    }
                    
                    ExcelDataSheet sheetData;
                    int columns = GetColumnCount(sheet);
                    if (columns > 1)
                    {
                        sheetData = BuildDataEntrySection(sheet);
                        if (sheetData == null)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        sheetData = BuildDataEntryArray(sheet);
                        if (sheetData == null)
                        {
                            continue;
                        }
                    }

                    data.AddSheet(sheetData);
                }

                return data;
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static int GetColumnCount(ISheet sheet)
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

        private static ExcelDataSheet BuildDataEntryArray(ISheet sheet)
        {
            var result = new ExcelDataSheet { Name = sheet.SheetName, IsArrayData = true };

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

            result.AddData(entries);
            return result;
        }

        private static ExcelDataSheet BuildDataEntrySection(ISheet sheet)
        {
            var result = new ExcelDataSheet { Name = sheet.SheetName };

            // Get the row enum
            IEnumerator rowEnum = sheet.GetRowEnumerator();

            // Read the headers
            rowEnum.MoveNext();
            var row = (XSSFRow)rowEnum.Current;
            IList<string> headers = new List<string>();
            IEnumerator cellEnum = row.CellIterator();
            while (cellEnum.MoveNext())
            {
                string columnHeader = cellEnum.Current.ToString();
                if (columnHeader.Equals(DataEndMarker, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                headers.Add(columnHeader);
                result.AddColumn(columnHeader);
            }

            // Read the data
            IList<string> primaryKeyCheck = new List<string>();
            while (rowEnum.MoveNext())
            {
                row = (XSSFRow)rowEnum.Current;
                cellEnum = row.CellIterator();

                // Get all the columns in this row
                string header;
                string value;
                IDictionary<string, string> rowValues = new Dictionary<string, string>();
                while (GetNextCell(cellEnum, headers, out header, out value))
                {
                    if (header.Equals(DataEndMarker, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    rowValues.Add(header, value);
                }

                // Ensure that we found at least one value
                if (rowValues.Count <= 0)
                {
                    break;
                }

                // The first column is always assumed to be the key
                string key = rowValues.First().Value.Trim('"');
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
                    Logger.Warn("Duplicate or invalid primary key data in sheet {0}: {1}", sheet.SheetName, key);
                    continue;
                }

                primaryKeyCheck.Add(key);

                IList<string> sequentialValues = new List<string>();
                foreach (string column in result.Columns)
                {
                    sequentialValues.Add(rowValues.ContainsKey(column) ? rowValues[column] : null);
                }

                result.AddData(sequentialValues);
            }

            return result;
        }

        private static bool GetNextCell(IEnumerator cellEnum, IList<string> headers, out string header, out string value)
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
                value = BuildDataEntryValue(cellData.ToString());
                return true;
            }

            return false;
        }

        private static string BuildDataEntryValue(string source)
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
