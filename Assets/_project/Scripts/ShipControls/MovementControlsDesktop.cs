using System;
using UnityEngine;

[Serializable]
public class MovementControlsDesktop : MovementControlsBase
{
    [SerializeField] float _deadZoneRadius = 0.1f;
    [SerializeField] float _rollSmoothing = 3f;

    float _rollAmount = 0;

    Vector2 ScreenCenter => new(Screen.width * 0.5f, Screen.height * 0.5f);
    Vector2 NormalizedMouseOffset => GetNormalizedMouseOffset();

    public override float YawAmount => ApplyDeadZone(NormalizedMouseOffset.x);

    public override float PitchAmount => -ApplyDeadZone(NormalizedMouseOffset.y);

    public override float RollAmount
    {
        get
        {
            float targetRoll = GetTargetRollAmount();
            _rollAmount = Mathf.Lerp(_rollAmount, targetRoll, Time.deltaTime * _rollSmoothing);
            return _rollAmount;
        }
    }

    public override float ThrustAmount => Input.GetAxis("Vertical");

    Vector2 GetNormalizedMouseOffset()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector2 screenCenter = ScreenCenter;

        return new Vector2(
            (mousePosition.x - screenCenter.x) / screenCenter.x,
            (mousePosition.y - screenCenter.y) / screenCenter.y
        );
    }

    float ApplyDeadZone(float value)
    {
        return Mathf.Abs(value) > _deadZoneRadius ? value : 0f;
    }

    float GetTargetRollAmount()
    {
        if (Input.GetKey(KeyCode.A)) return 1f;
        return Input.GetKey(KeyCode.D) ? -1f : 0f;
    }
}
