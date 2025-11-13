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

        public void Initiate(Vector2Int coordinate)
        {
            this.coordinate = coordinate;
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            transform.SetAsLastSibling();

            CardManager.Instance.OnCardSelected(this);
        }
    }

}

