using System;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class EnemyShipController : ShipController
{
    [Header("Ranges")]
    [SerializeField] float _patrolRange = 2000f;
    [SerializeField] float _attackRange = 1000f;

    [Header("Aggression")]
    [Tooltip("Retreat when HP drops below this fraction of max health (0 = never retreat, 1 = always retreat).")]
    [SerializeField, Range(0f, 1f)] float _retreatHealthPercent = 0.20f;

    [Tooltip("How far away the reposition target is placed from the ship.")]
    [SerializeField] float _repositionDistance = 250f;

    [Tooltip("How far the retreat target is placed from the ship.")]
    [SerializeField] float _retreatDistance = 5000f;

    [Header("Layer Masks")]
    [SerializeField] LayerMask _targetMask;
    [SerializeField] LayerMask _playerMask;

    // -------------------------------------------------------------------------
    // State machine
    // -------------------------------------------------------------------------

    enum EnemyShipState
    {
        None,
        Patrol,
        Attack,
        Reposition,
        Retreat
    }

    AIShipMovementControls _aiShipMovementControls;
    AIShipWeaponControls _aiShipWeaponControls;
    EnemyShipState _state = EnemyShipState.None;
    Transform _transform;

    // Cached player reference – refreshed lazily to avoid FindGameObjectWithTag
    // being called every property access (which is multiple times per frame).
    GameObject _playerShipCache;
    GameObject PlayerShip
    {
        get
        {
            // Only search again when the cache is truly empty / destroyed.
            if (_playerShipCache == null)
                _playerShipCache = GameObject.FindGameObjectWithTag("Player");
            return _playerShipCache;
        }
    }

    Transform _target;

    public UnityEvent<int> ShipDestroyed = new();
    bool _destroyed;

    // -------------------------------------------------------------------------
    // Debug properties
    // -------------------------------------------------------------------------

    public string ShipState => _state.ToString();
    public string TargetName => _target ? _target.name : "none";

    public string DistanceToTarget
    {
        get
        {
            if (_target == null) return string.Empty;
            return $"{Vector3.Distance(_target.position, _transform.position):F2}";
        }
    }

    public string HealthLevel => $"{_damageHandler.Health}/{_damageHandler.MaxHealth}";

    // -------------------------------------------------------------------------
    // Condition helpers
    // -------------------------------------------------------------------------

    bool InAttackRange
    {
        get
        {
            if (PlayerShip == null) return false;
            return Vector3.Distance(PlayerShip.transform.position, _transform.position) <= _attackRange;
        }
    }

    // BUG FIX: _retreatHealthPercent is now serialized so it can be tuned without
    // recompiling, and defaults to 20% instead of the original hard-coded 33%.
    bool ShouldRetreat => _damageHandler.Health < (_damageHandler.MaxHealth * _retreatHealthPercent);

    // BUG FIX: guard against _target being null before accessing position.
    bool ReachedPatrolTarget => _target != null &&
        Vector3.Distance(_target.position, _transform.position) < 10f;

    bool ShouldReposition => Physics.SphereCast(
        _transform.position, 3f, _transform.forward, out _, 100f, _playerMask);

    public float VectorDifference
    {
        get
        {
            if (PlayerShip == null) return 0f;
            return (PlayerShip.transform.forward - _transform.forward).magnitude;
        }
    }

    // -------------------------------------------------------------------------
    // Unity lifecycle
    // -------------------------------------------------------------------------

    public override void OnEnable()
    {
        _transform = transform;
        _aiShipMovementControls = (AIShipMovementControls)_movementControls;
        _aiShipWeaponControls = (AIShipWeaponControls)_weaponControls;
        SetState(EnemyShipState.Patrol);
        base.OnEnable();
    }

    void OnDisable()
    {
        _destroyed = true;
        ShipDestroyed.Invoke(gameObject.GetInstanceID());
    }

    public override void Update()
    {
        if (_destroyed) return;
        EnemyShipState next = GetNextState();
        SetState(next);
        base.Update();
    }

    // -------------------------------------------------------------------------
    // State machine – transitions
    // -------------------------------------------------------------------------

    EnemyShipState GetNextState()
    {
        if (_destroyed) return EnemyShipState.None;

        return _state switch
        {
            EnemyShipState.Patrol => Patrol(),
            EnemyShipState.Attack => Attack(),
            EnemyShipState.Reposition => Reposition(),
            EnemyShipState.Retreat => Retreat(),
            _ => EnemyShipState.None
        };
    }

    EnemyShipState Patrol()
    {
        if (ShouldRetreat) return EnemyShipState.Retreat;
        if (InAttackRange) return EnemyShipState.Attack;

        // BUG FIX: set patrol waypoint relative to the ship's current world
        // position, not relative to world origin (0,0,0). The old code placed
        // every waypoint within a sphere centred on the scene origin, so ships
        // that had drifted far from origin would never reach them.
        if (ReachedPatrolTarget && _target != null)
        {
            _target.position = _transform.position + Random.insideUnitSphere * _patrolRange;
        }

        return EnemyShipState.Patrol;
    }

    EnemyShipState Attack()
    {
        if (ShouldRetreat) return EnemyShipState.Retreat;
        if (ShouldReposition) return EnemyShipState.Reposition;

        // If the player somehow left attack range (e.g. boosted away) go back
        // to patrol so the ship pursues rather than standing still shooting air.
        if (!InAttackRange) return EnemyShipState.Patrol;

        return EnemyShipState.Attack;
    }

    EnemyShipState Reposition()
    {
        if (ShouldRetreat) return EnemyShipState.Retreat;

        // BUG FIX: the old condition checked distance to _target (the
        // reposition waypoint). That is correct, but 100 f is generous –
        // keep it so the ship actually commits to the new position.
        if (_target == null) return EnemyShipState.Attack;

        return Vector3.Distance(_target.position, _transform.position) < 100f
            ? EnemyShipState.Attack
            : EnemyShipState.Reposition;
    }

    EnemyShipState Retreat()
    {
        // Once retreating, stay retreating. A designer can expand this later
        // (e.g. re-engage when health is restored via a pickup).
        return EnemyShipState.Retreat;
    }

    // -------------------------------------------------------------------------
    // State machine – entry actions
    // -------------------------------------------------------------------------

    void SetState(EnemyShipState state)
    {
        if (_state == state) return;
        _state = state;

        switch (state)
        {
            case EnemyShipState.Patrol:
                // BUG FIX: destroy any leftover temporary target from a
                // previous Reposition or Retreat before creating a new patrol
                // one, to prevent orphaned GameObjects accumulating in the scene.
                DestroyTemporaryTarget();

                _target = new GameObject("Patrol Target").transform;
                // BUG FIX: initial patrol waypoint relative to ship position.
                _target.position = _transform.position + Random.insideUnitSphere * _patrolRange;
                _aiShipMovementControls.SetTarget(_target);
                break;

            case EnemyShipState.Attack:
                // BUG FIX: destroy any temporary waypoint before switching to
                // the player transform as the target.
                DestroyTemporaryTarget();

                _target = PlayerShip != null ? PlayerShip.transform : null;
                _aiShipMovementControls.SetTarget(_target);
                if (_target != null)
                    SetWeaponsTarget(_target, _attackRange, _targetMask);
                break;

            case EnemyShipState.Reposition:
                // BUG FIX: destroy the old temporary target before assigning a new one.
                DestroyTemporaryTarget();

                _target = GetRepositionTarget();
                _aiShipMovementControls.SetTarget(_target);
                SetWeaponsTarget(null, 0, 0);
                break;

            case EnemyShipState.Retreat:
                DestroyTemporaryTarget();

                _target = GetRetreatTarget();
                _aiShipMovementControls.SetTarget(_target);
                SetWeaponsTarget(null, 0, 0);
                break;
        }
    }

    // -------------------------------------------------------------------------
    // Waypoint helpers
    // -------------------------------------------------------------------------

    /// <summary>
    /// Destroys the temporary waypoint GameObject if <see cref="_target"/> is
    /// one we own (patrol, reposition, retreat). Does nothing when _target is
    /// the player's transform.
    /// </summary>
    void DestroyTemporaryTarget()
    {
        if (_target == null) return;

        // We never own the player transform, so skip it.
        if (PlayerShip != null && _target == PlayerShip.transform) return;

        Destroy(_target.gameObject);
        _target = null;
    }

    // BUG FIX: the original code calculated direction as (player − ship), then
    // multiplied *that* (un-normalised) vector by -5000. Because the direction
    // vector has arbitrary magnitude, the retreat position ended up at a random
    // wildly incorrect world-space point instead of directly behind the ship.
    Transform GetRetreatTarget()
    {
        Vector3 awayDirection = PlayerShip != null
            ? (_transform.position - PlayerShip.transform.position).normalized
            : _transform.forward * -1f;           // fallback if player is gone

        var target = new GameObject("Retreat Target").transform;
        target.position = _transform.position + awayDirection * _retreatDistance;
        return target;
    }

    Transform GetRepositionTarget()
    {
        var target = new GameObject("Reposition Target").transform;

        int rand = Random.Range(1, 5);        // [1, 4] inclusive
        Vector3 right = _transform.right;
        Vector3 up = _transform.up;

        Vector3 direction = rand switch
        {
            1 => right,
            2 => -right,
            3 => up,
            4 => -up,
            _ => -_transform.forward
        };

        target.position = _transform.position + direction * _repositionDistance;
        return target;
    }

    void SetWeaponsTarget(Transform target, float attackRange, int targetMask)
    {
        _aiShipWeaponControls.SetTarget(target, attackRange, targetMask);
    }
}