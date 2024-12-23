using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f; // Movement speed
    public float mouseSensitivity = 100f; // Sensitivity for camera rotation
    public float verticalLookLimit = 90f; // Limit vertical look to ±90 degrees
    public Transform playerCamera; // Reference to the camera

    public float jumpHeight = 2f; // Jump height
    public float gravity = -9.81f; // Gravity value

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation = 0f; // Track vertical rotation

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // Lock cursor for FPS
    }

    void Update()
    {
        HandleMouseLook();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    // Mouse look logic
    void HandleMouseLook()
    {
        // Mouse look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Horizontal rotation (player body)
        transform.Rotate(Vector3.up * mouseX);

        // Vertical rotation (camera)
        xRotation -= mouseY; // Subtract to invert controls (typical FPS)
        xRotation = Mathf.Clamp(xRotation, -verticalLookLimit, verticalLookLimit); // Clamp rotation
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    // Movement logic
    void HandleMovement()
    {
        // Check if the player is grounded
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Reset velocity when grounded
        }

        // Get input for movement
        float horizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float vertical = Input.GetAxis("Vertical"); // W/S or Up/Down
        Vector3 move = transform.right * horizontal + transform.forward * vertical;

        // Move the player
        controller.Move(move * speed * Time.fixedDeltaTime);

        // Jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply gravity
        velocity.y += gravity * Time.fixedDeltaTime;
        controller.Move(velocity * Time.fixedDeltaTime);
    }
}
