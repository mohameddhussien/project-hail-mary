using System.Collections.Generic;
using UnityEngine;

public class DamageDelegator : MonoBehaviour, IDamageable
{
    [SerializeField] private GameObject[] _damageDelegates;

    private readonly List<IDamageable> _damageReceivers = new();

    private void Awake()
    {
        if (_damageDelegates == null)
            return;

        foreach (GameObject damageDelegate in _damageDelegates)
        {
            if (damageDelegate == null)
                continue;

            if (damageDelegate.TryGetComponent<IDamageable>(out var damageable))
            {
                _damageReceivers.Add(damageable);
                continue;
            }
            Debug.LogWarning($"{damageDelegate.name} does not implement IDamageable.", damageDelegate);
        }
    }

    public void TakeDamage(int damage, Vector3 hitPosition)
    {
        for (int i = 0; i < _damageReceivers.Count; i++) _damageReceivers[i].TakeDamage(damage, hitPosition);
    }
}