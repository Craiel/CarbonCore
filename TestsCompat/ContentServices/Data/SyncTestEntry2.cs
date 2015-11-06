namespace CarbonCore.Tests.Compat.ContentServices.Data
{
    using System;
    using System.IO;

    using CarbonCore.ContentServices.Compat.Logic.DataEntryLogic;
    using CarbonCore.ContentServices.Compat.Logic.DataEntryLogic.Serializers;
    using CarbonCore.Utils.Compat;

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

        public override bool IsChanged
        {
            get
            {
                return this.Id.IsChanged 
                    || this.OtherTestString.IsChanged 
                    || this.OtherTestFloat.IsChanged
                    || this.OtherTestBool.IsChanged 
                    || this.OtherTestLong.IsChanged;
            }
        }

        public override void Save(Stream target, bool ignoreChangeState = false)
        {
            NativeSerialization.Serialize(target, ignoreChangeState || this.Id.IsChanged, this.Id.Value, StringSerializer.Instance.Serialize);
            NativeSerialization.Serialize(target, ignoreChangeState || this.OtherTestString.IsChanged, this.OtherTestString.Value, StringSerializer.Instance.Serialize);
            NativeSerialization.Serialize(target, ignoreChangeState || this.OtherTestBool.IsChanged, this.OtherTestBool.Value, BooleanSerializer.Instance.Serialize);
            NativeSerialization.Serialize(target, ignoreChangeState || this.OtherTestFloat.IsChanged, this.OtherTestFloat.Value, FloatSerializer.Instance.Serialize);
            NativeSerialization.Serialize(target, ignoreChangeState || this.OtherTestLong.IsChanged, this.OtherTestLong.Value, Int64Serializer.Instance.Serialize);
        }

        public override void Load(Stream source)
        {
            this.Id.Value = NativeSerialization.Deserialize(source, this.Id.Value, StringSerializer.Instance.Deserialize);
            this.OtherTestString.Value = NativeSerialization.Deserialize(source, this.OtherTestString.Value, StringSerializer.Instance.Deserialize);
            this.OtherTestBool.Value = NativeSerialization.Deserialize(source, this.OtherTestBool.Value, BooleanSerializer.Instance.Deserialize);
            this.OtherTestFloat.Value = NativeSerialization.Deserialize(source, this.OtherTestFloat.Value, FloatSerializer.Instance.Deserialize);
            this.OtherTestLong.Value = NativeSerialization.Deserialize(source, this.OtherTestLong.Value, Int64Serializer.Instance.Deserialize);
        }

        public override void ResetChangeState(bool state = false)
        {
            this.Id.ResetChangeState(state);
            this.OtherTestString.ResetChangeState(state);
            this.OtherTestBool.ResetChangeState(state);
            this.OtherTestFloat.ResetChangeState(state);
            this.OtherTestLong.ResetChangeState(state);
        }

        public override bool Equals(object obj)
        {
            var typed = obj as SyncTestEntry2;
            if (typed == null)
            {
                return false;
            }

            return this.Id.Value == typed.Id.Value
                && Math.Abs(this.OtherTestFloat.Value - typed.OtherTestFloat.Value) < float.Epsilon
                && this.OtherTestBool.Value == typed.OtherTestBool.Value
                && this.OtherTestLong.Value == typed.OtherTestLong.Value
                && this.OtherTestString.Value == typed.OtherTestString.Value;
        }

        public override int GetHashCode()
        {
            return
                HashUtils.CombineObjectHashes(
                    new[]
                        {
                            (object)this.Id.Value,
                            this.OtherTestFloat.Value,
                            this.OtherTestBool.Value,
                            this.OtherTestLong.Value,
                            this.OtherTestString.Value
                        });
        }
    }
}
