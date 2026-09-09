using UnityEngine;

public class Poop : MonoBehaviour
{
    [Header("Penalty")]
    public int penaltyPoints = 50;

    [Tooltip("If ON, this object disappears after being hit once.")]
    public bool destroyOnHit = true;

    private bool alreadyHit = false;

    public void Hit(GameManager gameManager)
    {
        if (alreadyHit)
            return;

        alreadyHit = true;

        if (gameManager != null)
            gameManager.AddScore(-penaltyPoints);

        if (destroyOnHit)
            Destroy(gameObject);
    }
}
