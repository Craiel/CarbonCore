namespace CarbonCore.Utils.Unity.Scene
{
    using System.Collections.Generic;

    using UnityEngine;

    public class SceneObjectRoot
    {
        private readonly IList<GameObject> children;

        private readonly IDictionary<GameObject, float> childRegistrationTime;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SceneObjectRoot(SceneContainer parent, GameObject existingObject)
        {
            this.GameObject = existingObject;
            this.GameObject.transform.SetParent(parent.GameObject.transform);

            this.children = new List<GameObject>();
            this.childRegistrationTime = new Dictionary<GameObject, float>();
        }
        
        public SceneObjectRoot(SceneContainer parent, string name)
            : this(parent, new GameObject(name))
        {
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int ChildCount
        {
            get
            {
                return this.children.Count;
            }
        }

        public GameObject GameObject { get; private set; }

        public void AddChild(GameObject entry, bool worldPositionStays = false)
        {
            entry.transform.SetParent(this.GameObject.transform, worldPositionStays);

            if (!this.children.Contains(entry))
            {
                this.children.Add(entry);
                this.childRegistrationTime.Add(entry, Time.time);
            }
        }

        public void RemoveChild(GameObject entry)
        {
            this.children.Remove(entry);
            this.childRegistrationTime.Remove(entry);

            entry.transform.SetParent(null);
        }

        public float GetAge(GameObject entry)
        {
            return Time.time - this.childRegistrationTime[entry];
        }
    }
}
