using UnityEngine;
using System;

public class ScoreSystem : MonoBehaviour
{
    public event Action<int> OnScoreChanged;
    public event Action<int> OnComboChanged;

    [SerializeField] private int baseMatchScore = 100;
    [SerializeField] private int comboMultiplier = 50;

    private int currentScore;
    private int currentCombo;

    public void ResetScore()
    {
        currentScore = 0;
        currentCombo = 0;
        OnScoreChanged?.Invoke(currentScore);
        OnComboChanged?.Invoke(currentCombo);
    }

    public void AddMatchScore()
    {
        currentCombo++;
        int comboBonus = (currentCombo - 1) * comboMultiplier;
        int scoreToAdd = baseMatchScore + comboBonus;

        currentScore += scoreToAdd;

        OnScoreChanged?.Invoke(currentScore);
        OnComboChanged?.Invoke(currentCombo);
    }

    public void ResetCombo()
    {
        currentCombo = 0;
        OnComboChanged?.Invoke(currentCombo);
    }

    public int GetScore()
    {
        return currentScore;
    }

    public int GetCombo()
    {
        return currentCombo;
    }

    public void SetScore(int score)
    {
        currentScore = score;
        OnScoreChanged?.Invoke(currentScore);
    }

    public void SetCombo(int combo)
    {
        currentCombo = combo;
        OnComboChanged?.Invoke(currentCombo);
    }
}