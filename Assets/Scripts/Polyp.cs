using UnityEngine;

public class Polyp : MonoBehaviour
{
    public int points = 100;
    public int hitsToDestroy = 1;

    private int currentHits = 0;
    private bool destroyedAlready = false;

    public void Hit(GameManager gameManager)
    {
        if (destroyedAlready) return;

        currentHits++;

        if (currentHits >= hitsToDestroy)
        {
            destroyedAlready = true;

            if (gameManager != null)
                gameManager.PolypDestroyed(points);

            Destroy(gameObject);
        }
    }
}
