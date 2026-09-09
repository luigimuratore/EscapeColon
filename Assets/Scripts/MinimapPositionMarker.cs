using UnityEngine;

public class MinimapPositionMarker : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform originalColon;
    public Transform miniColon;
    public Transform marker;

    [Header("Axis Correction")]
    public bool invertX = false;
    public bool invertY = false;
    public bool invertZ = false;

    [Header("Marker Offset")]
    public Vector3 markerWorldOffset = Vector3.zero;

    void LateUpdate()
    {
        if (player == null || originalColon == null || miniColon == null || marker == null)
            return;

        // Player position relative to the original colon.
        Vector3 localPositionInColon =
            originalColon.InverseTransformPoint(player.position);

        // Optional corrections if the miniature is mirrored/rotated
        // compared with the original colon.
        if (invertX)
            localPositionInColon.x *= -1f;

        if (invertY)
            localPositionInColon.y *= -1f;

        if (invertZ)
            localPositionInColon.z *= -1f;

        // Convert the corrected local position onto the miniature colon.
        marker.position =
            miniColon.TransformPoint(localPositionInColon)
            + markerWorldOffset;
    }
}
