using System.Collections.Generic;
using UnityEngine;

public class InteriorPolypSpawner : MonoBehaviour
{
    [Header("Colon")]
    public MeshCollider colonCollider;

    [Header("Prefab")]
    public GameObject polypPrefab;

    [Header("Centerline / lumen points")]
    [Tooltip("Place these empty GameObjects INSIDE the colon, in order from start to end.")]
    public Transform[] lumenPoints;

    [Header("Generation")]
    [Range(1, 100)]
    public int polypCount = 25;

    public int randomSeed = 12345;

    [Tooltip("Distance from centerline used for the raycast.")]
    public float rayDistance = 1000f;

    [Tooltip("Moves the polyp slightly INTO the lumen after hitting the wall.")]
    public float inwardOffset = 0.15f;

    [Tooltip("Minimum separation between generated polyps.")]
    public float minDistanceBetweenPolyps = 1.0f;

    [Tooltip("Max random attempts before giving up.")]
    public int maxAttempts = 2000;

    [Header("Random Size")]
    [Tooltip("Minimum scale multiplier relative to the prefab size.")]
    public float minScale = 0.6f;

    [Tooltip("Maximum scale multiplier relative to the prefab size.")]
    public float maxScale = 1.8f;

    [Header("Orientation")]
    [Tooltip("Assumes the polyp grows along its local +Y axis.")]
    public bool alignToWall = true;

    private const string GeneratedRootName = "GeneratedPolyps";

    [ContextMenu("Generate Interior Polyps")]
    public void GenerateInteriorPolyps()
    {
        if (colonCollider == null)
        {
            Debug.LogError("Assign the colon MeshCollider.");
            return;
        }

        if (polypPrefab == null)
        {
            Debug.LogError("Assign the polyp prefab.");
            return;
        }

        if (lumenPoints == null || lumenPoints.Length < 2)
        {
            Debug.LogError("Add at least 2 lumen points inside the colon.");
            return;
        }

        ClearGeneratedPolyps();

        Random.InitState(randomSeed);

        GameObject root = new GameObject(GeneratedRootName);
        root.transform.SetParent(transform, false);

        List<Vector3> generatedPositions = new List<Vector3>();

        int generated = 0;
        int attempts = 0;

        while (generated < polypCount && attempts < maxAttempts)
        {
            attempts++;

            // Pick a random segment of the centerline.
            int segment = Random.Range(0, lumenPoints.Length - 1);

            Transform a = lumenPoints[segment];
            Transform b = lumenPoints[segment + 1];

            if (a == null || b == null)
                continue;

            float t = Random.value;
            Vector3 centerPoint = Vector3.Lerp(a.position, b.position, t);

            // Direction of the colon at this section.
            Vector3 forward = (b.position - a.position).normalized;

            if (forward.sqrMagnitude < 0.001f)
                continue;

            // Build a local radial plane perpendicular to the colon direction.
            Vector3 reference = Mathf.Abs(Vector3.Dot(forward, Vector3.up)) > 0.95f
                ? Vector3.right
                : Vector3.up;

            Vector3 right = Vector3.Cross(forward, reference).normalized;
            Vector3 up = Vector3.Cross(right, forward).normalized;

            // Random radial direction around the lumen.
            float angle = Random.Range(0f, Mathf.PI * 2f);
            Vector3 radialDirection =
                (right * Mathf.Cos(angle) + up * Mathf.Sin(angle)).normalized;

            Ray ray = new Ray(centerPoint, radialDirection);

            if (!colonCollider.Raycast(ray, out RaycastHit hit, rayDistance))
                continue;

            // Move slightly away from the wall, back toward the lumen.
            Vector3 spawnPosition = hit.point - radialDirection * inwardOffset;

            bool tooClose = false;

            foreach (Vector3 existing in generatedPositions)
            {
                if (Vector3.Distance(spawnPosition, existing) < minDistanceBetweenPolyps)
                {
                    tooClose = true;
                    break;
                }
            }

            if (tooClose)
                continue;

            Quaternion rotation = Quaternion.identity;

            if (alignToWall)
            {
                // +Y of polyp points away from wall and toward lumen.
                rotation = Quaternion.FromToRotation(Vector3.up, -radialDirection);
            }

            GameObject polyp = Instantiate(
                polypPrefab,
                spawnPosition,
                rotation,
                root.transform
            );

            // Random uniform scale: preserves the original polyp shape.
            float randomScale = Random.Range(minScale, maxScale);
            polyp.transform.localScale *= randomScale;

            polyp.name = $"Polyp_{generated + 1:00}_Scale_{randomScale:F2}";
            generatedPositions.Add(spawnPosition);
            generated++;
        }

        Debug.Log(
            $"Generated {generated}/{polypCount} interior polyps after {attempts} attempts."
        );
    }

    [ContextMenu("Clear Generated Polyps")]
    public void ClearGeneratedPolyps()
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
