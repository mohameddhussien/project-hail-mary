using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarScreen : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private LayerMask _layerMask;

    [SerializeField]
    [Range(100, 5000)]
    private float _detectionRange = 500f;

    [SerializeField]
    [Range(0, 1000)]
    private float _lockOnRange = 1000f;

    [SerializeField]
    [Range(0, 45)]
    private float _lockOnRadius = 15f;

    [SerializeField] private int _maxTargets = 200;

    [SerializeField] private float _refreshDelay = 0.25f;

    [Header("References")]
    [SerializeField] private Transform _player;

    [Header("Debug")]
    [SerializeField] private bool _enableDebugLogs = true;

    private readonly List<Transform> _targetsInRange = new();

    private Collider[] _targetColliders;

    private WaitForSeconds _waitForSeconds;

    private Transform _transform;
    private Transform _lockedOnTarget;

    private void Awake()
    {
        _transform = transform;

        _targetColliders = new Collider[_maxTargets];

        _waitForSeconds = new WaitForSeconds(_refreshDelay);

        DebugLog("RadarScreen Awake");

        if (_player == null)
        {
            Debug.LogWarning(
                "[RadarScreen] Player reference is NULL.",
                this
            );
        }
    }

    private void OnEnable()
    {
        DebugLog("RadarScreen Enabled");

        StartCoroutine(RefreshTargetList());
    }

    private void OnDisable()
    {
        DebugLog("RadarScreen Disabled");

        StopAllCoroutines();
    }

    private void LateUpdate()
    {
        _targetsInRange.RemoveAll(target => target == null);

        if (UIManager.Instance == null)
        {
            Debug.LogWarning(
                "[RadarScreen] UIManager.Instance is NULL."
            );

            return;
        }

        DebugLog(
            $"Updating indicators. Targets: {_targetsInRange.Count}"
        );

        UIManager.Instance.UpdateTargetIndicators(
            _targetsInRange,
            _lockedOnTarget != null
                ? _lockedOnTarget.GetInstanceID()
                : -1
        );
    }

    private IEnumerator RefreshTargetList()
    {
        while (true)
        {
            RefreshTargets();

            yield return _waitForSeconds;
        }
    }

    private void RefreshTargets()
    {
        _targetsInRange.Clear();

        _lockedOnTarget = null;

        float closestTargetDistance = _lockOnRange;

        Vector3 myPosition = _transform.position;

        int size = Physics.OverlapSphereNonAlloc(
            myPosition,
            _detectionRange,
            _targetColliders,
            _layerMask
        );

        DebugLog($"OverlapSphere found {size} colliders.");

        for (int i = 0; i < size; i++)
        {
            Collider col = _targetColliders[i];

            if (col == null)
            {
                DebugLog($"Collider at index {i} is NULL.");
                continue;
            }

            DebugLog($"Detected collider: {col.name}");

            Transform target = GetRootTransform(i);

            if (target == null)
            {
                DebugLog($"Root transform is NULL for {col.name}");
                continue;
            }

            DebugLog($"Resolved target: {target.name}");

            if (!target.gameObject.activeInHierarchy)
            {
                DebugLog($"{target.name} is inactive.");
                continue;
            }

            if (!_targetsInRange.Contains(target))
            {
                _targetsInRange.Add(target);

                DebugLog($"Added target: {target.name}");
            }

            closestTargetDistance = TryLockOnTarget(
                target,
                myPosition,
                closestTargetDistance
            );
        }

        DebugLog($"Final target count: {_targetsInRange.Count}");

        if (_lockedOnTarget != null)
        {
            DebugLog($"Locked target: {_lockedOnTarget.name}");
        }
        else
        {
            DebugLog("No locked target.");
        }
    }

    private float TryLockOnTarget(
        Transform target,
        Vector3 myPosition,
        float closest
    )
    {
        Vector3 direction = target.position - myPosition;

        float distance = direction.magnitude;

        float angle = Vector3.Angle(
            direction,
            _transform.forward
        );

        DebugLog(
            $"Target: {target.name} | Distance: {distance:F1} | Angle: {angle:F1}"
        );

        if (distance < closest && angle < _lockOnRadius)
        {
            closest = distance;

            _lockedOnTarget = target;

            DebugLog($"LOCKED ON: {target.name}");
        }

        return closest;
    }

    private Transform GetRootTransform(int index)
    {
        Collider col = _targetColliders[index];

        if (col == null)
            return null;

        Transform root = col.transform;

        int layer = root.gameObject.layer;

        while (root.parent != null &&
               layer == root.parent.gameObject.layer)
        {
            root = root.parent;
        }

        return root;
    }

    private void DebugLog(string message)
    {
        if (!_enableDebugLogs)
            return;

        Debug.Log($"[RadarScreen] {message}", this);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _lockOnRange);
    }
}