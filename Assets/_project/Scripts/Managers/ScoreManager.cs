using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public event Action<int> ScoreChanged = delegate { };
    public event Action<int> HighScoreChanged = delegate { };

    public int Score { get; private set; }
    public int HighScore { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    
    public void ResetScore()
    {
        Score = 0;
        ScoreChanged(Score);
        // HighScore intentionally NOT reset here — it persists as an all-time best.
        // If you want a full wipe (e.g. uninstall / new profile) add a separate ResetAll().
    }

   
    public void AddPoints(int points)
    {
        // Guard: never let a late event inflate the score after the round is over.
        if (GameManager.Instance != null &&
            GameManager.Instance.GameState == GameState.GameOver)
            return;

        Score += points;
        ScoreChanged(Score);

        if (Score <= HighScore) return;
        HighScore = Score;
        HighScoreChanged(HighScore);
    }
}