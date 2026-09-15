using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float verticalSpeed = 5f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 2f;

    [Header("Gamepad Look")]
    public float gamepadLookSpeed = 140f;
    public float gamepadDeadZone = 0.12f;
    public bool invertGamepadY = false;

    [Header("References")]
    public Camera playerCamera;

    private CharacterController controller;
    private float pitch = 0f;

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    void Update()
    {
        HandleLook();
        HandleMovement();
    }

    void HandleLook()
    {
        if (playerCamera == null)
            return;

        // Mouse
        float lookX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float lookY = Input.GetAxis("Mouse Y") * mouseSensitivity;

#if ENABLE_INPUT_SYSTEM
        // Right analog stick
        Gamepad pad = Gamepad.current;

        if (pad != null)
        {
            Vector2 rightStick = pad.rightStick.ReadValue();

            if (rightStick.magnitude < gamepadDeadZone)
                rightStick = Vector2.zero;

            lookX += rightStick.x * gamepadLookSpeed * Time.unscaledDeltaTime;

            float gamepadY = rightStick.y;
            if (!invertGamepadY)
                gamepadY = -gamepadY;

            lookY += gamepadY * gamepadLookSpeed * Time.unscaledDeltaTime;
        }
#endif

        // Horizontal rotation rotates the player
        transform.Rotate(Vector3.up * lookX);

        // Vertical rotation rotates only the camera.
        // No clamp: free vertical look, as in your current version.
        pitch -= lookY;
        playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void HandleMovement()
    {
        if (playerCamera == null)
            return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 forward = playerCamera.transform.forward.normalized;
        Vector3 right = playerCamera.transform.right.normalized;

        Vector3 movement = (forward * vertical) + (right * horizontal);

        // Keyboard vertical movement
        if (Input.GetKey(KeyCode.E))
            movement += Vector3.up;

        if (Input.GetKey(KeyCode.Q))
            movement -= Vector3.up;

        if (movement.sqrMagnitude > 1f)
            movement.Normalize();

        controller.Move(movement * moveSpeed * Time.deltaTime);
    }
}
