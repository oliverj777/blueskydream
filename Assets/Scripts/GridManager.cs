using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OllieJones
{
    // Create a grid of cards
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private Vector2Int gridSize = new Vector2Int(2, 3);
        [SerializeField] private List<GameObject> prefabCards = new List<GameObject>();
        [SerializeField] private RectTransform canvas;
        [SerializeField] private List<CardModule> runtimeStack = new List<CardModule>();

        private void Start()
        {
            BuildGrid();
        }

        [ContextMenu("Build Grid")]
        public void BuildGrid()
        {
            ClearGrid();

            for (int y = 0; y < gridSize.y; y++)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    int random = Random.Range(0, prefabCards.Count);
                    GameObject card = Instantiate(prefabCards[random], canvas.transform);
                    CardModule module = card.GetComponent<CardModule>();
                    module.Initiate(new Vector2Int(x,y));

                    card.transform.localPosition = new Vector3(x * 100, y * 100, 0);

                    runtimeStack.Add(module);
                }
            }
        }

        public void ClearGrid()
        {
            foreach(CardModule card in runtimeStack)
            {
                Destroy(card.gameObject);
            }

            runtimeStack.Clear();
        }

        private List<GameObject> GenerateCardPairsPrefab(Vector2Int gridSize)
        {
            List<GameObject> pairs = new List<GameObject>();

            //TODO, add logic that'll generate a list of suitbale paired cards, shuffle and return
            return pairs;
        }

        private void Shuffle<T>(List<T> list)
        {
            //TODO, helper function to shuffle list
        }
    }
}

