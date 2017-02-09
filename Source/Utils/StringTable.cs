namespace CarbonCore.Utils
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using IO;

    public class StringTable
    {
        private readonly IList<string> headers;
        private readonly IList<IList<string>> rows;

        private IList<string> currentRow;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public StringTable()
        {
            this.headers = new List<string>();
            this.rows = new List<IList<string>>();
            this.currentRow = new List<string>();
            this.rows.Add(this.currentRow);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int RowCount
        {
            get { return this.rows.Count; }
        }

        public int ColumnCount
        {
            get { return this.headers.Count; }
        }

        public void AddHeaders(params string[] headers)
        {
            this.headers.AddRange(headers);
        }

        public void AddColumns(params string[] values)
        {
            this.currentRow.AddRange(values);
        }

        public void AddColumns(params object[] values)
        {
            foreach (object value in values)
            {
                if (value is string)
                {
                    this.currentRow.Add((string)value);
                }
                else
                {
                    this.currentRow.Add(value.ToString());
                }
            }
        }

        public void SetColumn(int index, string value)
        {
            while (this.currentRow.Count - 1 < index)
            {
                this.currentRow.Add(string.Empty);
            }

            this.currentRow[index] = value;
        }

        public void SetActiveRow(int index)
        {
            while (this.rows.Count - 1 < index)
            {
                this.AdvanceRow();
            }

            this.currentRow = this.rows[index];
        }

        public void AdvanceRow()
        {
            this.currentRow = new List<string>(this.headers.Count);
            this.rows.Add(this.currentRow);
        }

        public void SaveAs(string file, string delimiter = "\t")
        {
            this.SaveAs(new CarbonFile(file), delimiter);
        }

        public void SaveAs(CarbonFile file, string delimiter = "\t")
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(string.Join(delimiter, this.headers.ToArray()));

            foreach (IList<string> row in this.rows)
            {
                builder.AppendLine(string.Join(delimiter, row.ToArray()));
            }

            file.WriteAsString(builder.ToString());
        }
    }
}
