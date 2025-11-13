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

        private void Start()
        {
            scoreText.text = "0";
            pointsText.text = "";
            timerText.text = "";

            // Subscribe to the update event
            CardManager.Instance.OnEventGameLoopUpdate.AddListener(HandlerOnGameLoopUpdate);
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

        private void Update()
        {
            int timer = CardManager.Instance.currentTimer;
            timerText.text = timer.ToString();
        }
    }

}