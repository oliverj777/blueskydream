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

        public void Initiate(Vector2Int coordinate)
        {
            this.coordinate = coordinate;
            this.img = GetComponent<Image>();
            this.imgColor = img.color; // Cache the color, used when flipping
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            transform.SetAsLastSibling();

            CardManager.Instance.OnCardSelected(this);
        }

        // Called when the CheckGameState declares this card matched with another card
        public void MatchCard(CardModule otherCard)
        {
            matched = true;
        }
        public bool IsMatched()
        {
            return matched;
        }

        public Vector2Int Coordinates()
        {
            return coordinate;
        }

        public void FlipCard()
        {
            if (matched) return;
            hidden = !hidden;

            if (hidden) img.color = Color.gray;
            else img.color = imgColor;
        }
    }

}

