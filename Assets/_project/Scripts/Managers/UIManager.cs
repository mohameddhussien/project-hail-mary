using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private TargetIndicator _targetIndicatorPrefab;
    [SerializeField] private Canvas _mainCanvas;
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _highScoreText;

    private readonly List<TargetIndicator> _targetIndicators = new();

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
    }

    void Start()
    {
        
        SubscribeToEvents();
        SyncScoreDisplay();
    }

    void OnDestroy()
    {
        UnsubscribeFromEvents();

        if (Instance == this)
            Instance = null;
    }

    // -------------------------------------------------------------------------
    // Public API — called by Targetable and RadarScreen
    // -------------------------------------------------------------------------

    public void AddTarget(Transform target)
    {
        if (_targetIndicatorPrefab == null || _mainCanvas == null) return;

        var indicator = Instantiate(_targetIndicatorPrefab, _mainCanvas.transform);
        indicator.Init(target, _mainCanvas);
        _targetIndicators.Add(indicator);
    }

    public void RemoveTarget(Transform target)
    {
        int key = target.GetInstanceID();
        var indicator = _targetIndicators.FirstOrDefault(i => i.Key == key);
        if (indicator == null) return;

        _targetIndicators.Remove(indicator);
        Destroy(indicator.gameObject);
    }

    public void UpdateTargetIndicators(List<Transform> targets, int lockedOnTarget)
    {
        foreach (var indicator in _targetIndicators)
        {
            indicator.gameObject.SetActive(
                targets.Any(t => t.GetInstanceID() == indicator.Key));

            indicator.LockedOn = indicator.Key == lockedOnTarget;
        }
    }

    // -------------------------------------------------------------------------
    // Event wiring
    // -------------------------------------------------------------------------

    void SubscribeToEvents()
    {
        if (ScoreManager.Instance == null)
        {
            Debug.LogWarning("[UIManager] ScoreManager.Instance is null on Subscribe.", this);
            return;
        }

        ScoreManager.Instance.ScoreChanged += OnScoreChanged;
        ScoreManager.Instance.HighScoreChanged += OnHighScoreChanged;
    }

    void UnsubscribeFromEvents()
    {
        if (ScoreManager.Instance == null) return;

        ScoreManager.Instance.ScoreChanged -= OnScoreChanged;
        ScoreManager.Instance.HighScoreChanged -= OnHighScoreChanged;
    }

    
    void SyncScoreDisplay()
    {
        if (ScoreManager.Instance == null) return;

        OnScoreChanged(ScoreManager.Instance.Score);
        OnHighScoreChanged(ScoreManager.Instance.HighScore);
    }

    // -------------------------------------------------------------------------
    // Event handlers
    // -------------------------------------------------------------------------

    void OnScoreChanged(int score)
    {
        if (_scoreText != null)
            _scoreText.text = score.ToString();
    }

    void OnHighScoreChanged(int highScore)
    {
        if (_highScoreText != null)
            _highScoreText.text = highScore.ToString();
    }
}