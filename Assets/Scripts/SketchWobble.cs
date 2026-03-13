using UnityEngine;

public class SketchWobble : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform visualTarget;

    [Header("Position Wobble")]
    [SerializeField] private bool affectPosition = false;
    [SerializeField] private float positionAmplitude = 0.02f;
    [SerializeField] private float positionFrequency = 1.2f;

    [Header("Rotation Wobble")]
    [SerializeField] private bool affectRotation = true;
    [SerializeField] private float rotationAmplitudeDegrees = 1.2f;
    [SerializeField] private float rotationFrequency = 1.4f;

    [Header("Scale Wobble")]
    [SerializeField] private bool affectScale = true;
    [SerializeField] private float scaleAmplitudePercent = 0.03f;
    [SerializeField] private float scaleFrequency = 1.3f;

    [Header("Axes")]
    [SerializeField] private bool wobbleX = true;
    [SerializeField] private bool wobbleY = true;
    [SerializeField] private bool wobbleZRotation = true;

    private Vector3 baseLocalPosition;
    private Quaternion baseLocalRotation;
    private Vector3 baseLocalScale;
    private float phaseOffset;

    private void Awake()
    {
        EnsureTarget();
        CacheBaseTransform();
        phaseOffset = Random.Range(0f, 1000f);
    }

    private void OnEnable()
    {
        EnsureTarget();
        CacheBaseTransform();
    }

    private void Update()
    {
        if (visualTarget == null)
        {
            return;
        }

        float t = Time.time + phaseOffset;

        float posNoiseA = Mathf.PerlinNoise(t * positionFrequency, 0.13f) - 0.5f;
        float posNoiseB = Mathf.PerlinNoise(0.71f, t * positionFrequency) - 0.5f;
        float rotNoise = Mathf.PerlinNoise(t * rotationFrequency, 0.37f) - 0.5f;
        float scaleNoise = Mathf.PerlinNoise(t * scaleFrequency, 0.91f) - 0.5f;

        if (affectPosition)
        {
            float xOffset = wobbleX ? posNoiseA * 2f * positionAmplitude : 0f;
            float yOffset = wobbleY ? posNoiseB * 2f * positionAmplitude : 0f;
            visualTarget.localPosition = baseLocalPosition + new Vector3(xOffset, yOffset, 0f);
        }
        else
        {
            visualTarget.localPosition = baseLocalPosition;
        }

        if (affectRotation)
        {
            float zAngle = wobbleZRotation ? rotNoise * 2f * rotationAmplitudeDegrees : 0f;
            visualTarget.localRotation = baseLocalRotation * Quaternion.Euler(0f, 0f, zAngle);
        }
        else
        {
            visualTarget.localRotation = baseLocalRotation;
        }

        if (affectScale)
        {
            float scaleDelta = scaleNoise * 2f * scaleAmplitudePercent;
            float xScale = wobbleX ? baseLocalScale.x * (1f + scaleDelta) : baseLocalScale.x;
            float yScale = wobbleY ? baseLocalScale.y * (1f - scaleDelta) : baseLocalScale.y;
            visualTarget.localScale = new Vector3(xScale, yScale, baseLocalScale.z);
        }
        else
        {
            visualTarget.localScale = baseLocalScale;
        }
    }

    private void CacheBaseTransform()
    {
        if (visualTarget == null)
        {
            return;
        }

        baseLocalPosition = visualTarget.localPosition;
        baseLocalRotation = visualTarget.localRotation;
        baseLocalScale = visualTarget.localScale;
    }

    private void EnsureTarget()
    {
        if (visualTarget == null)
        {
            visualTarget = transform;
        }
    }
}
