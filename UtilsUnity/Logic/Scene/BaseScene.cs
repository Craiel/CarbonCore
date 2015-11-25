// ReSharper disable IteratorMethodResultIsIgnored
namespace CarbonCore.Utils.Unity.Logic.Scene
{
    using System;

    using CarbonCore.Utils.Compat.Diagnostics;
    using CarbonCore.Utils.Unity.Contracts;
    using CarbonCore.Utils.Unity.Logic.Enums;
    using CarbonCore.Utils.Unity.Logic.Resource;

    using UnityEngine;

    public abstract class BaseScene : IScene
    {
        private AsyncOperation loadLevelOperation;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected BaseScene(string name, string levelName)
        {
            this.Name = name;
            this.LevelName = levelName;
            this.UseAsyncLoading = true;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name { get; }

        public string LevelName { get; }

        public bool HadErrors { get; private set; }
        
        public bool ContinueLoad(SceneTransitionStep step)
        {
            if (this.HadErrors)
            {
                // On errors we keep the loading active indefinitly
                return true;
            }

            try
            {
                switch (step)
                {
                    case SceneTransitionStep.Initialize:
                        {
                            return this.TransitionInitialize(true);
                        }

                    case SceneTransitionStep.PreLoad:
                        {
                            return this.ScenePreLoad();
                        }

                    case SceneTransitionStep.LoadRegisterResources1:
                        {
                            return this.SceneRegisterResources1();
                        }

                    case SceneTransitionStep.LoadResources1:
                        {
                            return this.SceneLoadResources1();
                        }

                    case SceneTransitionStep.LoadRegisterResources2:
                        {
                            return this.SceneRegisterResources2();
                        }

                    case SceneTransitionStep.LoadResources2:
                        {
                            return this.SceneLoadResources2();
                        }

                    case SceneTransitionStep.LoadRegisterResources3:
                        {
                            return this.SceneRegisterResources3();
                        }

                    case SceneTransitionStep.LoadResources3:
                        {
                            return this.SceneLoadResources3();
                        }

                    case SceneTransitionStep.Load:
                        {
                            return this.SceneLoad();
                        }

                    case SceneTransitionStep.PostLoad:
                        {
                            return this.ScenePostLoad();
                        }

                    case SceneTransitionStep.Finalize:
                        {
                            return this.TransitionFinalize(true);
                        }

                    default:
                        {
                            throw new InvalidOperationException();
                        }
                }
            }
            catch (Exception e)
            {
                this.HadErrors = true;
                Diagnostic.Error("Error in Load of Scene {0}({1}): {2}", this.GetType(), step, e);
                return true;
            }
        }

        public bool ContinueDestroy(SceneTransitionStep step)
        {
            if (this.HadErrors)
            {
                // On errors we keep the loading active indefinitly
                return true;
            }

            try
            {
                switch (step)
                {
                    case SceneTransitionStep.Initialize:
                        {
                            return this.TransitionInitialize(false);
                        }

                    case SceneTransitionStep.PreDestroy:
                        {
                            return this.ScenePreDestroy();
                        }

                    case SceneTransitionStep.Destroy:
                        {
                            return this.SceneDestroy();
                        }

                    case SceneTransitionStep.PostDestroy:
                        {
                            return this.ScenePostDestroy();
                        }

                    case SceneTransitionStep.Finalize:
                        {
                            return this.TransitionFinalize(false);
                        }

                    default:
                        {
                            throw new InvalidOperationException();
                        }
                }
            }
            catch (Exception e)
            {
                this.HadErrors = true;
                Diagnostic.Error("Error in Destroy of Scene {0}({1}): {2}", this.GetType(), step, e);
                return true;
            }
        }

        public virtual void SetData(object[] data)
        {
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected bool UseAsyncLoading { get; set; }

        protected virtual bool TransitionInitialize(bool isLoadTransition)
        {
            return false;
        }

        protected virtual bool TransitionFinalize(bool isLoadTransition)
        {
            return false;
        }

        protected virtual bool SceneRegisterResources1()
        {
            return false;
        }

        protected virtual bool SceneLoadResources1()
        {
            if (this.LoadBundlesAndResources())
            {
                return true;
            }

            return false;
        }

        protected virtual bool SceneRegisterResources2()
        {
            return false;
        }

        protected virtual bool SceneLoadResources2()
        {
            if (this.LoadBundlesAndResources())
            {
                return true;
            }

            return false;
        }

        protected virtual bool SceneRegisterResources3()
        {
            return false;
        }

        protected virtual bool SceneLoadResources3()
        {
            if (this.LoadBundlesAndResources())
            {
                return true;
            }

            return false;
        }

        protected virtual bool ScenePreLoad()
        {
            this.loadLevelOperation = null;

            return false;
        }

        protected virtual bool SceneLoad()
        {
            if (this.loadLevelOperation == null)
            {
                this.loadLevelOperation = Application.LoadLevelAsync(this.LevelName);
                return true;
            }

            if (!this.loadLevelOperation.isDone)
            {
                return true;
            }

            return false;
        }

        protected virtual bool ScenePostLoad()
        {
            return false;
        }

        protected virtual bool ScenePreDestroy()
        {
            return false;
        }

        protected virtual bool SceneDestroy()
        {
            return false;
        }

        protected virtual bool ScenePostDestroy()
        {
            return false;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private bool LoadBundlesAndResources()
        {
            if (this.UseAsyncLoading)
            {
                if (BundleProvider.Instance.ContinueLoad())
                {
                    return true;
                }

                if (ResourceProvider.Instance.ContinueLoad())
                {
                    return true;
                }

                return false;
            }

            BundleProvider.Instance.LoadImmediate();
            ResourceProvider.Instance.LoadImmediate();
            return false;
        }
    }
}
