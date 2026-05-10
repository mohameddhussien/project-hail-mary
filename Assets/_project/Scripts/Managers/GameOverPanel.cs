using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Attach to an ALWAYS-ACTIVE GameObject (e.g. the Canvas).
/// NEVER attach to the GameOverPanel child itself.
///
/// Inspector:
///   _damageHandler  → player's DamageHandler
///   _gameOverPanel  → GameOverPanel child (Text + Button)
///   _tryAgainButton → the Button inside GameOverPanel
/// </summary>
public class GameOverPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DamageHandler _damageHandler;
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private Button _tryAgainButton;

    private bool _triggered = false;
    private string _sceneName;

    private void Awake()
    {
        _sceneName = SceneManager.GetActiveScene().name;

        if (_tryAgainButton != null)
        {
            _tryAgainButton.onClick.RemoveAllListeners();
            _tryAgainButton.onClick.AddListener(TryAgain);
        }
        else
        {
            Debug.LogError("[GameOverPanel] _tryAgainButton not assigned!", this);
        }

        if (_damageHandler == null)
        {
            Debug.LogError("[GameOverPanel] DamageHandler not assigned!", this);
            return;
        }

        _damageHandler.ObjectDestroyed.AddListener(TriggerGameOver);
        _damageHandler.HealthChanged.AddListener(OnHealthChanged);
    }

    private void Start()
    {
        _triggered = false;
        _gameOverPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (_tryAgainButton != null)
            _tryAgainButton.onClick.RemoveListener(TryAgain);

        if (_damageHandler == null) return;
        _damageHandler.ObjectDestroyed.RemoveListener(TriggerGameOver);
        _damageHandler.HealthChanged.RemoveListener(OnHealthChanged);
    }

    private void TriggerGameOver()
    {
        if (_triggered) return;
        _triggered = true;

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        _gameOverPanel.SetActive(true);
    }

    private void OnHealthChanged()
    {
        if (_triggered) return;
        if (_damageHandler == null) return;
        if (_damageHandler.MaxHealth <= 0) return;

        if (_damageHandler.Health <= 0)
            TriggerGameOver();
    }

    private void TryAgain()
    {
        // ── Reset all DontDestroyOnLoad singletons ───────────────────
        // Managers.cs keeps GameManager + ScoreManager alive across reloads.
        // The fresh UIManager/HealthBarPlayer that spawn after LoadScene will
        // read from these singletons immediately, so they must be clean first.
        ScoreManager.Instance?.ResetScore();   // score → 0, fires ScoreChanged(0)
        GameManager.Instance?.ResetState();    // state → Patrol, fires GameStateChanged(Patrol)

        // ── Reset own state ──────────────────────────────────────────
        _triggered = false;
        _gameOverPanel.SetActive(false);

        // ── Restore engine state ─────────────────────────────────────
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Confined; // matches GameManager.Start()
        Cursor.visible = false;

        SceneManager.LoadScene(_sceneName);
    }
}