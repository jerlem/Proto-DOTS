using System;
using UnityEngine;

namespace FPSTemplate
{
    public class EventManager : BaseManager
    {
        public bool IsFocused => Application.isFocused;
        private bool isDataManagerLoaded = false;

        /// <summary>
        /// Mouse buttons
        /// </summary>
        /// <param name="button"></param>
        public delegate void MouseClick(int button);
        public delegate void MouseReleased(int button);
        public event MouseClick OnMouseClick;
        public event MouseReleased OnMouseReleased;

        /// <summary>
        /// Mouse scrolling
        /// </summary>
        /// <param name="y"></param>
        public delegate void MouseScroll(float y);
        public event MouseScroll OnMouseScroll;

        /// <summary>
        /// Keyboard events
        /// </summary>
        /// <param name="keyKode"></param>
        public delegate void KeyPressed(KeyCode keyKode);
        public delegate void KeyReleased(KeyCode keyKode);
        public event KeyPressed OnKeyPress;
        public event KeyReleased OnKeyReleased;

        /// <summary>
        /// DataManager loaded
        /// </summary>
        public delegate void DataManagerLoaded();
        public event DataManagerLoaded OnDataManagerLoaded;

        public void DataLoaded()
        {
            if (!isDataManagerLoaded)
            {
                isDataManagerLoaded = true;
                OnDataManagerLoaded?.Invoke();
            }
        }

        /*
         *  Game State
         */
        private GameState gameState;

        public GameState GameState
        {
            get => gameState;
            set
            {
                gameState = value;
                OnGameStateChanged?.Invoke(gameState);
            }
        }

        public delegate void GameStateChanged(GameState state);
        public event GameStateChanged OnGameStateChanged;


        /// <summary>
        /// Check inputs in frame update
        /// </summary>
        public void FrameUpdate()
        {

            if (!IsFocused)
                return;

            // left click
            if (Input.GetMouseButtonDown(0)) OnMouseClick?.Invoke(0);
            if (Input.GetMouseButtonUp(0)) OnMouseReleased?.Invoke(0);
            // right click
            if (Input.GetMouseButtonDown(1)) OnMouseClick?.Invoke(1);
            if (Input.GetMouseButtonUp(1)) OnMouseReleased?.Invoke(1);
            // whell click
            if (Input.GetMouseButtonDown(2)) OnMouseClick?.Invoke(2);
            if (Input.GetMouseButtonUp(2)) OnMouseReleased?.Invoke(2);

            // Mouse Scrolling
            if (Input.mouseScrollDelta.y != 0)
                OnMouseScroll?.Invoke(Input.mouseScrollDelta.y);

            // Keyboard input
            foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                    OnKeyPress?.Invoke(keyCode);

                if (Input.GetKeyUp(keyCode))
                    OnKeyReleased?.Invoke(keyCode);
            }
        }

        /// <summary>
        /// Init
        /// </summary>
        public override void Init()
        {
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public override void Dispose()
        {
        }
    }

}
