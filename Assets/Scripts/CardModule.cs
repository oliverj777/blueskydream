using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OllieJones
{
    public class CardModule : MonoBehaviour
    {
        [SerializeField] private Vector2Int coordinate;

        public void Initiate(Vector2Int coordinate)
        {
            this.coordinate = coordinate;
        }
    }

}

