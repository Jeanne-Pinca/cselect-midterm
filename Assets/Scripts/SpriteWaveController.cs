using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteWaveController : MonoBehaviour
{
    [SerializeField] private float amplitude = 0.04f;
    [SerializeField] private float frequency = 5f;
    [SerializeField] private float speed = 1.3f;
    [SerializeField] private float softness = 0.010f;

    [Header("Per-Edge Frequency")]
    [SerializeField] private float topEdgeFrequencyMultiplier = 1f;
    [SerializeField] private float bottomEdgeFrequencyMultiplier = 1f;
    [SerializeField] private float leftEdgeFrequencyMultiplier = 1f;
    [SerializeField] private float rightEdgeFrequencyMultiplier = 1f;

    [SerializeField] private bool randomizePhase = true;

    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock propertyBlock;
    private float phaseOffset;

    private static readonly int AmplitudeId   = Shader.PropertyToID("_Amplitude");
    private static readonly int FrequencyId   = Shader.PropertyToID("_Frequency");
    private static readonly int SpeedId       = Shader.PropertyToID("_Speed");
    private static readonly int SoftnessId    = Shader.PropertyToID("_Softness");
    private static readonly int TopEdgeFrequencyMultiplierId = Shader.PropertyToID("_TopEdgeFrequencyMultiplier");
    private static readonly int BottomEdgeFrequencyMultiplierId = Shader.PropertyToID("_BottomEdgeFrequencyMultiplier");
    private static readonly int LeftEdgeFrequencyMultiplierId = Shader.PropertyToID("_LeftEdgeFrequencyMultiplier");
    private static readonly int RightEdgeFrequencyMultiplierId = Shader.PropertyToID("_RightEdgeFrequencyMultiplier");
    private static readonly int PhaseOffsetId = Shader.PropertyToID("_PhaseOffset");

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        propertyBlock = new MaterialPropertyBlock();
        phaseOffset = randomizePhase ? Random.Range(0f, Mathf.PI * 2f) : 0f;
        ApplyProperties();
    }

    private void OnValidate()
    {
        amplitude = Mathf.Max(0f, amplitude);
        frequency = Mathf.Max(0f, frequency);
        speed     = Mathf.Max(0f, speed);
        softness  = Mathf.Max(0.001f, softness);
        topEdgeFrequencyMultiplier = Mathf.Max(0f, topEdgeFrequencyMultiplier);
        bottomEdgeFrequencyMultiplier = Mathf.Max(0f, bottomEdgeFrequencyMultiplier);
        leftEdgeFrequencyMultiplier = Mathf.Max(0f, leftEdgeFrequencyMultiplier);
        rightEdgeFrequencyMultiplier = Mathf.Max(0f, rightEdgeFrequencyMultiplier);

        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (propertyBlock == null)  propertyBlock  = new MaterialPropertyBlock();
        ApplyProperties();
    }

    private void LateUpdate()
    {
        ApplyProperties();
    }

    private void ApplyProperties()
    {
        if (spriteRenderer == null) return;

        spriteRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat(AmplitudeId,   amplitude);
        propertyBlock.SetFloat(FrequencyId,   frequency);
        propertyBlock.SetFloat(SpeedId,       speed);
        propertyBlock.SetFloat(SoftnessId,    softness);
        propertyBlock.SetFloat(TopEdgeFrequencyMultiplierId, topEdgeFrequencyMultiplier);
        propertyBlock.SetFloat(BottomEdgeFrequencyMultiplierId, bottomEdgeFrequencyMultiplier);
        propertyBlock.SetFloat(LeftEdgeFrequencyMultiplierId, leftEdgeFrequencyMultiplier);
        propertyBlock.SetFloat(RightEdgeFrequencyMultiplierId, rightEdgeFrequencyMultiplier);
        propertyBlock.SetFloat(PhaseOffsetId, phaseOffset);
        spriteRenderer.SetPropertyBlock(propertyBlock);
    }
}
