using UnityEngine;

public class AIShipWeaponControls : WeaponControlsBase
{
    public override bool PrimaryFired => _firePrimary;
    public override bool SecondaryFired => false;

    bool _firePrimary;
    Transform _transform, _target;
    float _attackRange;
    int _layerMask;

    void Awake()
    {
        _transform = transform;
    }

    void Update()
    {
        _firePrimary = CanFirePrimary();
    }

    bool CanFirePrimary()
    {
        if (!_target) return false;
        return Physics.Raycast(_transform.position, _transform.forward, out var hit, _attackRange * 0.5f, _layerMask);
    }

    public void SetTarget(Transform target, float attackRange, int targetMask)
    {
        _target = target;
        _attackRange = attackRange;
        _layerMask = targetMask;
    }
}