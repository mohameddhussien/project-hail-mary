using System.Collections.Generic;
using UnityEngine;

public class AnimationCockpitControls : MonoBehaviour
{
    [SerializeField] private Transform _joystick;

    [SerializeField] private Vector3 _joystickRange = Vector3.zero;

    [SerializeField] private List<Transform> _throttles;

    [SerializeField] private float _throttleRange = 35f;
    IMovementControls _movementControls;

    private void Awake()
    {

    }

    private void Update()
    {
            if (_movementControls == null) return;
        _joystick.localRotation = Quaternion.Euler(_movementControls.PitchAmount * _joystickRange.x, _movementControls.YawAmount * _joystickRange.y, -_movementControls.RollAmount * _joystickRange.z);

        Vector3 throttleRotation = _throttles[0].localRotation.eulerAngles;
        throttleRotation.x = _movementControls.ThrustAmount * _throttleRange;
        foreach (Transform throttle in _throttles)
        {
            throttle.localRotation = Quaternion.Euler(throttleRotation);
        }
    }

    public void Init(IMovementControls movementControls)
    {
        _movementControls = movementControls;
    }

}
