namespace CarbonCore.ContentServices.Logic.Attributes
{
    using System;

    public class DatabaseEntryJoinedElementAttribute : Attribute
    {
        public DatabaseEntryJoinedElementAttribute(string foreignKeyColumn)
        {
            this.ForeignKeyColumn = foreignKeyColumn;
        }

        public string ForeignKeyColumn { get; private set; }
    }
}
