using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OllieJones
{
    // Controls game logic, game tracking, events and loop
    public class CardManager : MonoBehaviour
    {
        public static CardManager Instance;

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
        }
    }

}
