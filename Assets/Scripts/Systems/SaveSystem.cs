using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CardState
{
    public bool isFlipped;
    public bool isMatched;
}

[System.Serializable]
public class GameSaveData
{
    public int[] cardIds;
    public CardState[] cardStates;
    public int score;
    public int combo;
    public int rows;
    public int columns;
    public int matchedPairs;
}

public class SaveSystem
{
    private const string SAVE_KEY = "CardGameSave";

    public void SaveGame(Card[] cards, int score, int combo, int rows, int columns, int matchedPairs)
    {
        GameSaveData data = new GameSaveData
        {
            cardIds = new int[cards.Length],
            cardStates = new CardState[cards.Length],
            score = score,
            combo = combo,
            rows = rows,
            columns = columns,
            matchedPairs = matchedPairs
        };

        for (int i = 0; i < cards.Length; i++)
        {
            data.cardIds[i] = cards[i].CardId;
            data.cardStates[i] = new CardState
            {
                isFlipped = cards[i].IsFlipped,
                isMatched = cards[i].IsMatched
            };
        }

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    public GameSaveData LoadGame()
    {
        string json = PlayerPrefs.GetString(SAVE_KEY, "");

        if (string.IsNullOrEmpty(json))
            return null;

        return JsonUtility.FromJson<GameSaveData>(json);
    }

    public bool HasSaveData()
    {
        return PlayerPrefs.HasKey(SAVE_KEY);
    }

    public void ClearSaveData()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.Save();
    }
}