using UnityEngine;

public class CameraWobble : MonoBehaviour
{
    [Header("References")]
    public Camera targetCamera;

    [Header("Wobble Settings")]
    public float rotationAmount = 5f;      // Max tilt in degrees
    public float smoothSpeed = 5f;         // How smooth the movement is
    public float returnSpeed = 2f;         // How fast it recenters

    [Header("Idle Wobble")]
    public float idleAmplitude = 0.5f;
    public float idleFrequency = 1f;

    private Quaternion initialRotation;

    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        initialRotation = targetCamera.transform.localRotation;
    }

    void Update()
    {
        if (targetCamera == null) return;

        // 🖱️ Mouse position normalized (-1 to 1)
        float mouseX = (Input.mousePosition.x / Screen.width) * 2f - 1f;
        float mouseY = (Input.mousePosition.y / Screen.height) * 2f - 1f;

        // 🎯 Target rotation based on mouse
        float rotX = -mouseY * rotationAmount;
        float rotY = mouseX * rotationAmount;

        Quaternion targetRotation = Quaternion.Euler(rotX, rotY, 0f);

        // 🌊 Idle wobble (subtle sine wave)
        float idleX = Mathf.Sin(Time.time * idleFrequency) * idleAmplitude;
        float idleY = Mathf.Cos(Time.time * idleFrequency) * idleAmplitude;

        Quaternion idleRotation = Quaternion.Euler(idleX, idleY, 0f);

        // 🎯 Final rotation
        Quaternion finalRotation = initialRotation * targetRotation * idleRotation;

        // 🧈 Smooth interpolation
        targetCamera.transform.localRotation = Quaternion.Slerp(
            targetCamera.transform.localRotation,
            finalRotation,
            Time.deltaTime * smoothSpeed
        );
    }
}
