namespace CarbonCore.ContentServices.Logic.DataEntryLogic
{
    using System;
    using System.Collections.Generic;

    public static class DataEntrySyncDescriptors
    {
        private static readonly IDictionary<Type, DataEntryElementSerializer> InnerSyncSerializers;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static DataEntrySyncDescriptors()
        {
            InnerSyncSerializers = new Dictionary<Type, DataEntryElementSerializer>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static DataEntryElementSerializer GetSerializer(Type type)
        {
            return InnerSyncSerializers[type];
        }

        public static void SetInnerSyncSerializer(Type type, DataEntryElementSerializer serializer)
        {
            if (InnerSyncSerializers.ContainsKey(type))
            {
                return;
            }

            InnerSyncSerializers.Add(type, serializer);
        }
    }
}
