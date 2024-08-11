using UnityEngine;
using UnityEngine.SceneManagement;

namespace FPSTemplate
{
    public class GameManager : GameSingleton
    {
        public static DataManager DataManager;
        public static EventManager EventManager;

        public static PlayerController PlayerController;
        public static UIManager UIManager;

        public bool CtrlPressed { get; private set; } = false;
        public bool AltPressed { get; private set; } = false;

        // check for first key pressed
        bool hasMoved = false;

        public static void SetGameState(GameState state)
        {
            Debug.Log("Game state changed to : " + state);
            EventManager.GameState = state;

            if(state == GameState.GameOver)
            {
                GameManager.PlayerController.Active = false;
                GameManager.UIManager.ShowGameOver();
            }
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

            // UI Manager
            UIManager = new UIManager();
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

        public void DamageRod(int rodId, int damage)
        {
            UIManager.RodList[rodId].Hp -= damage;

            if (UIManager.RodsDetroyed())
                SetGameState(GameState.GameOver);
        }

        public static void SetRodHP(int index, float value)
        {
            if (index > 4)
                return;

            UIManager.RodList[index].Hp = (int)value * 100;

            if (UIManager.RodsDetroyed())
                SetGameState(GameState.GameOver);
        }

        public void KeyPressed(KeyCode keyCode)
        {
            if (!hasMoved)
            {
                hasMoved = true;
                GameManager.UIManager.HideObjective();
            }

            GameState state = GameManager.EventManager.GameState;

            if (keyCode == KeyCode.LeftControl || keyCode == KeyCode.RightControl)
                CtrlPressed = true;

            if (keyCode == KeyCode.LeftAlt || keyCode == KeyCode.RightAlt)
                AltPressed = true;

            if (keyCode == KeyCode.Escape)
                ReturnToMainMenu();

            // TEST
            //if (keyCode == KeyCode.F)
            //{
            //    DamageRod(0, 5);
            //    DamageRod(1, 5);
            //    DamageRod(2, 5);
            //    DamageRod(3, 5);
            //    DamageRod(4, 5);
            //}

            if (keyCode == KeyCode.Return && AltPressed == true)
            {
                Debug.Log("Switch full screen");
            }

            // game over : pressing enter leads to main menu
            if (state == GameState.GameOver)
            {
                if(keyCode == KeyCode.Return|| keyCode == KeyCode.Escape)
                    ReturnToMainMenu();
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
                EventManager.OnKeyPress -= KeyPressed;
                EventManager.OnKeyReleased -= KeyReleased;
            }
            else
            {
                Debug.LogWarning("EventManager is null. It might not have been initialized properly.");
            }
            base.Dispose();
        }
    }

}