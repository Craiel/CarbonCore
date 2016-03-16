namespace Assets.Scripts.Game
{
    using System;

    using Assets.Scripts.Data;

    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.Json;

    public static class GameSaveLoad
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void Save()
        {
            // Build the save object
            SaveData targetData = new SaveData { Version = Constants.Version };

            // Channel the save object through all the components to store the data
            foreach (GameComponent component in Components.Instance.GetComponents())
            {
                Diagnostic.Info("Saving data for Component {0}", component.GetType().Name);
                component.Save(targetData);
            }
            
            try
            {
                // Serialize and write into the preferences
                string serialized = JsonExtensions.SaveToData(targetData);
                UnityEngine.PlayerPrefs.SetString(Constants.SaveKey, serialized);
            }
            catch (Exception e)
            {
                Diagnostic.Error("Failed to save state: {0}", e);
            }
        }

        public static void Load()
        {
            // Get the data from the preferences
            string saveData = UnityEngine.PlayerPrefs.GetString(Constants.SaveKey);
            if (string.IsNullOrEmpty(saveData))
            {
                return;
            }

            try
            {
                // De-serialize
                SaveData data = JsonExtensions.LoadFromData<SaveData>(saveData);

                // Run all components through the load process
                foreach (GameComponent component in Components.Instance.GetComponents())
                {
                    Diagnostic.Info("Loading data for Component {0}", component.GetType().Name);
                    component.Load(data);
                }
            }
            catch (Exception e)
            {
                Diagnostic.Error("Failed to load save state: {0}", e);
            }
        }

        public static void Reset()
        {
            // Create a fresh data object
            SaveData data = new SaveData();

            // Run all components through the load process
            foreach (GameComponent component in Components.Instance.GetComponents())
            {
                Diagnostic.Info("Loading data for Component {0}", component.GetType().Name);
                component.Load(data);
            }
        }
    }
}
