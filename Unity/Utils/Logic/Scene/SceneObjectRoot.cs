namespace CarbonCore.Utils.Unity.Logic.Scene
{
    using System;

    using CarbonCore.Unity.Utils.Logic.Scene;

    using UnityEngine;

    using Object = UnityEngine.Object;

    public class SceneObjectRoot
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SceneObjectRoot(SceneObjectContainer parent, GameObject existingObject, bool persistent = false)
        {
            this.GameObject = existingObject;
            this.GameObject.transform.SetParent(parent.GameObject.transform);

            this.Persistent = persistent;
        }

        public SceneObjectRoot(SceneObjectContainer parent, string name, bool persistent = false)
            : this(parent, new GameObject(name), persistent)
        {
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public event Action OnDestroying;
        public event Action OnDestroyed;

        public GameObject GameObject { get; private set; }

        public bool Persistent { get; set; }

        public void Destroy()
        {
            if (this.OnDestroying != null)
            {
                this.OnDestroying();
            }

            if (this.GameObject != null)
            {
                Object.Destroy(this.GameObject);
                this.GameObject = null;
            }

            if (this.OnDestroyed != null)
            {
                this.OnDestroyed();
            }
        }

        public void AddChild(GameObject entry, bool worldPositionStays = false)
        {
            entry.transform.SetParent(this.GameObject.transform, worldPositionStays);
        }

        public void RemoveChild(GameObject entry)
        {
            entry.transform.SetParent(null);
        }
    }
}
