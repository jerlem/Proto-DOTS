using UnityEngine;
using UnityEngine.SceneManagement;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace FPSTemplate
{
    public class GameManager : GameSingleton
    {
        public static DataManager DataManager;
        public static EventManager EventManager;

        public static PlayerController PlayerController;

        public bool CtrlPressed { get; private set; } = false;
        public bool AltPressed { get; private set; } = false; 

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

            GameManager.EventManager.OnKeyPress += KeyPressed;
            GameManager.EventManager.OnKeyReleased += KeyReleased;

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

        public void KeyPressed(KeyCode keyCode)
        {
            if (keyCode == KeyCode.LeftControl || keyCode == KeyCode.RightControl)
                CtrlPressed = true;

            if (keyCode == KeyCode.LeftAlt || keyCode == KeyCode.RightAlt)
                AltPressed = true;

            if (keyCode == KeyCode.Escape)
                ReturnToMainMenu();

            if (keyCode == KeyCode.Return && AltPressed == true)
            {
                Debug.Log("Switch full screen");
            }

        }

        public void KeyReleased(KeyCode keyCode)
        {
            if (keyCode == KeyCode.LeftControl || keyCode == KeyCode.RightControl)
                CtrlPressed = false;

            if (keyCode == KeyCode.LeftAlt || keyCode == KeyCode.RightAlt)
                AltPressed = false;
        }
        

        /// <summary>
        /// return to menu
        /// </summary>
        private void ReturnToMainMenu() => SceneManager.LoadScene("MainMenu");

        /// <summary>
        /// change fullscreen
        /// </summary>
        public void ToogleFullScreen() => Screen.fullScreen = !Screen.fullScreen;


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