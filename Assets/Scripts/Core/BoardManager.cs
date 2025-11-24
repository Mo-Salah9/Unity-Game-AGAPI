using UnityEngine;
using System.Collections.Generic;

public class BoardManager
{
    public int[] GenerateCardIds(int rows, int columns, int spriteCount)
    {
        int totalCards = rows * columns;
        int pairCount = totalCards / 2;

        List<int> cardIds = new List<int>();

        for (int i = 0; i < pairCount; i++)
        {
            int id = i % spriteCount;
            cardIds.Add(id);
            cardIds.Add(id);
        }

        ShuffleList(cardIds);
        return cardIds.ToArray();
    }

    public List<Card> CreateBoard(Card prefab, Transform parent, RectTransform container,
        int rows, int columns, int[] cardIds, Sprite[] sprites, Sprite backSprite)
    {
        ClearBoard(parent);

        List<Card> cards = new List<Card>();
        float containerWidth = container.rect.width;
        float containerHeight = container.rect.height;

        float spacing = 10f;
        float cardWidth = (containerWidth - (spacing * (columns + 1))) / columns;
        float cardHeight = (containerHeight - (spacing * (rows + 1))) / rows;

        float cellSize = Mathf.Min(cardWidth, cardHeight);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                int index = i * columns + j;
                Card card = Object.Instantiate(prefab, parent);

                RectTransform rt = card.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(cellSize, cellSize);

                float xPos = j * (cellSize + spacing) + spacing + cellSize / 2 - containerWidth / 2;
                float yPos = containerHeight / 2 - (i * (cellSize + spacing) + spacing + cellSize / 2);
                rt.anchoredPosition = new Vector2(xPos, yPos);

                int cardId = cardIds[index];
                card.Initialize(cardId, sprites[cardId], backSprite);
                cards.Add(card);
            }
        }

        return cards;
    }

    private void ClearBoard(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Object.Destroy(parent.GetChild(i).gameObject);
        }
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}