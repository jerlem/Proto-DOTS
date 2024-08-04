using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FPSTemplate
{
    public class DataManager : BaseManager
    {
        /*
         *  Scriptable Data
         */
        public const string PathSO = "SO/";

        private List<ScriptableObject> SOData = new();

        /// <summary>
        /// Data Manager Initialization
        /// - Get Illustrations sprites
        /// - Get Scriptable Objects
        /// </summary>
        public void LoadData()
        {

            // SO
            SOData = LoadScriptable(PathSO);
            if (SOData == null)
            {
                Debug.LogWarning("DataManager.LoadData() Warning : coudn't find ScriptableObject data");
            }
            else
            {
                //CardsAnimations = SOData.Find(x => x.name == ScriptableCardAnimationName) as ScriptableAnimationData;
            }

            GameManager.EventManager.DataLoaded();
        }

        /// <summary>
        /// Load Scriptables
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private List<ScriptableObject> LoadScriptable(string path) => Resources.LoadAll<ScriptableObject>(path).ToList();

        /// <summary>
        /// Dispose
        /// </summary>
        public override void Dispose()
        {
        }

        /// <summary>
        /// Init
        /// </summary>
        public override void Init()
        {
        }
    }

}
