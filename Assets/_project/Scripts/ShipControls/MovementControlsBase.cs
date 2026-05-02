public abstract class MovementControlsBase : IMovementControls
{
    public abstract float ThrustAmount { get; }
    public abstract float PitchAmount { get; }
    public abstract float RollAmount { get; }
    public abstract float YawAmount { get; }
}
