using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

namespace OllieJones
{
    public class UiGameView : MonoBehaviour
    {
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text pointsText;
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text magicText;

        [Header("Lost Panel")]
        [SerializeField] private CanvasRenderer lostPanel;
        [SerializeField] private TMP_Text lostText;
        [SerializeField] private Button lostBtn;
        [SerializeField] private Button lostContinueBtn;

        [Header("Won Panel")]
        [SerializeField] private CanvasRenderer wonPanel;
        [SerializeField] private TMP_Text wonText;
        [SerializeField] private Button wonBtn;

        private void Awake()
        {
            lostPanel.gameObject.SetActive(false);
            wonPanel.gameObject.SetActive(false);

            lostBtn.onClick.AddListener(HandlerButtonTryAgain);
            lostContinueBtn.onClick.AddListener(HandlerButtonMagicContinue);

            wonBtn.onClick.AddListener(HandlerButtonNextGame);
        }

        private void Start()
        {
            scoreText.text = "0";
            pointsText.text = "";
            timerText.text = "";
            magicText.text = "0";

            // Subscribe to the game events
            CardManager.Instance.OnEventGameLoopUpdate.AddListener(HandlerOnGameLoopUpdate);
            CardManager.Instance.OnEventGameStart.AddListener(HandlerOnGameStart);
            CardManager.Instance.OnEventGameComplete.AddListener(HandlerOnGameComplete);
            CardManager.Instance.OnEventGameFailed.AddListener(HandlerOnGameFailed);

            CardManager.Instance.OnEventMagicCollected.AddListener(HandlerOnMagicCollected);
        }

        private void Update()
        {
            int timer = CardManager.Instance.currentTimer;
            timerText.text = timer.ToString();
        }

        private void HandlerOnGameLoopUpdate(int score, int points, int combo, int currentTimer)
        {
            scoreText.text = score.ToString();

            if (points > 0)
            {
                pointsText.text = "+" + points.ToString();
                pointsText.color = Color.gray;
            }

            else if (points < 0)
            {
                pointsText.text = points.ToString();
                pointsText.color = Color.red;
            }
            else
            {
                pointsText.text = "";
            }
        }

        private void HandlerOnMagicCollected(int magic)
        {
            magicText.text = magic.ToString();
        }


        private void HandlerOnGameStart()
        {

        }

        private void HandlerOnGameComplete(CardManager.GameReport reason)
        {
            string msg = "Nicely done!";

            wonText.text = msg;
            wonPanel.gameObject.SetActive(true);
        }

        CardManager.GameReport cReason;
        private void HandlerOnGameFailed(CardManager.GameReport reason)
        {
            cReason = reason;
            string msg = "On no!\n";

            if(reason == CardManager.GameReport.Lost_Score)
            {
                msg += "Bad score";
            }

            if(reason == CardManager.GameReport.Lost_Timer)
            {
                msg += "You ran out of time";
            }

            lostText.text = msg;
            lostPanel.gameObject.SetActive(true);
        }


        private void HandlerButtonTryAgain()
        {
            CardManager.Instance.RestartGame();

            lostPanel.gameObject.SetActive(false);
            wonPanel.gameObject.SetActive(false);

        }

        private void HandlerButtonMagicContinue()
        {
            CardManager.Instance.SpendMagic(1);
            CardManager.Instance.RecommenceGame(cReason);

            lostPanel.gameObject.SetActive(false);
            wonPanel.gameObject.SetActive(false);
        }

        private void HandlerButtonNextGame()
        {
            CardManager.Instance.NewGame();

            lostPanel.gameObject.SetActive(false);
            wonPanel.gameObject.SetActive(false);

        }


    }

}