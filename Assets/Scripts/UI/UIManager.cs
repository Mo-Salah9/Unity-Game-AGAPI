using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private Button restartButton;

    private ScoreSystem scoreSystem;

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

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    private void Start()
    {
        scoreSystem = GameManager.Instance.GetComponent<ScoreSystem>();

        if (scoreSystem != null)
        {
            scoreSystem.OnScoreChanged += UpdateScoreDisplay;
            scoreSystem.OnComboChanged += UpdateComboDisplay;
        }

        UpdateScoreDisplay(0);
        UpdateComboDisplay(0);
    }

    private void UpdateScoreDisplay(int score)
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
    }

    private void UpdateComboDisplay(int combo)
    {
        if (comboText != null)
        {
            if (combo > 1)
                comboText.text = $"Combo: x{combo}";
            else
                comboText.text = "";
        }
    }

    public void ShowGameOver(int finalScore)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            if (finalScoreText != null)
                finalScoreText.text = $"Final Score: {finalScore}";
        }
    }

    private void RestartGame()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        GameManager.Instance.Reset();
    }

    private void OnDestroy()
    {
        if (scoreSystem != null)
        {
            scoreSystem.OnScoreChanged -= UpdateScoreDisplay;
            scoreSystem.OnComboChanged -= UpdateComboDisplay;
        }

        if (restartButton != null)
            restartButton.onClick.RemoveListener(RestartGame);
    }
}