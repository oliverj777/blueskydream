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
        [SerializeField] private ScriptableGameConfigs gameConfig;

        // Game Logic
        public int currentLevel = 0;
        //private float revealWait = 3; //TODO, make this dynamic based on level
        public int comboCounter = 0;
        public int currentScore;

        // Moved to configs
        //readonly int comboPointMatch = 10;
        //readonly int comboPointNoMatch = -1;

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

            if (grid == null) Debug.LogError("GridManager is not populated");
            if (gameConfig == null) Debug.LogError("GameConfig file is not populated");
        }

        private void Start()
        {
            BuildGame();
        }

        public ScriptableGameConfigs Config()
        {
            return gameConfig;
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
            OnEventGameLoopUpdate?.Invoke(currentScore, 0, comboCounter);
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

            AudioManager.Instance.PlayCardFlip();
            card.FlipCard();

            selectedStack.Add(card);
            CheckGameState();
        }

        // ---------- Game Drivers ---------- //

        Coroutine corRevealOpening;
        IEnumerator CoroutineRevealOpening()
        {
            // Wait and flip cards
            yield return new WaitForSeconds(Config().GameRevealTime);

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
            yield return new WaitForSeconds(Config().CardFlipTime);

            // Match
            if(cardA.nameTag == cardB.nameTag)
            {
                Debug.Log("Match!");

                AudioManager.Instance.PlayCardMatch();
                cardA.MatchCard(cardB);
                cardB.MatchCard(cardA);

                // Increment combo
                comboCounter++;
                int points = Config().ComboPointMatch * (comboCounter);
                currentScore += points;

                // Event triggers
                OnEventCardsMatched?.Invoke(cardA, cardB);
                OnEventGameLoopUpdate?.Invoke(currentScore, points, comboCounter);

                // Check for game progress
                if (HasMatchedAll())
                {
                    Debug.Log("* WON *");

                    OnEventGameComplete?.Invoke();

                    yield return new WaitForSeconds(Config().GameResetTime);
                    NewGame();
                }
            }
            else
            {
                yield return new WaitForSeconds(Config().CardFlipTime);

                Debug.Log("No Match");

                AudioManager.Instance.PlayCardFlop();
                cardA.FlipCard();
                cardB.FlipCard();

                // Reset combo
                comboCounter = 0;
                int points = Config().ComboPointNoMatch;
                currentScore += points;

                // Event triggers
                OnEventCardsNoMatch?.Invoke(cardA, cardB);
                OnEventGameLoopUpdate?.Invoke(currentScore, points, comboCounter);
            }
        }

        bool HasMatchedAll()
        {
            foreach(CardModule card in grid.RuntimeStack())
            {
                if (card.IsMatched() == false && card.IsEmpty() == false) return false;
            }

            return true;
        }
    }

}
