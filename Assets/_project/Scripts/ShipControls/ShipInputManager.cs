using UnityEngine;

public class ShipInputManager : MonoBehaviour
{
    public enum InputType
    {
        HumanDesktop,
        HumanMobile,
        Bot
    }

    public static IMovementControls GetInputControls(InputType inputType)
    {
        return inputType switch
        {
            InputType.HumanDesktop => new MovementControlsDesktop(),
            InputType.HumanMobile => null,
            InputType.Bot => null,
            _ => throw new System.ArgumentOutOfRangeException(nameof(inputType), inputType, null)
        };
    }
}
