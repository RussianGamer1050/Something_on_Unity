using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Movement stats
    public float speed = 5f; // Movement speed
    public float runSpeed = 8f; // Speed when running
    public float crouchSpeed = 2f; // Speed when crouching

    // Height variables
    public float crouchHeight = 1f; // Height when crouched
    private float originalHeight; // Original height of the player

    // Variables for smooth movement
    public float acceleration = 10f; // How fast the player accelerates
    public float runAcceleration = 5f; // Running acceleration factor
    public float deceleration = 8f; // How fast the player decelerates

    // Mouse look variables
    public float mouseSensitivity = 100f; // Sensitivity for camera rotation
    public float verticalLookLimit = 90f; // Limit vertical look to ±90 degrees
    public Transform playerCamera; // Reference to the camera

    // Physics variables
    public float gravity = -9.81f; // Gravity value

    // Movement variables
    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 currentVelocity = Vector3.zero; // Tracks the current velocity
    private bool isGrounded;
    private float xRotation = 0f; // Track vertical rotation

    // Audio variables
    public AudioClip footstepClip;
    private AudioSource audioSource;
    private float stepInterval = 1.5f; // Time between steps
    private float nextStepTime = 0f;


    void Start()
    {
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        originalHeight = controller.height; // Store the original height
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
        isGrounded = controller.isGrounded;

        // Gravity
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Get input for desired movement direction
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 desiredMove = (transform.right * horizontal + transform.forward * vertical).normalized;

        // Adjust speed for running or crouching
        float currentSpeed = speed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = Mathf.Lerp(currentSpeed, runSpeed, runAcceleration * Time.fixedDeltaTime);
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            currentSpeed = crouchSpeed;
            controller.height = Mathf.Lerp(controller.height, crouchHeight, 10f * Time.deltaTime);
        }
        else
        {
            controller.height = Mathf.Lerp(controller.height, originalHeight, 10f * Time.deltaTime);
        }

        // Smoothly interpolate current velocity toward desired movement
        if (desiredMove.magnitude > 0)
        {
            currentVelocity = Vector3.Lerp(currentVelocity, desiredMove * currentSpeed, acceleration * Time.fixedDeltaTime);

            // Play footstep sounds at intervals
            if (Time.time >= nextStepTime)
            {
                audioSource.PlayOneShot(footstepClip);
                nextStepTime = Time.time + stepInterval / currentSpeed; // Faster steps while running
            }
        }
        else
        {
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
        }

        // Apply movement
        controller.Move(currentVelocity * Time.fixedDeltaTime);

        // Apply gravity
        velocity.y += gravity * Time.fixedDeltaTime;
        controller.Move(new Vector3(0, velocity.y, 0) * Time.fixedDeltaTime);
    }
}
