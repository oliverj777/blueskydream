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
        [SerializeField] private GameObject prefabCardEmpty;

        [SerializeField] private RectTransform canvas;
        [SerializeField] private List<CardModule> runtimeStack = new List<CardModule>();

        public List<CardModule> RuntimeStack()
        {
            return runtimeStack;
        }

        private void Start()
        {
            // Let card manager deal with starting a grid build
            //List<GameObject> cards = GenerateCardPairsPrefab(gridSize);
            //BuildGrid(gridSize, cards);
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

            // Layout settings
            float canvasWidth = canvas.rect.width;
            float canvasHeight = canvas.rect.height;
            float padding = 40f; // total border space on each side

            /*
             * The cards should scale to fit the target display area, 
             * whether it's the screen or another container widget.
             */

            // Available space for cards (minus padding)
            float availableWidth = canvasWidth - padding;
            float availableHeight = canvasHeight - padding;

            // Add small gaps between cards as a fraction of card size
            float spacingRatio = 0.1f; // 10% of card size will be spacing

            // Calculate the max card size that fits both horizontally and vertically
            float cardWidth = availableWidth / (gridSize.x + (gridSize.x - 1) * spacingRatio);
            float cardHeight = availableHeight / (gridSize.y + (gridSize.y - 1) * spacingRatio);
            float cardSize = Mathf.Min(cardWidth, cardHeight);
            float spacing = cardSize * spacingRatio;

            // Position cards centrally within the canvas
            float totalWidth = gridSize.x * cardSize + (gridSize.x - 1) * spacing;
            float totalHeight = gridSize.y * cardSize + (gridSize.y - 1) * spacing;
            Vector2 offset = new Vector2(-totalWidth / 2f + cardSize / 2f, -totalHeight / 2f + cardSize / 2f);


            int i = 0;
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    CardModule card = Instantiate(cards[i], canvas.transform).GetComponent<CardModule>();
                    card.Initiate(new Vector2Int(x,y));

                    // Position, scale dynamically & centered
                    RectTransform rt = card.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(cardSize, cardSize);

                    float posX = x * (cardSize + spacing) + offset.x;
                    float posY = y * (cardSize + spacing) + offset.y;
                    rt.localPosition = new Vector3(posX, posY, 0);

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
        public List<GameObject> GenerateCardPairsPrefab(Vector2Int gridSize)
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

            // If we have an odd number of cells, add an empty tile
            if (totalCells % 2 != 0 && prefabCardEmpty != null)
            {
                pairs.Add(prefabCardEmpty);
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
            if (string.IsNullOrEmpty(nameTag)) return prefabCardEmpty;

            foreach (GameObject card in prefabCards)
            {
                if (card.GetComponent<CardModule>().nameTag == nameTag) return card;
            }

            Debug.LogError($"Unable to find card prefab with nameTag: {nameTag}");
            return prefabCardEmpty;
        }

        //Helper. Finds the runtime card based on its coordinates. Used for saveload
        public CardModule GetCardModule(int x, int y)
        {
            foreach (CardModule card in runtimeStack)
            {
                if (card.Coordinates().x == x && card.Coordinates().y == y) return card;
            }

            Debug.LogError($"Unable to find card module with coords: {x},{y}");
            return null;
        }
    }
}

