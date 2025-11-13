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
        public int currentTimer;
        public int magicCounter = 0;

        private bool injected = false;
        private bool gameStarted = false;
        private float fTimer = 0;

        // Caching, for restart
        private Vector2Int cGridSize;
        private List<GameObject> cCards;

        // Moved to configs
        //readonly int comboPointMatch = 10;
        //readonly int comboPointNoMatch = -1;

        // Public Events
        public UnityEvent<CardModule> OnEventCardSelected;
        public UnityEvent<CardModule, CardModule> OnEventCardsMatched;
        public UnityEvent<CardModule, CardModule> OnEventCardsNoMatch;
        public UnityEvent<int> OnEventMagicCollected; //magicCounter

        public UnityEvent<int, int, int, int> OnEventGameLoopUpdate; //current score, points, combo, timer

        public UnityEvent OnEventGameStart;
        public UnityEvent<GameReport> OnEventGameComplete;
        public UnityEvent<GameReport> OnEventGameFailed;

        public enum GameReport { Progress, Won, Lost_Score, Lost_Timer }

        private void Awake()
        {
            // Using singleton static instance for global access, but happy to work with dependency injection too.
            Instance = this;

            if (grid == null) Debug.LogError("GridManager is not populated");
            if (gameConfig == null) Debug.LogError("GameConfig file is not populated");
        }

        private void Start()
        {
            NewGame();
        }

        public ScriptableGameConfigs Config()
        {
            return gameConfig;
        }

        private void BuildGame()
        {
            injected = false;

            StopGame();
            List<GameObject> cards = grid.GenerateCardPairsPrefab(grid.GridSize());
            grid.BuildGrid(grid.GridSize(), cards);
            corRevealOpening = StartCoroutine(CoroutineRevealOpening());
        }

        public void InjectGame(Vector2Int gridSize, List<GameObject> cards, bool shuffle = false)
        {
            injected = true;

            // Cache
            cGridSize = gridSize;
            cCards = cards;

            StopGame();
            grid.BuildGrid(gridSize, cards, shuffle);
            corRevealOpening = StartCoroutine(CoroutineRevealOpening());
            OnEventGameLoopUpdate?.Invoke(currentScore, 0, comboCounter, currentTimer);
        }

        [ContextMenu("Force NewGame")]
        public void NewGame()
        {
            currentLevel++;
            //TODO, add dynamic difficulty system

            currentScore = Config().StartingScore;

            if (Config().ContinueComboScoreAcrossGames == false)
                comboCounter = 0;

            BuildGame();

            OnEventGameLoopUpdate?.Invoke(currentScore, 0, comboCounter, currentTimer);
        }

        private void StopGame()
        {
            gameStarted = false;

            if (corRevealOpening != null)
            {
                StopCoroutine(corRevealOpening);
                corRevealOpening = null;
            }
        }

        private void StartGame()
        {
            if (injected == false) fTimer = Config().GameMaxTimer;
            else fTimer = currentTimer;
            gameStarted = true;

            OnEventGameStart?.Invoke();
        }

        private void LostGame(GameReport reason)
        {
            gameStarted = false;
            Debug.Log(" ** GAMEOVER ** " + reason);

            OnEventGameFailed?.Invoke(reason);
        }

        public void RestartGame()
        {
            if(Config().RestartGameWithSameOrder)
                InjectGame(cGridSize, cCards);

            else
            {
                // We need to reshuffle the stack
                InjectGame(cGridSize, cCards, true);
            }

            // Reset some game values to prevent errors
            injected = false;
            fTimer = Config().GameMaxTimer;
            currentScore = Config().StartingScore;
            comboCounter = 0;

            OnEventGameLoopUpdate?.Invoke(currentScore, 0, comboCounter, currentTimer);
        }

        // Will reset the timer/score only, and continue the same game
        public void RecommenceGame(GameReport reason)
        {
            if(reason == GameReport.Lost_Timer)
                fTimer = Config().GameMaxTimer;

            if (reason == GameReport.Lost_Score)
                currentScore = Config().StartingScore;

            corRevealOpening = StartCoroutine(CoroutineRevealOpening());
        }


        private void Update()
        {
            if (gameStarted == false) return;

            fTimer -= Time.deltaTime;
            currentTimer = Mathf.RoundToInt(fTimer);

            CheckGameLoop();
        }


        // Called directly from the CardModule
        public void OnCardSelected(CardModule card)
        {
            if (gameStarted == false) return;

            Debug.Log("Card Selected: " + card.nameTag, card.transform);

            AudioManager.Instance.PlayCardFlip();
            card.FlipCard();

            selectedStack.Add(card);
            CheckGameState();
        }

        public void SpendMagic(int amt)
        {
            magicCounter -= amt;
            OnEventMagicCollected?.Invoke(magicCounter);
        }

        // ---------- Game Drivers ---------- //

        Coroutine corRevealOpening;
        IEnumerator CoroutineRevealOpening()
        {
            foreach (CardModule card in grid.RuntimeStack())
            {
                if (card.IsRevealed() == false)
                    card.FlipCard();
            }

            // Wait and flip cards
            yield return new WaitForSeconds(Config().GameRevealTime);

            foreach (CardModule card in grid.RuntimeStack())
            {
                if(card.IsRevealed())
                    card.FlipCard();
            }

            StartGame();
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

        private void CheckGameLoop()
        {
            if (fTimer <= 0)
            {
                LostGame(GameReport.Lost_Timer);
                gameStarted = false;
            }

            if (currentScore < 0)
            {
                LostGame(GameReport.Lost_Score);
                gameStarted = false;
            }
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
                OnEventGameLoopUpdate?.Invoke(currentScore, points, comboCounter, currentTimer);

                if(cardA.IsMagic() || cardB.IsMagic())
                {
                    magicCounter++;
                    OnEventMagicCollected?.Invoke(magicCounter);
                }

                // Check for game progress
                if (HasMatchedAll())
                {
                    Debug.Log("* WON *");

                    OnEventGameComplete?.Invoke(GameReport.Won);

                    // Let the UI view deal with this
                    //yield return new WaitForSeconds(Config().GameResetTime);
                    //NewGame();
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
                OnEventGameLoopUpdate?.Invoke(currentScore, points, comboCounter, currentTimer);
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
