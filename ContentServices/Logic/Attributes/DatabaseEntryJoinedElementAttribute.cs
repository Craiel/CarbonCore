namespace CarbonCore.ContentServices.Logic.Attributes
{
    public class DatabaseEntryJoinedElementAttribute : DataElementAttribute
    {
        public DatabaseEntryJoinedElementAttribute(string foreignKeyColumn)
        {
            this.ForeignKeyColumn = foreignKeyColumn;
        }

        public string ForeignKeyColumn { get; private set; }
    }
}
