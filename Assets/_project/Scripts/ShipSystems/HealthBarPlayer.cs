using System.Collections;
using UnityEngine;
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
    [SerializeField] private DamageHandler _damageHandler;

    private float _targetValue = 1f;

    private void Awake()
    {
        if (_damageHandler == null)
        {
            Debug.LogError("[HealthBarPlayer] DamageHandler is NOT assigned in Inspector!", this);
            return;
        }

        _damageHandler.HealthChanged.AddListener(OnHealthChanged);
        _damageHandler.ObjectDestroyed.AddListener(OnPlayerDied);
    }

    private IEnumerator Start()
    {
        if (_damageHandler == null) yield break;

        // Wait until DamageHandler.Init() has been called
        // It's not initialized until MaxHealth > 0
        // This is order-independent — works no matter which script runs first
        while (_damageHandler.MaxHealth <= 0)
        {
            Debug.Log("[HealthBarPlayer] Waiting for DamageHandler to initialize...");
            yield return null; // wait one frame, then check again
        }

        Debug.Log($"[HealthBarPlayer] DamageHandler ready. Health: {_damageHandler.Health}/{_damageHandler.MaxHealth}");
        SyncImmediately();
    }

    private void OnDestroy()
    {
        if (_damageHandler == null) return;

        _damageHandler.HealthChanged.RemoveListener(OnHealthChanged);
        _damageHandler.ObjectDestroyed.RemoveListener(OnPlayerDied);
    }

    private void Update()
    {
        if (healthSlider == null) return;

        healthSlider.value = Mathf.Lerp(
            healthSlider.value,
            _targetValue,
            Time.deltaTime * _smoothSpeed
        );
    }

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

    private void OnPlayerDied()
    {
        Debug.Log("[HealthBarPlayer] Player died — calling ShowGameOver");

        if (GameOverManager.Instance != null)
            GameOverManager.Instance.ShowGameOver();
        else
            Debug.LogError("[HealthBarPlayer] GameOverManager.Instance is NULL!");
    }
}