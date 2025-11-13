using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Note: Using JSON as the save/load format. SimpleJSON is a helper tool when wokring with JSON.
using SimpleJSON;

namespace OllieJones
{
    public class SaveLoadManager : MonoBehaviour
    {
        [ContextMenu("SaveGameState")]
        public void SaveGameState()
        {
            // Ref managers
            CardManager manager = CardManager.Instance;
            GridManager grid = manager.grid;

            //Go through each card and get its state
            List<CardModule> stack = grid.RuntimeStack();

            //Serialise the game/cards into JSON children
            JSONObject json = new JSONObject();
            json["size_x"] = grid.GridSize().x;
            json["size_y"] = grid.GridSize().y;

            JSONArray s = new JSONArray();
            json["stack"] = s;

            foreach (CardModule card in stack)
            {
                JSONObject c = new JSONObject();
                c["tag"] = card.nameTag;
                c["pos_x"] = card.Coordinates().x;
                c["pos_y"] = card.Coordinates().y;
                c["matched"] = card.IsMatched();
                s.Add(c);
            }

            string j = json.ToString();
            Debug.Log("SaveData: " + j);
            PlayerPrefs.SetString("SAVED_DATA", j);
        }

        [ContextMenu("LoadGameState")]
        public void LoadGameState()
        {
            //TODO, check if saved data exists first

            // Ref managers
            CardManager manager = CardManager.Instance;
            GridManager grid = manager.grid;

            string j = PlayerPrefs.GetString("SAVED_DATA");
            JSONNode json = JSON.Parse(j);
            Debug.Log("LoadData: " + j);

            Vector2Int gridSize = new Vector2Int(json["size_x"].AsInt, json["size_y"].AsInt);

            // Get the card prefabs, and order used in the data
            List<GameObject> cards = new List<GameObject>();

            foreach (JSONObject c in json["stack"].AsArray)
            {
                string nameTag = c["tag"].Value;
                cards.Add(grid.GetCardPrefab(nameTag));
            }

            manager.InjectGame(gridSize, cards);

            // Inject the match state of each card
            foreach (JSONObject c in json["stack"].AsArray)
            {
                bool matched = c["matched"].AsBool;
                int x = c["pos_x"].AsInt;
                int y = c["pos_y"].AsInt;

                //TODO, maybe saveload the matched 'other' card? Otherwise, pass in null
                if (matched)
                    grid.GetCardModule(x, y).MatchCard(null);
            }
        }
    }

}