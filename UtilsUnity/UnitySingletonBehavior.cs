namespace CarbonCore.Utils.Unity
{
    using System;

    using CarbonCore.Utils.Compat.Diagnostics;
    using CarbonCore.Utils.Unity.Scene;

    using UnityEngine;

    public abstract class UnitySingletonBehavior<T> : MonoBehaviour
        where T : UnitySingletonBehavior<T>
    {
        private static T instance;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static bool IsInstanceActive
        {
            get
            {
                return instance != default(T);
            }
        }

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();

                    if (instance == null)
                    {
                        GameObject gameObject = new GameObject(typeof(T).Name);

                        try
                        {
                            instance = gameObject.AddComponent<T>();
                        }
                        catch (Exception e)
                        {
                            Diagnostic.Error("Error trying to add Singleton Component {0}: {1}", typeof(T), e);
                            return null;
                        }
                        
                        if (instance == null)
                        {
                            Diagnostic.Error("Adding Component of type {0} returned null", typeof(T));
                            return null;
                        }

                        DontDestroyOnLoad(gameObject);
                    }
                }

                return instance;
            }
        }

        public virtual void Awake()
        {
            if (instance != null && instance != this)
            {
                Diagnostic.Error("Duplicate Instance of {0} found, destroying!", this.GetType());
                DestroyImmediate(this.gameObject);
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected SceneObjectRoot RegisterInController<TN>(SceneObjectController<TN> parent, TN category, bool persistent = false)
            where TN : struct, IConvertible
        {
            SceneObjectRoot root = parent.RegisterObjectAsRoot(category, this.gameObject, persistent);

            root.OnDestroying += this.OnSingletonDestroying;

            return root;
        }
        
        protected void DisposeSingleton()
        {
            Diagnostic.Info("Disposing Singleton MonoBehavior {0}", this.name);

            this.OnSingletonDestroying();

            Destroy(this.gameObject);
        }

        protected virtual void OnSingletonDestroying()
        {
            // Clear out the instance since our parent got destroyed
            instance = null;
        }
    }
}
