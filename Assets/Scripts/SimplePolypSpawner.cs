using UnityEngine;

public class SimplePolypSpawner : MonoBehaviour
{
    public GameObject polypPrefab;
    public Transform[] spawnPoints;
    public int numberToSpawn = 10;

    void Start()
    {
        if (polypPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
            return;

        int count = Mathf.Min(numberToSpawn, spawnPoints.Length);

        // Shuffle spawn point indices.
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            int j = Random.Range(i, spawnPoints.Length);
            (spawnPoints[i], spawnPoints[j]) = (spawnPoints[j], spawnPoints[i]);
        }

        for (int i = 0; i < count; i++)
        {
            Transform p = spawnPoints[i];
            Instantiate(polypPrefab, p.position, p.rotation);
        }
    }
}
