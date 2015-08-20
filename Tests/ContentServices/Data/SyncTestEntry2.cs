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
            this.Id = new SyncObject<string>();
            this.OtherTestString = new SyncObject<string>();
            this.OtherTestFloat = new Sync<float>();
            this.OtherTestBool = new Sync<bool>();
            this.OtherTestLong = new Sync<long>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public SyncObject<string> Id { get; set; }

        public SyncObject<string> OtherTestString { get; set; }

        public Sync<bool> OtherTestBool { get; set; }

        public Sync<float> OtherTestFloat { get; set; }

        public Sync<long> OtherTestLong { get; set; }

        public override bool GetEntryChanged()
        {
            return this.Id.IsChanged
                || this.OtherTestString.IsChanged
                || this.OtherTestFloat.IsChanged
                || this.OtherTestBool.IsChanged
                || this.OtherTestLong.IsChanged;
        }

        public override void Save(Stream target)
        {
            NativeSerialization.Serialize(target, this.Id.IsChanged, this.Id.Value, StringSerializer.Instance.Serialize);
            NativeSerialization.Serialize(target, this.OtherTestString.IsChanged, this.OtherTestString.Value, StringSerializer.Instance.Serialize);
            NativeSerialization.Serialize(target, this.OtherTestBool.IsChanged, this.OtherTestBool.Value, BooleanSerializer.Instance.Serialize);
            NativeSerialization.Serialize(target, this.OtherTestFloat.IsChanged, this.OtherTestFloat.Value, FloatSerializer.Instance.Serialize);
            NativeSerialization.Serialize(target, this.OtherTestLong.IsChanged, this.OtherTestLong.Value, Int64Serializer.Instance.Serialize);
        }

        public override void Load(Stream source)
        {
            this.Id.Value = NativeSerialization.Deserialize(source, this.Id.Value, StringSerializer.Instance.Deserialize);
            this.OtherTestString.Value = NativeSerialization.Deserialize(source, this.OtherTestString.Value, StringSerializer.Instance.Deserialize);
            this.OtherTestBool = NativeSerialization.Deserialize(source, this.OtherTestBool.Value, BooleanSerializer.Instance.Deserialize);
            this.OtherTestFloat = NativeSerialization.Deserialize(source, this.OtherTestFloat.Value, FloatSerializer.Instance.Deserialize);
            this.OtherTestLong = NativeSerialization.Deserialize(source, this.OtherTestLong.Value, Int64Serializer.Instance.Deserialize);
        }

        public override void ResetChangeState(bool state = false)
        {
            this.Id.ResetChangeState(state);
            this.OtherTestString.ResetChangeState(state);
            this.OtherTestBool = this.OtherTestBool.ResetChangeState(state);
            this.OtherTestFloat = this.OtherTestFloat.ResetChangeState(state);
            this.OtherTestLong = this.OtherTestLong.ResetChangeState(state);
        }
    }
}
