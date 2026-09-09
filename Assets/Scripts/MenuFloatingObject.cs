using UnityEngine;

public class MenuFloatingObject : MonoBehaviour
{
    [Header("Movement")]
    public Vector3 moveDirection = new Vector3(1f, 0.25f, 0f);
    public float moveSpeed = 0.8f;

    [Header("Rotation")]
    public Vector3 rotationSpeed = new Vector3(15f, 25f, 10f);

    [Header("Local Movement Area")]
    public float horizontalLimit = 8f;
    public float verticalLimit = 4.5f;

    [Tooltip("When the object exits one side, it reappears on the opposite side.")]
    public bool wrapAround = true;

    private Vector3 startLocalPosition;

    void Start()
    {
        startLocalPosition = transform.localPosition;

        if (moveDirection.sqrMagnitude > 0f)
            moveDirection.Normalize();
    }

    void Update()
    {
        // Unscaled time is important because the Start Menu uses Time.timeScale = 0.
        float dt = Time.unscaledDeltaTime;

        transform.localPosition += moveDirection * moveSpeed * dt;
        transform.Rotate(rotationSpeed * dt, Space.World);

        if (wrapAround)
            WrapPosition();
    }

    void WrapPosition()
    {
        Vector3 pos = transform.localPosition;

        float minX = startLocalPosition.x - horizontalLimit;
        float maxX = startLocalPosition.x + horizontalLimit;
        float minY = startLocalPosition.y - verticalLimit;
        float maxY = startLocalPosition.y + verticalLimit;

        if (pos.x > maxX)
            pos.x = minX;
        else if (pos.x < minX)
            pos.x = maxX;

        if (pos.y > maxY)
            pos.y = minY;
        else if (pos.y < minY)
            pos.y = maxY;

        transform.localPosition = pos;
    }
}
