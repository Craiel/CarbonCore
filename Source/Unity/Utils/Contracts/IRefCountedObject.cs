namespace CarbonCore.Unity.Utils.Contracts
{
    public interface IRefCountedObject
    {
        int RefCount();

        void RefCountIncrease();
        void RefCountDecrease();
    }
}
