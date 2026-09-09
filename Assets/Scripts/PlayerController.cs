using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float verticalSpeed = 2f;

    [Header("Look")]
    public Transform cameraTransform;
    public float mouseSensitivity = 2f;

    private CharacterController controller;
    private float pitch = 0f;

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraTransform != null)
        {
            float startPitch = cameraTransform.localEulerAngles.x;
            if (startPitch > 180f)
                startPitch -= 360f;

            pitch = startPitch;
        }
    }

    void Update()
    {
        Look();
        Move();
    }

    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // IDENTICO A PRIMA:
        // destra/sinistra ruotano il Player
        transform.Rotate(0f, mouseX, 0f);

        // sopra/sotto ruotano solamente la Camera
        // SENZA alcun Clamp: può continuare oltre ±90° e fare giri completi.
        pitch -= mouseY;

        if (cameraTransform != null)
            cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void Move()
    {
        if (cameraTransform == null)
            return;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // W/S seguono esattamente la direzione in cui guarda la camera,
        // anche verso l'alto o verso il basso.
        Vector3 direction =
            cameraTransform.right * x +
            cameraTransform.forward * z;

        // Q/E restano disponibili come movimento verticale opzionale.
        if (Input.GetKey(KeyCode.E))
            direction += Vector3.up * (verticalSpeed / moveSpeed);

        if (Input.GetKey(KeyCode.Q))
            direction -= Vector3.up * (verticalSpeed / moveSpeed);

        if (direction.sqrMagnitude > 1f)
            direction.Normalize();

        controller.Move(direction * moveSpeed * Time.deltaTime);
    }
}
