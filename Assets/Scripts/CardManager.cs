using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OllieJones
{
    // Controls game logic, game tracking, events and loop
    public class CardManager : MonoBehaviour
    {
        public static CardManager Instance;

        [SerializeField] private List<CardModule> selectedStack = new List<CardModule>();

        private void Awake()
        {
            // Using singleton static instance for global access, but happy to work with dependency injection too.
            Instance = this;
        }


        private void BuildGame()
        {
            //TODO, generate a valid list of cards and insert into the GridManager
        }

        private void InjectGame()
        {
            //TODO, used for save/load data
        }

        private void NewGame()
        {
            //TODO, game loop, once a game has been completed
        }

        private void StopGame()
        {
            //TODO, clears any coroutines and game data
        }

        // Called directly from the CardModule
        public void OnCardSelected(CardModule card)
        {
            Debug.Log("Card Selected: " + card.nameTag, card.transform);

            selectedStack.Add(card);
            CheckGameState();
        }

        /* NOTE ---
         * The system should allow continuous card flipping without requiring users 
         * to wait for card comparisons to finish before selecting additional cards.
         */
        private void CheckGameState()
        {
            if (selectedStack.Count < 2) return; // Ignore. Only check if we have 2 selected in the stack
            if (selectedStack.Count > 2) return; // Error. Should never get to here, but catch just incase

            CardModule cardA = selectedStack[0];
            CardModule cardB = selectedStack[1];

            if (cardA.nameTag == cardB.nameTag)
            {
                Debug.Log("Its a Match!");
            }
            else
            {
                Debug.Log("No Match");
            }

            // Clear the selected for retry
            selectedStack.Clear();
        }
    }

}
