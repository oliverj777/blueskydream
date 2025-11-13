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

        [Header("Lost Panel")]
        [SerializeField] private CanvasRenderer lostPanel;
        [SerializeField] private TMP_Text lostText;
        [SerializeField] private Button lostBtn;

        [Header("Won Panel")]
        [SerializeField] private CanvasRenderer wonPanel;
        [SerializeField] private TMP_Text wonText;

        private void Awake()
        {
            lostPanel.gameObject.SetActive(false);
            wonPanel.gameObject.SetActive(false);

            lostBtn.onClick.AddListener(HandlerButtonTryAgain);
        }

        private void Start()
        {
            scoreText.text = "0";
            pointsText.text = "";
            timerText.text = "";

            // Subscribe to the game events
            CardManager.Instance.OnEventGameLoopUpdate.AddListener(HandlerOnGameLoopUpdate);
            CardManager.Instance.OnEventGameStart.AddListener(HandlerOnGameStart);
            CardManager.Instance.OnEventGameComplete.AddListener(HandlerOnGameComplete);
            CardManager.Instance.OnEventGameFailed.AddListener(HandlerOnGameFailed);
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

        private void HandlerOnGameStart()
        {

        }

        private void HandlerOnGameComplete(CardManager.GameReport reason)
        {

        }

        private void HandlerOnGameFailed(CardManager.GameReport reason)
        {
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

        
    }

}