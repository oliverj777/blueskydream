using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace OllieJones
{
    // Holds data and functions for the card prefab object
    public class CardModule : MonoBehaviour, IPointerClickHandler
    {
        public string nameTag; // Use this to determine match (unique ID)
        [SerializeField] private Vector2Int coordinate;

        [SerializeField] private bool hidden = false;
        [SerializeField] private bool matched = false;
        private Image img;
        private Color imgColor;
        private Image graphic;

        public void Initiate(Vector2Int coordinate)
        {
            this.coordinate = coordinate;
            this.img = GetComponent<Image>();
            this.imgColor = img.color; // Cache the color, used when flipping
            this.graphic = transform.GetChild(0).GetComponent<Image>();
            this.graphic.raycastTarget = false;
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            if (IsEmpty()) return;
            if (IsMatched()) return;
            if (hidden == false) return;

            transform.SetAsLastSibling();
            CardManager.Instance.OnCardSelected(this);
        }

        public bool IsMatched()
        {
            return matched;
        }

        public bool IsEmpty()
        {
            if (string.IsNullOrEmpty(nameTag)) return true;
            return false;
        }

        public Vector2Int Coordinates()
        {
            return coordinate;
        }

        public void FlipCard()
        {
            if (IsEmpty()) return;
            if (IsMatched()) return;
            hidden = !hidden;

            StartCoroutine(CoroutineFlipCard());
        }

        // Called when the CheckGameState declares this card matched with another card
        public void MatchCard(CardModule otherCard)
        {
            if (matched) return;
            matched = true;

            if(otherCard != null) // Could be null if directly injected from saveload
                StartCoroutine(CoroutineMatchCard(otherCard));
            else
            {
                graphic.gameObject.SetActive(false);
            }
        }



        // --------- Coroutines --------- //
        // This uses iTween, a lightweight helper script used for creating lerp animations

        IEnumerator CoroutineFlipCard()
        {
            float flipTime = 0.25f;

            iTween.ValueTo(gameObject, iTween.Hash(
                "from", 1f,
                "to", 0f,
                "time", (flipTime / 2f),
                "onupdate", "TweenOnUpdateFlip"
            ));

            yield return new WaitForSeconds((flipTime / 2f));

            if (hidden) img.color = Color.gray;
            else img.color = imgColor;

            iTween.ValueTo(gameObject, iTween.Hash(
               "from", 0f,
               "to", 1f,
               "time", (flipTime / 2f),
               "onupdate", "TweenOnUpdateFlip"
           ));
        }

        void TweenOnUpdateFlip(float v)
        {
            transform.localScale = new Vector3(v, 1, 1);
        }

        IEnumerator CoroutineMatchCard(CardModule otherCard)
        {
            Vector3 myPos = graphic.transform.position;
            Vector3 otherPos = otherCard.graphic.transform.position;
            Vector3 midpoint = (myPos + otherPos) / 2f;

            iTween.MoveTo(graphic.gameObject, iTween.Hash(
                "position", midpoint,
                "time", 0.5f,
                "islocal", false,
                "easetype", iTween.EaseType.easeInSine
            ));

            yield return new WaitForSeconds(0.25f);

            iTween.ScaleTo(graphic.gameObject, iTween.Hash(
                "scale", Vector3.one * 5,
                "time", 0.5f,
                "easetype", iTween.EaseType.easeInSine
            ));

            iTween.ValueTo(gameObject, iTween.Hash(
                "from", 1f,
                "to", 0f,
                "time", 0.5f,
                "onupdate", "TweenOnUpdateAlpha",
                "easetype", iTween.EaseType.easeInSine
            ));

            yield return new WaitForSeconds(1);

            graphic.gameObject.SetActive(false);
        }

        void TweenOnUpdateAlpha(float v)
        {
            graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, v);
        }
    }

}

