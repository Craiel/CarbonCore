namespace Assets.Scripts.InputSystem
{
    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Enums;
    using Assets.Scripts.InputSystem.Contracts;

    public abstract class BaseInputDeviceMapping : IInputDeviceMapping
    {
        private readonly IDictionary<Controls, InputDeviceState> keyState;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected BaseInputDeviceMapping()
        {
            this.keyState = new Dictionary<Controls, InputDeviceState>();
            foreach (Controls control in EnumLists.Controls)
            {
                this.keyState.Add(control, new InputDeviceState());
            }
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public virtual void Update()
        {
            this.ResetState();
        }

        public InputDeviceState GetState(Controls control)
        {
            return this.keyState[control];
        }

        public void ResetState()
        {
            foreach (Controls control in this.keyState.Keys)
            {
                this.keyState[control].Reset();
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected void UpdateState(float value, bool pressed, Controls target, bool isPositive)
        {
            if (Math.Abs(value) < float.Epsilon)
            {
                // Nothing do do
                return;
            }

            if ((isPositive && value < 0) || (!isPositive && value > 0))
            {
                // The value is invalid for the requirements of the mapping
                return;
            }

            InputDeviceState state = this.keyState[target];
            state.Value = isPositive ? value : -value;
            state.IsPressed = pressed;
        }
    }
}
