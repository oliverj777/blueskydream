using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OllieJones
{
    // Controls game logic, game tracking, events and loop
    public class CardManager : MonoBehaviour
    {
        public static CardManager Instance;

        [SerializeField] private List<CardModule> selectedStack = new List<CardModule>();
        public GridManager grid;

        // Game Logic
        public int currentLevel = 0;
        private float revealWait = 3;
        public int comboCounter = 0;
        public int currentScore;

        readonly int comboPointMatch = 10;
        readonly int comboPointNoMatch = -1;

        // Public Events
        public UnityEvent<CardModule> OnEventCardSelected;
        public UnityEvent<CardModule, CardModule> OnEventCardsMatched;
        public UnityEvent<CardModule, CardModule> OnEventCardsNoMatch;
        public UnityEvent<int, int, int> OnEventGameLoopUpdate; //current score, points, combo
        public UnityEvent OnEventGameComplete;


        private void Awake()
        {
            // Using singleton static instance for global access, but happy to work with dependency injection too.
            Instance = this;
        }

        private void Start()
        {
            BuildGame();
        }

        private void BuildGame()
        {
            StopGame();
            List<GameObject> cards = grid.GenerateCardPairsPrefab(grid.GridSize());
            grid.BuildGrid(grid.GridSize(), cards);
            corRevealOpening = StartCoroutine(CoroutineRevealOpening());
        }

        public void InjectGame(Vector2Int gridSize, List<GameObject> cards)
        {
            StopGame();
            grid.BuildGrid(gridSize, cards);
            corRevealOpening = StartCoroutine(CoroutineRevealOpening());
        }

        private void NewGame()
        {
            //TODO, game loop, once a game has been completed
        }

        private void StopGame()
        {
            if(corRevealOpening != null)
            {
                StopCoroutine(corRevealOpening);
                corRevealOpening = null;
            }
        }

        // Called directly from the CardModule
        public void OnCardSelected(CardModule card)
        {
            Debug.Log("Card Selected: " + card.nameTag, card.transform);

            card.FlipCard();

            selectedStack.Add(card);
            CheckGameState();
        }

        // ---------- Game Drivers ---------- //

        Coroutine corRevealOpening;
        IEnumerator CoroutineRevealOpening()
        {
            // Wait and flip cards
            yield return new WaitForSeconds(revealWait);

            foreach (CardModule card in grid.RuntimeStack())
            {
                card.FlipCard();
            }
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

            // Handover to coroutine to deal with timed events/animations
            StartCoroutine(CoroutineCheckGameState(cardA, cardB));

            // Clear the selected for retry
            selectedStack.Clear();
        }

        IEnumerator CoroutineCheckGameState(CardModule cardA, CardModule cardB)
        {
            yield return new WaitForSeconds(0.25f);

            // Match
            if(cardA.nameTag == cardB.nameTag)
            {
                Debug.Log("Match!");

                cardA.MatchCard(cardB);
                cardB.MatchCard(cardA);

                // Increment combo
                comboCounter++;
                int points = comboPointMatch * (comboCounter);
                currentScore += points;

                OnEventCardsMatched?.Invoke(cardA, cardB);
                OnEventGameLoopUpdate?.Invoke(currentScore, points, comboCounter);

                // Check for game progress
                if (HasMatchedAll())
                {
                    Debug.Log("* WON *");

                    OnEventGameComplete?.Invoke();

                    yield return new WaitForSeconds(2);
                    NewGame();
                }
            }
            else
            {
                Debug.Log("No Match");

                cardA.FlipCard();
                cardB.FlipCard();

                // Reset combo
                comboCounter = 0;
                int points = comboPointNoMatch;
                currentScore += points;

                OnEventCardsNoMatch?.Invoke(cardA, cardB);
            }
        }

        bool HasMatchedAll()
        {
            foreach(CardModule card in grid.RuntimeStack())
            {
                if (card.IsMatched() == false) return false;
            }

            return true;
        }
    }

}
