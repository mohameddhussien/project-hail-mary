using UnityEngine;
public class ShipControls : MonoBehaviour
{
    [SerializeField]
    [Range(1000f, 10000f)]
    private float _thrustForce = 7500f, _pitchForce = 6000f, _rollForce = 1000f, _yawForce = 2000f;

    Rigidbody _rigidbody;
    [SerializeField]
    [Range(-1f, 1f)]
    float _thrustAmount, _pitchAmount, _rollAmount, _yawAmount = 0f;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!Mathf.Approximately(0f, _pitchAmount))
        {
            // _rigidbody.AddTorque(transform.up * _yawAmount * _yawForce);
            _rigidbody.AddTorque(transform.right * _pitchAmount * _pitchForce * Time.fixedDeltaTime);
        }

        if (!Mathf.Approximately(0f, _rollAmount))
        {
            _rigidbody.AddTorque(transform.forward * _rollAmount * _rollForce * Time.fixedDeltaTime);
        }

        if (!Mathf.Approximately(0f, _yawAmount))
        {
            // _rigidbody.AddForce(transform.forward * _thrustAmount * _thrustForce);
            _rigidbody.AddTorque(transform.up * _yawAmount * _yawForce * Time.fixedDeltaTime);
        }

        if (!Mathf.Approximately(0f, _thrustAmount))
        {
            _rigidbody.AddForce(transform.forward * _thrustAmount * _thrustForce * Time.fixedDeltaTime);
        }
    }
}
