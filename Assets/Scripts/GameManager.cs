using UnityEngine;

namespace FPSTemplate
{
    public class GameManager : GameSingleton
    {
        public static DataManager DataManager;
        public static EventManager EventManager;

        public static PlayerController PlayerController;

        public void SetGameState(GameState state)
        {
            EventManager.GameState = state;
        }

        /// <summary>
        /// Start The managers
        /// subscribe events
        /// </summary>
        public void Awake()
        {
            // Event Manager
            EventManager = new EventManager();
            EventManager.OnDataManagerLoaded += DataManagerLoaded;

            // Data Manager
            DataManager = new DataManager();
            DataManager.LoadData();

            PlayerController = new PlayerController();
        }

        /// <summary>
        /// Update main loop
        /// </summary>
        private void Update()
        {
            EventManager.FrameUpdate();

            PlayerController.FrameUpdate();
        }

        /// <summary>
        /// Data loaded event
        /// </summary>
        public void DataManagerLoaded()
        {
        }

        /// <summary>
        /// Unsuscribe events then dispose
        /// </summary>
        public override void Dispose()
        {
            if (EventManager != null)
            {
                EventManager.OnDataManagerLoaded -= DataManagerLoaded;
            }
            else
            {
                Debug.LogWarning("EventManager is null. It might not have been initialized properly.");
            }
            base.Dispose();
        }
    }

}