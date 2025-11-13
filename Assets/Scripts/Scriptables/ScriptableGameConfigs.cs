using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OllieJones
{
    [CreateAssetMenu(fileName = "Game Config", menuName = "OllieJones/Game Config", order = 1)]
    public class ScriptableGameConfigs : ScriptableObject
    {
        //TODO, use this to store game settings / configs as ScriptableObject, for easy refinement

        [Header("Card Settings")]
        //public float cardCooling = 0.25f;
        public float CardFlipTime = 0.25f;

        [Header("Score Settings")]
        public int ComboPointMatch = 10;
        public int ComboPointNoMatch = -1;

        [Header("Timer Settings")]
        public float GameRevealTime = 3;
        public float GameResetTime = 2;
    }

}

