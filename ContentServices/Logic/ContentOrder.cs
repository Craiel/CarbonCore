namespace CarbonCore.ContentServices.Logic
{
    using System;

    public class ContentOrder
    {
        public ContentReflectionProperty PropertyInfo { get; set; }
        public bool Ascending { get; set; }

        public override int GetHashCode()
        {
            return Tuple.Create(this.PropertyInfo, this.Ascending).GetHashCode();
        }
    }
}
