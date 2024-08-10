using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSTemplate
{
    /// <summary>
    /// Base Class for Singleton 
    /// </summary>
    public class GameSingleton : MonoBehaviour, IDisposable
    {
        internal static string className;

        public static GameSingleton instance { get; private set; }

        public static GameSingleton Instance
        {
            get
            {
                if (instance == null)
                    Debug.LogError("[IRSSingleton] Instance Error - instance was null");

                return instance;
            }
        }

        /// <summary>
        /// Constructor just get Instance reference and ClassName
        /// </summary>
        public GameSingleton()
        {
            instance = this;
            className = GetType().Name;
        }

        /// <summary>
        /// Dispose : triggered by OnDestroy()
        /// </summary>
        public virtual void Dispose() { }

        private void OnDestroy() => Dispose();
    }
}
