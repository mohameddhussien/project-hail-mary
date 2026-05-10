using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] TargetIndicator _targetIndicatorPrefab;
    [SerializeField] Canvas _mainCanvas;
    [SerializeField] TMP_Text _scoreText, _highScoreText;
    [SerializeField] GameObject _gameOverScreen;

    List<TargetIndicator> _targetIndicators;

    // -------------------------------------------------------------------------
    // Unity lifecycle
    // -------------------------------------------------------------------------

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _targetIndicators = new List<TargetIndicator>();

        // SceneManager.sceneLoaded fires after every scene load (including
        // reloads) once all Awake() calls in the new scene have completed.
        // That guarantees ScoreManager.Instance and GameManager.Instance are
        // already set, so the null guards in Subscribe* never bail out early.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnEnable()
    {
        // Hide the game-over screen whenever this object is re-enabled.
        if (_gameOverScreen != null)
            _gameOverScreen.SetActive(false);

        // Subscribe here for the very first scene (all singletons initialise
        // in the same Awake pass, so Instance references are valid by OnEnable).
        SubscribeToEvents();
    }

    void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // -------------------------------------------------------------------------
    // Scene reload
    // -------------------------------------------------------------------------

    /// <summary>
    /// Re-subscribes to all manager events after a scene reload.
    /// Runs after every Awake() in the new scene, so singleton Instance
    /// references are guaranteed to be populated.
    /// Also performs a direct read of the current score/high-score so the UI
    /// reflects the reset values immediately, without waiting for the next event.
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Unsubscribe first to avoid duplicate handlers from the previous scene.
        UnsubscribeFromEvents();
        SubscribeToEvents();
    }

    // -------------------------------------------------------------------------
    // Public API
    // -------------------------------------------------------------------------

    public void AddTarget(Transform target)
    {
        var targetIndicator = Instantiate(_targetIndicatorPrefab, _mainCanvas.transform);
        targetIndicator.Init(target, _mainCanvas);
        _targetIndicators.Add(targetIndicator);
    }

    public void RemoveTarget(Transform target)
    {
        var key = target.GetInstanceID();
        var indicator = _targetIndicators.FirstOrDefault(i => i.Key == key);
        if (indicator)
        {
            _targetIndicators.Remove(indicator);
            Destroy(indicator.gameObject);
        }
    }

    public void UpdateTargetIndicators(List<Transform> targets, int lockedOnTarget)
    {
        foreach (var targetIndicator in _targetIndicators)
        {
            targetIndicator.gameObject.SetActive(targets.Any(t => t.GetInstanceID() == targetIndicator.Key));
            targetIndicator.LockedOn = targetIndicator.Key == lockedOnTarget;
        }
    }

    // -------------------------------------------------------------------------
    // Event subscription helpers
    // -------------------------------------------------------------------------

    void SubscribeToEvents()
    {
        SubscribeToScoreManagerEvents();
        SubscribeToGameManagerEvents();
    }

    void UnsubscribeFromEvents()
    {
        UnsubscribeFromScoreManagerEvents();
        UnsubscribeFromGameManagerEvents();
    }

    void SubscribeToScoreManagerEvents()
    {
        if (!ScoreManager.Instance) return;

        // Always unsubscribe before subscribing to prevent duplicate handlers.
        UnsubscribeFromScoreManagerEvents();
        ScoreManager.Instance.ScoreChanged += OnScoreChanged;
        ScoreManager.Instance.HighScoreChanged += OnHighScoreChanged;

        // Direct read: ScoreManager persists via DontDestroyOnLoad and has
        // already reset its values by this point. Pulling the values now means
        // the score display is correct immediately, not just on the next event.
        OnScoreChanged(ScoreManager.Instance.Score);
        OnHighScoreChanged(ScoreManager.Instance.HighScore);
    }

    void UnsubscribeFromScoreManagerEvents()
    {
        if (!ScoreManager.Instance) return;
        ScoreManager.Instance.ScoreChanged -= OnScoreChanged;
        ScoreManager.Instance.HighScoreChanged -= OnHighScoreChanged;
    }

    void SubscribeToGameManagerEvents()
    {
        if (!GameManager.Instance) return;
        // Unsubscribe first to prevent duplicate handlers on reload.
        UnsubscribeFromGameManagerEvents();
        GameManager.Instance.GameStateChanged += OnGameStateChanged;
    }

    void UnsubscribeFromGameManagerEvents()
    {
        if (!GameManager.Instance) return;
        GameManager.Instance.GameStateChanged -= OnGameStateChanged;
    }

    // -------------------------------------------------------------------------
    // Event handlers
    // -------------------------------------------------------------------------

    void OnGameStateChanged(GameState state)
    {
        // Respond to every state so the screen is hidden when Patrol fires
        // (e.g. after TryAgain). The original code only showed it and relied
        // on OnEnable to hide it — but OnEnable never re-fires on a
        // DontDestroyOnLoad object, so the overlay stayed visible after reload.
        _gameOverScreen?.SetActive(state == GameState.GameOver);
    }

    void OnScoreChanged(int score)
    {
        _scoreText.text = score.ToString();
    }

    void OnHighScoreChanged(int highScore)
    {
        _highScoreText.text = highScore.ToString();
    }
}