namespace CarbonCore.Utils.Unity.Logic.Scene
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    
    using UnityEngine;

    using Object = UnityEngine.Object;

    public class SceneObjectRoot
    {
        private readonly IList<GameObject> children;

        private readonly IDictionary<GameObject, float> childRegistrationTime;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SceneObjectRoot(SceneObjectContainer parent, GameObject existingObject, bool persistent = false)
        {
            this.GameObject = existingObject;
            this.GameObject.transform.SetParent(parent.GameObject.transform);

            this.Persistent = persistent;

            this.children = new List<GameObject>();
            this.childRegistrationTime = new Dictionary<GameObject, float>();
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

        public int ChildCount => this.children.Count;

        public GameObject GameObject { get; private set; }

        public bool Persistent { get; set; }

        public void Destroy()
        {
            this.OnDestroying?.Invoke();

            Object.Destroy(this.GameObject);
            this.GameObject = null;

            this.OnDestroyed?.Invoke();
        }

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

        [SuppressMessage("ReSharper", "ExpressionIsAlwaysNull")]
        public void Cleanup()
        {
            foreach (GameObject gameObject in new List<GameObject>(this.children))
            {
                if (gameObject != null)
                {
                    continue;
                }

                this.children.Remove(gameObject);
                this.childRegistrationTime.Remove(gameObject);
            }
        }
    }
}
