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
        public float CardFlipTime = 0.25f;

        [Header("Score Settings")]
        public int StartingScore = 5;
        public int ComboPointMatch = 10;
        public int ComboPointNoMatch = -1;

        [Header("Timer Settings")]
        public float GameRevealTime = 3; //Seconds it'll wait at a new game, until the cards are hidden
        public float GameResetTime = 2; //Seconds it'll wait until a new game will start (wait for animation)
        public int GameMaxTimer = 30; // Seconds to complete a game

        [Header("Game Settings")]
        public bool RestartGameWithSameOrder = false; //If true, the game will restart with the same order of cards as before.
        public bool ContinueComboScoreAcrossGames = true;
    }

}

