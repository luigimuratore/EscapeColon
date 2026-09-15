using System.Collections;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class Shooter : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public GameManager gameManager;
    public HitScorePopup scorePopup;

    [Header("Shooting")]
    public float range = 100f;
    public LayerMask hitMask = ~0;

    [Header("Gamepad")]
    [Range(0f, 1f)] public float r2FireThreshold = 0.5f;

    [Header("Laser")]
    public LineRenderer laserLine;
    public float laserDuration = 0.10f;
    public float laserStartOffset = 0.20f;

    [Header("Impact Effects")]
    public GameObject polypImpactEffectPrefab;
    public GameObject poopImpactEffectPrefab;
    public float impactEffectLifetime = 3f;

    [Header("Score Popup")]
    public int polypPopupPoints = 100;

    [Header("Audio")]
    public AudioSource shotAudioSource;
    public AudioClip shotSound;
    [Range(0f, 1f)] public float shotVolume = 1f;

    public AudioClip poopHitSound;
    [Range(0f, 1f)] public float poopHitVolume = 1f;

    private Coroutine laserCoroutine;
    private bool r2WasPressed = false;

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager>();

        if (laserLine != null)
        {
            laserLine.enabled = false;
            laserLine.positionCount = 2;
            laserLine.useWorldSpace = true;
        }
    }

    void Update()
    {
        bool shootPressed =
            Input.GetMouseButtonDown(0) ||
            Input.GetKeyDown(KeyCode.Space);

#if ENABLE_INPUT_SYSTEM
        Gamepad pad = Gamepad.current;

        if (pad != null)
        {
            float r2 = pad.rightTrigger.ReadValue();
            bool r2Pressed = r2 >= r2FireThreshold;

            // Fires once when R2 crosses the threshold.
            if (r2Pressed && !r2WasPressed)
                shootPressed = true;

            r2WasPressed = r2Pressed;
        }
        else
        {
            r2WasPressed = false;
        }
#endif

        if (shootPressed)
            Shoot();
    }

    void Shoot()
    {
        if (playerCamera == null)
            return;

        if (shotAudioSource != null && shotSound != null)
            shotAudioSource.PlayOneShot(shotSound, shotVolume);

        Vector3 rayOrigin = playerCamera.transform.position;
        Vector3 rayDirection = playerCamera.transform.forward;

        Vector3 laserStart = rayOrigin + rayDirection * laserStartOffset;
        Vector3 laserEnd = rayOrigin + rayDirection * range;

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, range, hitMask))
        {
            laserEnd = hit.point;

            Polyp polyp = hit.collider.GetComponentInParent<Polyp>();
            if (polyp != null)
            {
                SpawnImpactEffect(polypImpactEffectPrefab, polyp.transform.position);

                if (scorePopup != null)
                    scorePopup.ShowPositive(polypPopupPoints);

                polyp.Hit(gameManager);
            }
            else
            {
                Poop poop = hit.collider.GetComponentInParent<Poop>();

                if (poop != null)
                {
                    SpawnImpactEffect(poopImpactEffectPrefab, poop.transform.position);

                    if (shotAudioSource != null && poopHitSound != null)
                        shotAudioSource.PlayOneShot(poopHitSound, poopHitVolume);

                    if (scorePopup != null)
                        scorePopup.ShowNegative(poop.penaltyPoints);

                    poop.Hit(gameManager);
                }
            }
        }

        ShowLaser(laserStart, laserEnd);
    }

    void SpawnImpactEffect(GameObject prefab, Vector3 position)
    {
        if (prefab == null)
            return;

        GameObject effect = Instantiate(prefab, position, Quaternion.identity);

        ParticleSystem[] systems = effect.GetComponentsInChildren<ParticleSystem>(true);
        foreach (ParticleSystem ps in systems)
            ps.Play();

        Destroy(effect, impactEffectLifetime);
    }

    void ShowLaser(Vector3 start, Vector3 end)
    {
        if (laserLine == null)
            return;

        if (laserCoroutine != null)
            StopCoroutine(laserCoroutine);

        laserCoroutine = StartCoroutine(LaserRoutine(start, end));
    }

    IEnumerator LaserRoutine(Vector3 start, Vector3 end)
    {
        laserLine.enabled = true;
        laserLine.SetPosition(0, start);
        laserLine.SetPosition(1, end);

        yield return new WaitForSecondsRealtime(laserDuration);

        laserLine.enabled = false;
        laserCoroutine = null;
    }

    void OnDisable()
    {
        if (laserLine != null)
            laserLine.enabled = false;

        if (laserCoroutine != null)
        {
            StopCoroutine(laserCoroutine);
            laserCoroutine = null;
        }

        r2WasPressed = false;
    }
}
