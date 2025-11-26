using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Board Settings")]
    [SerializeField] private int rows = 4;
    [SerializeField] private int columns = 4;
    [SerializeField] private Card cardPrefab;
    [SerializeField] private Transform boardParent;
    [SerializeField] private RectTransform boardContainer;

    [Header("Card Settings")]
    [SerializeField] private Sprite[] cardSprites;
    [SerializeField] private Sprite cardBackSprite;

    private BoardManager boardManager;
    private MatchChecker matchChecker;
    private ScoreSystem scoreSystem;
    private SaveSystem saveSystem;
    private AudioManager audioManager;

    private List<Card> flippedCards = new List<Card>();
    private int totalPairs;
    private int matchedPairs = 0;
    private bool isProcessing = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeSystems();
    }

    private void Start()
    {
        if (saveSystem.HasSaveData())
        {
            LoadGame();
        }
        else
        {
            StartNewGame();
        }
    }

    private void InitializeSystems()
    {
        boardManager = new BoardManager();
        matchChecker = new MatchChecker();
        scoreSystem = GetComponent<ScoreSystem>();
        saveSystem = new SaveSystem();
        audioManager = GetComponent<AudioManager>();
    }

    public void StartNewGame()
    {
        matchedPairs = 0;
        totalPairs = (rows * columns) / 2;
        flippedCards.Clear();
        isProcessing = false;  

        scoreSystem.ResetScore();

        int[] cardIds = boardManager.GenerateCardIds(rows, columns, cardSprites.Length);
        List<Card> cards = boardManager.CreateBoard(cardPrefab, boardParent, boardContainer,
            rows, columns, cardIds, cardSprites, cardBackSprite);

        foreach (Card card in cards)
        {
            card.OnCardFlipped += HandleCardFlipped;
        }

        saveSystem.ClearSaveData();  
    }
    public void Reset()
    {
        SceneManager.LoadScene(0);
    }
    public void HandleCardFlipped(Card card)
    {
        if (isProcessing || card.IsMatched || flippedCards.Contains(card))
            return;

        card.Flip();
        flippedCards.Add(card);
        audioManager.PlayFlipSound();

        if (flippedCards.Count == 2)
        {
            CheckForMatch();
        }
    }

    private void CheckForMatch()
    {
        isProcessing = true;

        Card card1 = flippedCards[0];
        Card card2 = flippedCards[1];

        if (matchChecker.IsMatch(card1, card2))
        {
            HandleMatch(card1, card2);
        }
        else
        {
            HandleMismatch(card1, card2);
        }
    }

    private void HandleMatch(Card card1, Card card2)
    {
        card1.SetMatched();
        card2.SetMatched();

        matchedPairs++;
        scoreSystem.AddMatchScore();
        audioManager.PlayMatchSound();

        flippedCards.Clear();
        isProcessing = false;

        if (matchedPairs >= totalPairs)
        {
            Invoke(nameof(HandleGameOver), 0.5f);
        }
    }

    private void HandleMismatch(Card card1, Card card2)
    {
        scoreSystem.ResetCombo();
        audioManager.PlayMismatchSound();

        Invoke(nameof(FlipCardsBack), 1f);
    }

    private void FlipCardsBack()
    {
        foreach (Card card in flippedCards)
        {
            card.FlipBack();
        }
        flippedCards.Clear();
        isProcessing = false;
    }

    private void HandleGameOver()
    {
        audioManager.PlayGameOverSound();
        UIManager.Instance.ShowGameOver(scoreSystem.GetScore());
        saveSystem.ClearSaveData();
    }

    public void SaveGame()
    {
        Card[] allCards = boardParent.GetComponentsInChildren<Card>();
        saveSystem.SaveGame(allCards, scoreSystem.GetScore(), scoreSystem.GetCombo(),
            rows, columns, matchedPairs);
    }

    public void LoadGame()
    {
        GameSaveData data = saveSystem.LoadGame();

        rows = data.rows;
        columns = data.columns;
        matchedPairs = data.matchedPairs;
        totalPairs = (rows * columns) / 2;

        scoreSystem.SetScore(data.score);
        scoreSystem.SetCombo(data.combo);

        List<Card> cards = boardManager.CreateBoard(cardPrefab, boardParent, boardContainer,
            rows, columns, data.cardIds, cardSprites, cardBackSprite);

        flippedCards.Clear();

        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].OnCardFlipped += HandleCardFlipped;

            if (data.cardStates[i].isMatched)
            {
                cards[i].SetFlippedState(true);
                cards[i].SetMatched();
            }
            else if (data.cardStates[i].isFlipped)
            {
                cards[i].SetFlippedState(true);
                flippedCards.Add(cards[i]);
            }
        }

        if (flippedCards.Count == 2)
        {
            CheckForMatch();
        }
        else if (flippedCards.Count == 1)
        {
            isProcessing = false;
        }
    }

    private void OnApplicationQuit()
    {
        if (matchedPairs < totalPairs)
        {
            SaveGame();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause && matchedPairs < totalPairs)
        {
            SaveGame();
        }
    }
}