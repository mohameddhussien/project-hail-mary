using UnityEngine;
using UnityEngine.InputSystem;

public class ShipMovementInput : MonoBehaviour
{
    [SerializeField] ShipInputManager.InputType _inputType = ShipInputManager.InputType.HumanDesktop;

    public IMovementControls _movementControls { get; private set; }

    void Start()
    {
        _movementControls = ShipInputManager.GetInputControls(_inputType);       
    }

    void Oestroy()
    {
        _movementControls = null;
    }
}
