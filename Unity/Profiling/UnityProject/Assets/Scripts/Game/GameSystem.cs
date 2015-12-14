namespace Assets.Scripts.Game
{
    using Assets.Scripts.Data;
    using Assets.Scripts.Enums;
    using Assets.Scripts.Game.Scenes;
    using Assets.Scripts.InputSystem;
    using Assets.Scripts.Systems;
    using Assets.Scripts.Systems.Contracts;
    using Assets.Scripts.Tests;
    using Assets.Scripts.UI;

    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.Diagnostics.Metrics;
    using CarbonCore.Utils.Unity.Data;
    using CarbonCore.Utils.Unity.Logic;
    using CarbonCore.Utils.Unity.Logic.Enums;
    using CarbonCore.Utils.Unity.Logic.Resource;

    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics.CodeAnalysis;

    using UnityEngine;

    public delegate void TransitionStartingDelegate(GameSceneType? current, GameSceneType target);
    public delegate void TransitionFinishedDelegate(GameSceneType current);

    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Reviewed. Suppression is OK here.")]
    public class GameSystem : UnitySingletonBehavior<GameSystem>
    {
        private static readonly IDictionary<GameSceneType, Type> SceneTypeMap = new Dictionary<GameSceneType, Type>
                                {
                                    { GameSceneType.Intro, typeof(SceneIntro) },
                                    { GameSceneType.MainMenu, typeof(SceneMainMenu) }
                                };

        private readonly IDictionary<GameSceneType, IGameScene> scenes;

        private bool transitioning;
        private GameSceneType transitionTarget;
        private SceneTransitionStep transitionStep;
        private MetricTime transitionTime;
        private object[] transitionData;

        private TestUI testUi;

        private IGameScene activeScene;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public GameSystem()
        {
            this.scenes = new Dictionary<GameSceneType, IGameScene>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public event TransitionStartingDelegate TransitionStarting;
        public event TransitionFinishedDelegate TransitionFinished;

        public GameSceneType? ActiveSceneType { get; private set; }

        public SceneGameData DefaultSceneGameData { get; private set; }

        public bool InTransition { get; private set; }

        public int Scenes
        {
            get
            {
                return this.scenes.Count;
            }
        }

        public void SetInit(GameInit init)
        {
            this.testUi = init.UI;

            this.DefaultSceneGameData = init.SceneGameData;
        }

        public override void Initialize()
        {
            base.Initialize();

            CarbonEngine.InstantiateAndInitialize();

            SceneController.InstantiateAndInitialize();
            this.RegisterInController(SceneController.Instance, SceneRootCategory.System, true);

            Diagnostic.Info("Starting GameSystem!");

            // Initialize all components
            InputHandler.InstantiateAndInitialize();
            Components.InstantiateAndInitialize();
            TestController.InstantiateAndInitialize();

            this.LoadPermanentResources();
        }

        public void Transition(GameSceneType type, params object[] data)
        {
            this.transitioning = true;
            this.transitionTarget = type;
            this.transitionStep = SceneTransitionStep.Initialize;
            this.transitionTime = Diagnostic.BeginTimeMeasure();
            this.transitionData = data;
            this.InTransition = true;

            if (this.ActiveSceneType == type)
            {
                Diagnostic.Warning("Transition target and active scene are the same, skipping!");
                return;
            }

            Diagnostic.Info("Transitioning to {0}", type);

            if (this.TransitionStarting != null)
            {
                this.TransitionStarting(this.ActiveSceneType, type);
            }

            this.LoadScene(type);
        }
        
        public void Update()
        {
            if (this.transitioning)
            {
                this.UpdateSceneTransition();
            }

            Components.Instance.Update();
        }
        
        public T GetScene<T>()
            where T : IGameScene
        {
            return (T)this.activeScene;
        }

#if UNITY_EDITOR
        public IDictionary<ResourceKey, long> GetHistory()
        {
            return ResourceProvider.Instance.GetHistory();
        }
#endif

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void UpdateSceneTransition()
        {
            // Check if we are still on the previous scene
            if (this.activeScene != null && this.activeScene.Type != this.transitionTarget)
            {
                // We need to destroy this scene
                if (this.activeScene.ContinueDestroy(this.transitionStep))
                {
                    // The scene is still processing the destroy step
                    return;
                }

                if (this.AdvanceDestroyTransition())
                {
                    // Transitioned to the next step
                    return;
                }

                // We are done destroying the scene
                this.DestroyScene(this.activeScene.Type);
                this.activeScene = null;
                this.ActiveSceneType = null;
                this.transitionStep = SceneTransitionStep.Initialize;

                // Destroy done, moving to load phase
            }

            // Check if the scene is not active yet
            if (this.activeScene == null)
            {
                this.LoadScene(this.transitionTarget);
                this.activeScene = this.scenes[this.transitionTarget];
                this.activeScene.SetData(this.transitionData);

                Diagnostic.Info("Activated target scene {0}", this.transitionTarget);
            }

            if (this.activeScene.ContinueLoad(this.transitionStep))
            {
                // Still processing load step
                return;
            }

            if (this.AdvanceLoadTransition())
            {
                return;
            }

            // We are done transitioning
            Diagnostic.TakeTimeMeasure(this.transitionTime);
            Diagnostic.Info("Transition to {0} completed in {1}ms", this.transitionTarget, Diagnostic.GetTimeInMS(this.transitionTime.Total));

            // Try to free up un-used assets after transition
            Resources.UnloadUnusedAssets();

            this.transitioning = false;
            this.transitionTime = null;
            this.ActiveSceneType = this.activeScene.Type;
            
            this.InTransition = false;

            if (this.TransitionFinished != null)
            {
                this.TransitionFinished(this.ActiveSceneType.Value);
            }
        }

        private bool AdvanceDestroyTransition()
        {
            switch (this.transitionStep)
            {
                case SceneTransitionStep.Initialize:
                    {
                        this.transitionStep = SceneTransitionStep.PreDestroy;
                        return true;
                    }

                case SceneTransitionStep.PreDestroy:
                    {
                        this.transitionStep = SceneTransitionStep.Destroy;
                        return true;
                    }

                case SceneTransitionStep.Destroy:
                    {
                        this.transitionStep = SceneTransitionStep.PostDestroy;
                        return true;
                    }

                case SceneTransitionStep.PostDestroy:
                    {
                        this.transitionStep = SceneTransitionStep.Finalize;
                        return true;
                    }

                case SceneTransitionStep.Finalize:
                    {
                        return false;
                    }

                default:
                    {
                        throw new InvalidOperationException("Invalid state: " + this.transitionStep);
                    }
            }
        }

        private bool AdvanceLoadTransition()
        {
            switch (this.transitionStep)
            {
                case SceneTransitionStep.Initialize:
                    {
                        this.transitionStep = SceneTransitionStep.PreLoad;
                        return true;
                    }

                case SceneTransitionStep.PreLoad:
                    {
                        this.transitionStep = SceneTransitionStep.LoadRegisterResources1;
                        return true;
                    }

                case SceneTransitionStep.LoadRegisterResources1:
                    {
                        this.transitionStep = SceneTransitionStep.LoadResources1;
                        return true;
                    }

                case SceneTransitionStep.LoadResources1:
                    {
                        this.transitionStep = SceneTransitionStep.LoadRegisterResources2;
                        return true;
                    }

                case SceneTransitionStep.LoadRegisterResources2:
                    {
                        this.transitionStep = SceneTransitionStep.LoadResources2;
                        return true;
                    }

                case SceneTransitionStep.LoadResources2:
                    {
                        this.transitionStep = SceneTransitionStep.LoadRegisterResources3;
                        return true;
                    }

                case SceneTransitionStep.LoadRegisterResources3:
                    {
                        this.transitionStep = SceneTransitionStep.LoadResources3;
                        return true;
                    }

                case SceneTransitionStep.LoadResources3:
                    {
                        this.transitionStep = SceneTransitionStep.Load;
                        return true;
                    }

                case SceneTransitionStep.Load:
                    {
                        this.transitionStep = SceneTransitionStep.PostLoad;
                        return true;
                    }

                case SceneTransitionStep.PostLoad:
                    {
                        this.transitionStep = SceneTransitionStep.Finalize;
                        return true;
                    }

                case SceneTransitionStep.Finalize:
                    {
                        return false;
                    }

                default:
                    {
                        throw new InvalidOperationException("Invalid state: " + this.transitionStep);
                    }
            }
        }
        
        private void LoadPermanentResources()
        {
            MetricTime measure = Diagnostic.BeginTimeMeasure();

            // Todo: Add resources we will always need

            ResourceProvider.Instance.LoadImmediate();

            Diagnostic.TakeTimeMeasure(measure);
            Diagnostic.Info("Loaded permanent resources in {0}ms", Diagnostic.GetTimeInMS(measure.Total));
        }
        
        private void DestroyScene(GameSceneType type)
        {
            if (!this.scenes.ContainsKey(type))
            {
                Diagnostic.Warning("Scene {0} is not loaded, skipping shutdown", type);
                return;
            }

            this.scenes.Remove(type);
        }

        private void LoadScene(GameSceneType type)
        {
            if (!this.scenes.ContainsKey(type))
            {
                IGameScene scene = this.InitializeScene(type);
                if (scene == null)
                {
                    return;
                }

                this.scenes.Add(type, scene);
            }
        }

        private IGameScene InitializeScene(GameSceneType type)
        {
            if (this.scenes.ContainsKey(type))
            {
                Diagnostic.Warning("Scene {0} is already loaded, skipping", type);
                return null;
            }

            if (!SceneTypeMap.ContainsKey(type))
            {
                Diagnostic.Error("Scene {0} has no implementation defined!", type);
                return null;
            }

            if (!typeof(IGameScene).IsAssignableFrom(SceneTypeMap[type]))
            {
                Diagnostic.Error("Scene implementation {0} is not of type IGameScene!", SceneTypeMap[type]);
                return null;
            }

            return (IGameScene)Activator.CreateInstance(SceneTypeMap[type]);
        }
    }
}
