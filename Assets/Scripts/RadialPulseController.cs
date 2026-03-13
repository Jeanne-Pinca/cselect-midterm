using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
public class RadialPulseController : MonoBehaviour
{
    [Header("Material Setup")]
    [SerializeField] private bool autoAssignPulseShader = true;
    [SerializeField] private Shader pulseShader;

    [Header("Pulse Look")]
    [SerializeField] private Color pulseColor = Color.white;
    [SerializeField, Range(0f, 2f)] private float pulseIntensity = 1f;
    [SerializeField, Range(0.25f, 12f)] private float pulseCount = 2f;
    [SerializeField, Range(0.001f, 0.5f)] private float pulseWidth = 0.08f;
    [SerializeField, Range(0.001f, 0.25f)] private float pulseSoftness = 0.03f;
    [SerializeField, Range(0f, 6f)] private float pulseSpeed = 1.2f;
    [SerializeField] private bool randomizePhase = true;

    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock propertyBlock;
    private Material runtimeMaterial;
    private float phaseOffset;

    private static readonly int PulseColorId = Shader.PropertyToID("_PulseColor");
    private static readonly int PulseIntensityId = Shader.PropertyToID("_PulseIntensity");
    private static readonly int PulseCountId = Shader.PropertyToID("_PulseCount");
    private static readonly int PulseWidthId = Shader.PropertyToID("_PulseWidth");
    private static readonly int PulseSoftnessId = Shader.PropertyToID("_PulseSoftness");
    private static readonly int PulseSpeedId = Shader.PropertyToID("_PulseSpeed");
    private static readonly int PhaseOffsetId = Shader.PropertyToID("_PhaseOffset");

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        propertyBlock = new MaterialPropertyBlock();
        phaseOffset = randomizePhase ? Random.Range(0f, Mathf.PI * 2f) : 0f;

        TryAssignMaterial();
        ApplyProperties();
    }

    private void OnValidate()
    {
        pulseIntensity = Mathf.Max(0f, pulseIntensity);
        pulseCount = Mathf.Max(0.25f, pulseCount);
        pulseWidth = Mathf.Max(0.001f, pulseWidth);
        pulseSoftness = Mathf.Max(0.001f, pulseSoftness);
        pulseSpeed = Mathf.Max(0f, pulseSpeed);

        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (propertyBlock == null) propertyBlock = new MaterialPropertyBlock();

        TryAssignMaterial();
        ApplyProperties();
    }

    private void LateUpdate()
    {
        ApplyProperties();
    }

    private void OnDestroy()
    {
        if (runtimeMaterial == null) return;

        if (Application.isPlaying)
            Destroy(runtimeMaterial);
        else
            DestroyImmediate(runtimeMaterial);
    }

    private void TryAssignMaterial()
    {
        if (!autoAssignPulseShader || spriteRenderer == null) return;

        Shader shaderToUse = pulseShader != null ? pulseShader : Shader.Find("Custom/SpriteRadialPulse");
        if (shaderToUse == null) return;

        if (runtimeMaterial == null || runtimeMaterial.shader != shaderToUse)
        {
            if (runtimeMaterial != null)
            {
                if (Application.isPlaying)
                    Destroy(runtimeMaterial);
                else
                    DestroyImmediate(runtimeMaterial);
            }

            runtimeMaterial = new Material(shaderToUse)
            {
                name = "Runtime Sprite Radial Pulse"
            };
        }

        if (spriteRenderer.sharedMaterial == null || spriteRenderer.sharedMaterial.shader != shaderToUse)
            spriteRenderer.sharedMaterial = runtimeMaterial;
    }

    private void ApplyProperties()
    {
        if (spriteRenderer == null) return;

        spriteRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor(PulseColorId, pulseColor);
        propertyBlock.SetFloat(PulseIntensityId, pulseIntensity);
        propertyBlock.SetFloat(PulseCountId, pulseCount);
        propertyBlock.SetFloat(PulseWidthId, pulseWidth);
        propertyBlock.SetFloat(PulseSoftnessId, pulseSoftness);
        propertyBlock.SetFloat(PulseSpeedId, pulseSpeed);
        propertyBlock.SetFloat(PhaseOffsetId, phaseOffset);
        spriteRenderer.SetPropertyBlock(propertyBlock);
    }
}