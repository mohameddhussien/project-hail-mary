using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameOverPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DamageHandler _damageHandler;
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private Button _tryAgainButton;

    private bool _triggered;
    private string _sceneName;

    // -------------------------------------------------------------------------
    // Unity lifecycle
    // -------------------------------------------------------------------------

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
        if (_gameOverPanel != null)
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

    // -------------------------------------------------------------------------
    // Game-over trigger
    // -------------------------------------------------------------------------

    private void TriggerGameOver()
    {
        if (_triggered) return;
        _triggered = true;

        // Freeze game but keep UI animating.
        Time.timeScale = 0f;

        // Show cursor so the player can click Try Again.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (_gameOverPanel != null)
            _gameOverPanel.SetActive(true);
    }

    private void OnHealthChanged()
    {
        if (_triggered) return;
        if (_damageHandler == null || _damageHandler.MaxHealth <= 0) return;

        if (_damageHandler.Health <= 0)
            TriggerGameOver();
    }

    // -------------------------------------------------------------------------
    // Try Again
    // -------------------------------------------------------------------------

    private void TryAgain()
    {
        
        GameManager.Instance?.ResetState();

        // ── Reset own state ─────────────────────────────────────────────────
        _triggered = false;
        if (_gameOverPanel != null)
            _gameOverPanel.SetActive(false);

        // ── Restore engine state ────────────────────────────────────────────
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Confined; // matches GameManager.Start()
        //Cursor.visible = false;

        // ── Reload scene ─────────────────────────────────────────────────────
        // Everything in the Gameplay scene (UIManager, EnemyShipManager,
        // HealthBarPlayer, GameOverPanel itself) is destroyed and recreated
        // fresh. Singletons on Managers survive and already hold reset values.
        SceneManager.LoadScene(_sceneName);
    }
}