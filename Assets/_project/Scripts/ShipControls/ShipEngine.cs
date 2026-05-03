using UnityEngine;

public class ShipEngine : MonoBehaviour
{
    [SerializeField] GameObject _thruster;

    IMovementControls _shipMovementControls;
    Rigidbody _rigidbody;

    float _thrustForce, _thrustAmount;

    bool ThrustersEnabled => !Mathf.Approximately(0f, _shipMovementControls.ThrustAmount);

    void Update()
    {
        ActivateThrusters();
    }

    void FixedUpdate()
    {
        if (!ThrustersEnabled) return;
        _rigidbody.AddForce(_thrustAmount * Time.fixedDeltaTime * transform.forward);
    }

    public void Init(IMovementControls movementControls, Rigidbody rb, float thrustForce)
    {
        _shipMovementControls = movementControls;
        _rigidbody = rb;
        _thrustForce = thrustForce;
    }

    void ActivateThrusters()
    {
        _thruster.SetActive(ThrustersEnabled);
        if (!ThrustersEnabled) return;
        _thrustAmount = _thrustForce * _shipMovementControls.ThrustAmount;
    }
    
}