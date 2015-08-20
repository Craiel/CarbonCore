namespace CarbonCore.Tests.ContentServices
{
    using System.IO;

    using CarbonCore.ContentServices.Logic.DataEntryLogic;
    using CarbonCore.ContentServices.Logic.DataEntryLogic.Serializers;

    public class SyncTestEntry2 : SyncEntry
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SyncTestEntry2()
        {
            this.Id = new Sync<string>();
            this.OtherTestString = new Sync<string>();
            this.OtherTestFloat = new Sync<float>();
            this.OtherTestBool = new Sync<bool>();
            this.OtherTestLong = new Sync<long>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Sync<string> Id { get; set; }

        public Sync<string> OtherTestString { get; set; }

        public Sync<bool> OtherTestBool { get; set; }

        public Sync<float> OtherTestFloat { get; set; }

        public Sync<long> OtherTestLong { get; set; }

        public override int Save(Stream target)
        {
            long start = target.Position;
            NativeSerialization.Serialize(target, this.Id.IsChanged, this.Id.Value, StringSerializer.Instance.Serialize);
            NativeSerialization.Serialize(target, this.OtherTestString.IsChanged, this.OtherTestString.Value, StringSerializer.Instance.Serialize);
            NativeSerialization.Serialize(target, this.OtherTestBool.IsChanged, this.OtherTestBool.Value, BooleanSerializer.Instance.Serialize);
            NativeSerialization.Serialize(target, this.OtherTestFloat.IsChanged, this.OtherTestFloat.Value, FloatSerializer.Instance.Serialize);
            NativeSerialization.Serialize(target, this.OtherTestLong.IsChanged, this.OtherTestLong.Value, Int64Serializer.Instance.Serialize);

            return (int)(target.Position - start);
        }

        public override void Load(Stream source)
        {
            this.Id = NativeSerialization.Deserialize(source, this.Id.Value, StringSerializer.Instance.Deserialize);
            this.OtherTestString = NativeSerialization.Deserialize(source, this.OtherTestString.Value, StringSerializer.Instance.Deserialize);
            this.OtherTestBool = NativeSerialization.Deserialize(source, this.OtherTestBool.Value, BooleanSerializer.Instance.Deserialize);
            this.OtherTestFloat = NativeSerialization.Deserialize(source, this.OtherTestFloat.Value, FloatSerializer.Instance.Deserialize);
            this.OtherTestLong = NativeSerialization.Deserialize(source, this.OtherTestLong.Value, Int64Serializer.Instance.Deserialize);
        }

        public override void ResetSyncState(bool state = false)
        {
            this.Id.ResetChangeState(state);
            this.OtherTestString.ResetChangeState(state);
            this.OtherTestBool.ResetChangeState(state);
            this.OtherTestFloat.ResetChangeState(state);
            this.OtherTestLong.ResetChangeState(state);
        }
    }
}
