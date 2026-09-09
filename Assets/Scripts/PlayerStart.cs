using UnityEngine;

public class PlayerStart : MonoBehaviour
{
    public Transform player;
    public Transform startPoint;

    [Tooltip("If enabled, the player will also copy the start point rotation.")]
    public bool copyRotation = true;

    void Start()
    {
        if (player == null || startPoint == null)
        {
            Debug.LogWarning("PlayerStart: assign Player and Start Point.");
            return;
        }

        CharacterController controller = player.GetComponent<CharacterController>();

        // CharacterController can interfere with teleporting, so disable it briefly.
        if (controller != null)
            controller.enabled = false;

        player.position = startPoint.position;

        if (copyRotation)
            player.rotation = startPoint.rotation;

        if (controller != null)
            controller.enabled = true;
    }
}
