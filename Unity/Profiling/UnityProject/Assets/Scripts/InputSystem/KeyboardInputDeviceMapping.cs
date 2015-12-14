namespace Assets.Scripts.InputSystem
{
    using System.Collections.Generic;

    using Assets.Scripts.Enums;

    using UnityEngine;

    public class KeyboardInputDeviceMapping : BaseInputDeviceMapping
    {
        private readonly IDictionary<string, IList<KeyboardInputDeviceMappingEntry>> axisMapping;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public KeyboardInputDeviceMapping()
        {
            this.axisMapping = new Dictionary<string, IList<KeyboardInputDeviceMappingEntry>>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void SetAxis(string axis, Controls target, bool isPositive = true)
        {
            System.Diagnostics.Trace.Assert(!this.axisMapping.ContainsKey(axis));

            var entry = new KeyboardInputDeviceMappingEntry { Control = target, IsPositive = isPositive };
            if (!this.axisMapping.ContainsKey(axis))
            {
                this.axisMapping.Add(axis, new List<KeyboardInputDeviceMappingEntry>());
            }

            this.axisMapping[axis].Add(entry);
        }

        public override void Update()
        {
            base.Update();

            foreach (string axis in this.axisMapping.Keys)
            {
                foreach (KeyboardInputDeviceMappingEntry entry in this.axisMapping[axis])
                {
                    this.UpdateState(Input.GetAxis(axis), Input.GetButton(axis), entry.Control, entry.IsPositive);
                }
            }
        }
    }
}
