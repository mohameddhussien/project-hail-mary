using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class HealthBarPlayer : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Smoothing")]
    [SerializeField] private float _smoothSpeed = 5f;

    [Header("Dependencies")]
    // Populated by Inspector for the first scene; re-acquired via tag on every
    // subsequent scene load, so a stale Unity-null ref after reload is safe.
    [SerializeField] private DamageHandler _damageHandler;

    [Tooltip("Tag used to locate the player GameObject when re-acquiring the " +
             "DamageHandler after a scene reload.")]
    [SerializeField] private string _playerTag = "Player";

    private float _targetValue = 1f;

    // -------------------------------------------------------------------------
    // Unity lifecycle
    // -------------------------------------------------------------------------
    
    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        WireHandler(_damageHandler);
    }

    private IEnumerator Start()
    {
        yield return WaitForHandlerReady();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        UnwireHandler(_damageHandler);
    }

    private void Update()
    {
        if (healthSlider == null) return;

        // Use unscaledDeltaTime so the bar animates even when Time.timeScale == 0
        // (e.g. game-over freeze) and drains to 0 after the player dies.
        healthSlider.value = Mathf.Lerp(
            healthSlider.value,
            _targetValue,
            Time.unscaledDeltaTime * _smoothSpeed
        );
    }

    // -------------------------------------------------------------------------
    // Scene reload -- re-acquire the new scene's DamageHandler
    // -------------------------------------------------------------------------

    /// <summary>
    /// Called by SceneManager after every scene load, including reloads.
    /// All Awake() calls in the new scene have already run at this point, so
    /// finding the player by tag is safe and guaranteed to return the fresh object.
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(ReacquireHandler());
    }

    private IEnumerator ReacquireHandler()
    {
        // Unsubscribe from the old (possibly Unity-null) handler first.
        UnwireHandler(_damageHandler);
        _damageHandler = null;

        // Locate the new player and its DamageHandler.
        GameObject playerObj = GameObject.FindWithTag(_playerTag);
        if (playerObj == null)
        {
            Debug.LogError($"[HealthBarPlayer] No GameObject with tag '{_playerTag}' found after scene load.", this);
            yield break;
        }

        if (!playerObj.TryGetComponent(out DamageHandler handler))
        {
            Debug.LogError($"[HealthBarPlayer] Player '{playerObj.name}' has no DamageHandler component.", this);
            yield break;
        }

        WireHandler(handler);

        // Wait until DamageHandler.Init() has been called (MaxHealth > 0).
        yield return WaitForHandlerReady();
    }

    // -------------------------------------------------------------------------
    // Handler wiring helpers
    // -------------------------------------------------------------------------

    /// <summary>Subscribes to the handler's event and caches the reference.</summary>
    private void WireHandler(DamageHandler handler)
    {
        if (handler == null) return;
        _damageHandler = handler;
        _damageHandler.HealthChanged.AddListener(OnHealthChanged);
    }

    /// <summary>
    /// Removes the listener from the supplied handler.
    /// Uses ReferenceEquals rather than the Unity == override so that
    /// RemoveListener is still called on a destroyed (Unity-null) object,
    /// preventing a leaked delegate on the old instance.
    /// </summary>
    private void UnwireHandler(DamageHandler handler)
    {
        if (!ReferenceEquals(handler, null))
            handler.HealthChanged.RemoveListener(OnHealthChanged);
    }

    // -------------------------------------------------------------------------
    // Initialisation helpers
    // -------------------------------------------------------------------------

    /// <summary>
    /// Spins until MaxHealth > 0 (i.e. DamageHandler.Init has been called),
    /// then hard-syncs the bar to the current value.
    /// </summary>
    private IEnumerator WaitForHandlerReady()
    {
        if (_damageHandler == null) yield break;

        while (_damageHandler.MaxHealth <= 0)
        {
            Debug.Log("[HealthBarPlayer] Waiting for DamageHandler to initialise...");
            yield return null;
        }

        Debug.Log($"[HealthBarPlayer] DamageHandler ready. " +
                  $"Health: {_damageHandler.Health}/{_damageHandler.MaxHealth}");
        SyncImmediately();
    }

    // -------------------------------------------------------------------------
    // Event handlers
    // -------------------------------------------------------------------------

    private void OnHealthChanged()
    {
        if (_damageHandler == null) return;

        _targetValue = _damageHandler.MaxHealth > 0
            ? (float)_damageHandler.Health / _damageHandler.MaxHealth
            : 0f;

        if (healthText != null)
            healthText.text = $"{_damageHandler.Health} / {_damageHandler.MaxHealth}";
    }

    private void SyncImmediately()
    {
        if (_damageHandler == null) return;

        _targetValue = _damageHandler.MaxHealth > 0
            ? (float)_damageHandler.Health / _damageHandler.MaxHealth
            : 0f;

        if (healthSlider != null)
            healthSlider.value = _targetValue;

        if (healthText != null)
            healthText.text = $"{_damageHandler.Health} / {_damageHandler.MaxHealth}";
    }
}