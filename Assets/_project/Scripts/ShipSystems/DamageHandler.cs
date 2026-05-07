using UnityEngine;
using UnityEngine.Events;

public class DamageHandler : MonoBehaviour, IDamageable
{
    [SerializeField] private GameObject _explosionPrefab;

    private UnityEvent _healthChangedEvent;
    private UnityEvent _objectDestroyedEvent;

    private bool _isDead;

    public int MaxHealth { get; private set; }
    public int Health { get; private set; }

    public UnityEvent HealthChanged => _healthChangedEvent ??= new UnityEvent();
    public UnityEvent ObjectDestroyed => _objectDestroyedEvent ??= new UnityEvent();

    public void Init(int maxHealth)
    {
        MaxHealth = maxHealth;
        Health = maxHealth;
        _isDead = false;

        HealthChanged.Invoke();
    }

    public void TakeDamage(int damage, Vector3 hitPosition)
    {
        // Prevent taking damage after death
        if (_isDead)
            return;

        // Prevent negative damage
        damage = Mathf.Max(damage, 0);

        // Clamp health to 0
        Health = Mathf.Max(Health - damage, 0);

        HealthChanged.Invoke();

        // Still alive
        if (Health > 0)
            return;

        // Mark as dead
        _isDead = true;

        // Spawn explosion effect
        if (_explosionPrefab != null)
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);

        // Notify listeners
        ObjectDestroyed.Invoke();
    }
}