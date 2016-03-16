namespace Assets.Scripts.Game
{
    using System.Collections.Generic;
    
    using CarbonCore.Utils.Unity.Logic;
    
    public class Components : UnitySingleton<Components>
    {
        private readonly IList<GameComponent> dynamicComponents;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Components()
        {
            this.dynamicComponents = new List<GameComponent>();

            // Create the static components
            //this.Audio = new AudioSystem();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        //public AudioSystem Audio { get; private set; }
        
        public override void Initialize()
        {
            base.Initialize();

            //this.Audio.Initialize();
        }

        public bool ContinueLoad()
        {
            /*if (this.Audio.ContinueLoad())
            {
                return true;
            }*/
            
            return false;
        }

        public void Update()
        {
            if (!this.IsInitialized)
            {
                return;
            }

            //this.Audio.Update();

            foreach (GameComponent component in this.dynamicComponents)
            {
                component.Update();
            }
        }

        public IList<GameComponent> GetComponents()
        {
            IList<GameComponent> result = new List<GameComponent>(this.dynamicComponents);

            //result.Add(this.Audio);

            return result;
        }

        public T GetComponent<T>()
            where T : GameComponent
        {
            // TODO: this is slow and needs refactoring
            foreach (GameComponent component in this.GetComponents())
            {
                if (component.GetType() == typeof(T))
                {
                    return component as T;
                }
            }

            return default(T);
        }

        public void RegisterComponent(GameComponent component)
        {
            this.dynamicComponents.Add(component);
        }

        public void UnregisterComponent(GameComponent component)
        {
            this.dynamicComponents.Remove(component);
        }
    }
}
