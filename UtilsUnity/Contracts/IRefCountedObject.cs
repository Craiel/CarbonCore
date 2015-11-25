namespace CarbonCore.Utils.Unity.Contracts
{
    public interface IRefCountedObject
    {
        int RefCount();

        void RefCountIncrease();
        void RefCountDecrease();
    }
}
