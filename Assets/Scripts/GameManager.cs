namespace FPSTemplate
{
    public class GameManager : GameSingleton
    {
        public static DataManager DataManager;
        public static EventManager EventManager;

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
        }

        /// <summary>
        /// Update main loop
        /// </summary>
        private void Update()
        {
            EventManager.FrameUpdate();
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
            EventManager.OnDataManagerLoaded -= DataManagerLoaded;
            base.Dispose();
        }
    }

}