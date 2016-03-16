namespace Assets.Scripts.InputSystem.Contracts
{
    using Assets.Scripts.Enums;

    public interface IInputDeviceMapping
    {
        void Update();

        InputDeviceState GetState(Controls control);

        void ResetState();
    }
}
