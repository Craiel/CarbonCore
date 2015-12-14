namespace Assets.Scripts.InputSystem
{
    public class InputDeviceState
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public float Value { get; set; }

        public bool IsPressed { get; set; }

        public void Reset()
        {
            this.Value = 0f;
            this.IsPressed = false;
        }
    }
}
