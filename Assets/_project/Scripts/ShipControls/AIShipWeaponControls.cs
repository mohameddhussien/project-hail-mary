using UnityEngine;

public class AIShipWeaponControls : WeaponControlsBase
{
    public override bool PrimaryFired => _firePrimary;
    public override bool SecondaryFired => false;

    bool _firePrimary;

    Transform _transform;
    Transform _target;

    float _attackRange;
    int _layerMask;

    void Awake()
    {
        _transform = transform;
    }

    void Update()
    {
        _firePrimary = CanFirePrimary();

        Debug.Log($"[AI WEAPON] Fire State: {_firePrimary}");
    }

    bool CanFirePrimary()
    {
        if (!_target)
        {
            Debug.Log("[AI WEAPON] No target assigned");
            return false;
        }

        float distance = Vector3.Distance(
            _transform.position,
            _target.position
        );

        Debug.Log($"[AI WEAPON] Distance To Target: {distance}");

        Debug.Log($"[AI WEAPON] Attack Range: {_attackRange}");

        Vector3 dir =
            (_target.position - _transform.position).normalized;

        Debug.DrawRay(
            _transform.position,
            dir * _attackRange,
            Color.red
        );

        bool hitSomething = Physics.Raycast(
            _transform.position,
            dir,
            out RaycastHit hit,
            _attackRange,
            _layerMask
        );

        if (!hitSomething)
        {
            Debug.Log("[AI WEAPON] Raycast missed");
            return false;
        }

        Debug.Log($"[AI WEAPON] Hit Object: {hit.collider.name}");

        bool valid =
            hit.transform == _target ||
            hit.transform.root == _target.root;

        Debug.Log($"[AI WEAPON] Valid Target Hit: {valid}");

        return valid;
    }

    public void SetTarget(
        Transform target,
        float attackRange,
        int targetMask)
    {
        _target = target;
        _attackRange = attackRange;
        _layerMask = targetMask;

        Debug.Log(
            $"[AI WEAPON] Target Set: " +
            $"{(target ? target.name : "NULL")} | " +
            $"Range: {_attackRange} | " +
            $"Mask: {_layerMask}"
        );
    }
}