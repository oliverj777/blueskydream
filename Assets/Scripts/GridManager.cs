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

        public List<CardModule> RuntimeStack()
        {
            return runtimeStack;
        }

        private void Start()
        {
            List<GameObject> cards = GenerateCardPairsPrefab(gridSize);
            BuildGrid(gridSize, cards);
        }

        public Vector2Int GridSize()
        {
            return gridSize;
        }

        // Builds the card grid by instantiating and positioning card prefabs in a defined X/Y layout.
        public void BuildGrid(Vector2Int gridSize, List<GameObject> cards)
        {
            this.gridSize = gridSize;

            ClearGrid();

            int i = 0;
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    CardModule card = Instantiate(cards[i], canvas.transform).GetComponent<CardModule>();
                    card.Initiate(new Vector2Int(x,y));

                    card.transform.localPosition = new Vector3(x * 100, y * 100, 0);

                    runtimeStack.Add(card);
                    i++;
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

        // Generates a valid random list of cards for a given grid size, and shuffles them
        // TODO accommodate for odd grid format
        private List<GameObject> GenerateCardPairsPrefab(Vector2Int gridSize)
        {
            List<GameObject> pairs = new List<GameObject>();

            int totalCells = gridSize.x * gridSize.y;
            int totalPairs = totalCells / 2;

            // Generate random pairs
            for (int i = 0; i < totalPairs; i++)
            {
                int random = Random.Range(0, prefabCards.Count);

                // Add each card twice (the pair)
                pairs.Add(prefabCards[random]);
                pairs.Add(prefabCards[random]);
            }

            Shuffle(pairs);

            return pairs;
        }

        //Helper. Shuffles a given list randomly
        private void Shuffle<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                T temp = list[i];
                int randomIndex = Random.Range(i, list.Count);
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }

        //Helper. Finds the prefab based on its nameTag. Used for saveload
        public GameObject GetCardPrefab(string nameTag)
        {
            foreach (GameObject card in prefabCards)
            {
                if (card.GetComponent<CardModule>().nameTag == nameTag) return card;
            }

            return null;
        }

        //Helper. Finds the runtime card based on its coordinates. Used for saveload
        public CardModule GetCardModule(int x, int y)
        {
            foreach (CardModule card in runtimeStack)
            {
                if (card.Coordinates().x == x && card.Coordinates().y == y) return card;
            }

            return null;
        }
    }
}

