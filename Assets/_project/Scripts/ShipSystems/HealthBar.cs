using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image _healthBarImage;
    [SerializeField] private float _updateRate = 1f;
    [SerializeField] private DamageHandler _damageHandler;

    private Transform _transform;
    private Camera _camera;

    private float _targetFillAmount;

    private void Awake()
    {
        _transform = transform;

        if (_damageHandler != null)
            // Sync immediately in case Init() already happened
            UpdateHealthBar();
    }

    private void OnEnable()
    {
        if (_damageHandler == null)
            return;

        _damageHandler.HealthChanged.AddListener(UpdateHealthBar);
        _damageHandler.ObjectDestroyed.AddListener(DisableHealthBar);
        // Ensure UI is synced when enabled
        UpdateHealthBar();
    }

    private void OnDisable()
    {
        if (_damageHandler == null)
            return;

        _damageHandler.HealthChanged.RemoveListener(UpdateHealthBar);
        _damageHandler.ObjectDestroyed.RemoveListener(DisableHealthBar);
    }

    private void LateUpdate()
    {
        // Camera.main can be null temporarily
        if (_camera == null)
        {
            _camera = Camera.main;

            if (_camera == null) return;
        }

        // Billboard toward camera
        _transform.forward = _camera.transform.forward;

        if (_damageHandler == null || _healthBarImage == null)
            return;

        if (Mathf.Approximately(_healthBarImage.fillAmount, _targetFillAmount))
            return;

        _healthBarImage.fillAmount = Mathf.MoveTowards(_healthBarImage.fillAmount, _targetFillAmount, _updateRate * Time.deltaTime);
    }

    private void UpdateHealthBar()
    {
        if (_damageHandler == null || _healthBarImage == null)
            return;

        // Prevent divide by zero
        if (_damageHandler.MaxHealth <= 0)
        {
            _targetFillAmount = 0f;
            return;
        }

        _targetFillAmount = (float)_damageHandler.Health / _damageHandler.MaxHealth;
    }

    void DisableHealthBar()
    {
        gameObject.SetActive(false);
    }
}