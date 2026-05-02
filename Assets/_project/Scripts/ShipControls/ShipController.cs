using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class ShipControls : MonoBehaviour
{
    [Header("Movement Settings")][SerializeField] ShipMovementInput _movementInput;
    [Header("Cockpit Controls")][SerializeField] private AnimationCockpitControls _animationCockpitControls;

    [SerializeField][Range(1000f, 10000f)] private float _thrustForce = 7500f, _pitchForce = 6000f, _rollForce = 1000f, _yawForce = 2000f;

    [Header("Inspector Input")][SerializeField][Range(-1f, 1f)] private float _thrustAmount = 0f;

    [SerializeField][Range(-1f, 1f)] private float _pitchAmount, _rollAmount, _yawAmount = 0f;
    private Rigidbody _rigidbody;

    IMovementControls ControlInput => _movementInput._movementControls;

    [SerializeField] private bool _invertPitch = true;

    [SerializeField] private bool _invertRoll = false;



    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
            _animationCockpitControls.Init(ControlInput);
    }

    private void Update()
    {
        if (ControlInput == null)
        {
            _thrustAmount = 0f;
            _pitchAmount = 0f;
            _rollAmount = 0f;
            _yawAmount = 0f;
            return;
        }

        _thrustAmount = ControlInput.ThrustAmount;
        _pitchAmount = ControlInput.PitchAmount;
        _rollAmount = ControlInput.RollAmount;
        _yawAmount = ControlInput.YawAmount;

        if (_invertPitch)
        {
            _pitchAmount = -_pitchAmount;
        }

        if (_invertRoll)
        {
            _rollAmount = -_rollAmount;
        }
    }

    private void FixedUpdate()
    {

        if (!Mathf.Approximately(0f, _pitchAmount))
        {
            _rigidbody.AddTorque(transform.right * _pitchAmount * _pitchForce * Time.fixedDeltaTime, ForceMode.Force);
        }

        if (!Mathf.Approximately(0f, _rollAmount))
        {
            _rigidbody.AddTorque(transform.forward * _rollAmount * _rollForce * Time.fixedDeltaTime, ForceMode.Force);
        }

        if (!Mathf.Approximately(0f, _yawAmount))
        {
            _rigidbody.AddTorque(transform.up * _yawAmount * _yawForce * Time.fixedDeltaTime, ForceMode.Force);
        }

        if (!Mathf.Approximately(0f, _thrustAmount))
        {
            _rigidbody.AddForce(transform.forward * _thrustAmount * _thrustForce * Time.fixedDeltaTime, ForceMode.Force);
        }
    }
}
