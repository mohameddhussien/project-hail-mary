using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class MovementControlsDesktop : MovementControlsBase
{
    [SerializeField]
    [Min(0f)]
    private float _mouseDeadzone = 40f;
    private float _rollAmount = 0f;


    private Vector2 ScreenCenter => new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

    public override float ThrustAmount => Input.GetAxis("Vertical");

    public override float PitchAmount
    {
        get
        {
            Mouse mouse = Mouse.current;
            if (mouse == null)
            {
                return 0f;
            }

            Vector2 mousePosition = mouse.position.ReadValue();
            float pitch = GetMouseAxisAmount(mousePosition.y - ScreenCenter.y, ScreenCenter.y);
            return Mathf.Clamp(pitch, -1f, 1f);
        }
    }

    public override float RollAmount
    {
        get
        {
            float roll;
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                roll = 0f;
            }
            else
            {
                roll = GetKeyboardAxis(keyboard.dKey.isPressed, keyboard.aKey.isPressed);
            }
            _rollAmount = Mathf.Lerp(_rollAmount, roll, Time.deltaTime * 3f);
            return _rollAmount;
        }
    }

    public override float YawAmount
    {
        get
        {
            Mouse mouse = Mouse.current;
            if (mouse == null)
            {
                return 0f;
            }

            Vector2 mousePosition = mouse.position.ReadValue();
            float yaw = GetMouseAxisAmount(mousePosition.x - ScreenCenter.x, ScreenCenter.x);
            return Mathf.Clamp(yaw, -1f, 1f);
        }
    }

    private float GetMouseAxisAmount(float mouseOffset, float screenHalfSize)
    {
        if (Mathf.Abs(mouseOffset) <= _mouseDeadzone)
        {
            return 0f;
        }

        float maxDistanceFromDeadzone = Mathf.Max(1f, screenHalfSize - _mouseDeadzone);
        float adjustedOffset = mouseOffset - Mathf.Sign(mouseOffset) * _mouseDeadzone;
        return adjustedOffset / maxDistanceFromDeadzone;
    }

    private float GetKeyboardAxis(bool positivePressed, bool negativePressed)
    {
        if (positivePressed == negativePressed)
        {
            return 0f;
        }

        return positivePressed ? 1f : -1f;
    }
}
