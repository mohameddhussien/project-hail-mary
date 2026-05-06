using UnityEngine;

public class Astronaut_Movement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float mouseSensitivity = 100f;

    private float yRotation = 0f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // --- ROTATION (Mouse) ---
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        yRotation += mouseX;
        transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    void FixedUpdate()
    {
        // --- MOVEMENT (WASD) via Rigidbody ---
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        rb.MovePosition(rb.position + move.normalized * moveSpeed * Time.deltaTime);
    }
}