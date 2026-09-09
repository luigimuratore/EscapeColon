using System.Collections.Generic;
using UnityEngine;

public class InteriorPoopSpawner : MonoBehaviour
{
    [Header("Colon")]
    public MeshCollider colonCollider;

    [Header("Prefab")]
    public GameObject poopPrefab;

    [Header("Centerline / lumen points")]
    public Transform[] lumenPoints;

    [Header("Generation")]
    public int poopCount = 10;
    public int randomSeed = 7777;

    [Tooltip("Maximum raycast distance from the lumen centerline.")]
    public float rayDistance = 1000f;

    [Tooltip("How far the poop is moved away from the wall and into the lumen.")]
    public float inwardOffset = 0.8f;

    [Tooltip("Minimum spacing between poop objects.")]
    public float minDistanceBetweenPoop = 1.5f;

    public int maxAttempts = 2000;

    [Header("Random Size")]
    public float minScale = 0.7f;
    public float maxScale = 1.4f;

    [Header("Orientation")]
    public bool alignToWall = true;

    private const string GeneratedRootName = "GeneratedPoop";

    [ContextMenu("Generate Interior Poop")]
    public void GenerateInteriorPoop()
    {
        if (colonCollider == null)
        {
            Debug.LogError("Assign the colon MeshCollider.");
            return;
        }

        if (poopPrefab == null)
        {
            Debug.LogError("Assign the poop prefab.");
            return;
        }

        if (lumenPoints == null || lumenPoints.Length < 2)
        {
            Debug.LogError("Add at least 2 lumen points.");
            return;
        }

        ClearGeneratedPoop();

        Random.InitState(randomSeed);

        GameObject root = new GameObject(GeneratedRootName);
        root.transform.SetParent(transform, false);

        List<Vector3> generatedPositions = new List<Vector3>();

        int generated = 0;
        int attempts = 0;

        while (generated < poopCount && attempts < maxAttempts)
        {
            attempts++;

            int segment = Random.Range(0, lumenPoints.Length - 1);

            Transform a = lumenPoints[segment];
            Transform b = lumenPoints[segment + 1];

            if (a == null || b == null)
                continue;

            float t = Random.value;
            Vector3 centerPoint = Vector3.Lerp(a.position, b.position, t);

            Vector3 forward = (b.position - a.position).normalized;

            if (forward.sqrMagnitude < 0.001f)
                continue;

            Vector3 reference =
                Mathf.Abs(Vector3.Dot(forward, Vector3.up)) > 0.95f
                ? Vector3.right
                : Vector3.up;

            Vector3 right = Vector3.Cross(forward, reference).normalized;
            Vector3 up = Vector3.Cross(right, forward).normalized;

            float angle = Random.Range(0f, Mathf.PI * 2f);

            Vector3 radialDirection =
                (right * Mathf.Cos(angle) +
                 up * Mathf.Sin(angle)).normalized;

            Ray ray = new Ray(centerPoint, radialDirection);

            if (!colonCollider.Raycast(ray, out RaycastHit hit, rayDistance))
                continue;

            // Keep the object visibly inside the lumen rather than embedded in the wall.
            Vector3 spawnPosition =
                hit.point - radialDirection * inwardOffset;

            bool tooClose = false;

            foreach (Vector3 existing in generatedPositions)
            {
                if (Vector3.Distance(spawnPosition, existing) < minDistanceBetweenPoop)
                {
                    tooClose = true;
                    break;
                }
            }

            if (tooClose)
                continue;

            Quaternion rotation = Quaternion.identity;

            if (alignToWall)
                rotation = Quaternion.FromToRotation(Vector3.up, -radialDirection);

            GameObject poop = Instantiate(
                poopPrefab,
                spawnPosition,
                rotation,
                root.transform
            );

            float randomScale = Random.Range(minScale, maxScale);
            poop.transform.localScale *= randomScale;

            // Extra random spin so identical models don't all look the same.
            poop.transform.Rotate(
                Random.Range(0f, 360f),
                Random.Range(0f, 360f),
                Random.Range(0f, 360f),
                Space.Self
            );

            poop.name = $"Poop_{generated + 1:00}_Scale_{randomScale:F2}";

            generatedPositions.Add(spawnPosition);
            generated++;
        }

        Debug.Log($"Generated {generated}/{poopCount} poop objects after {attempts} attempts.");
    }

    [ContextMenu("Clear Generated Poop")]
    public void ClearGeneratedPoop()
    {
        Transform existing = transform.Find(GeneratedRootName);

        if (existing == null)
            return;

        if (Application.isPlaying)
            Destroy(existing.gameObject);
        else
            DestroyImmediate(existing.gameObject);
    }
}
