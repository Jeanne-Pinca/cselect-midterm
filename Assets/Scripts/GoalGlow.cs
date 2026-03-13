using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
public class GoalGlow : MonoBehaviour
{
    [Header("Pulse")]
    [SerializeField] private Color baseColor = Color.white;
    [SerializeField] private Color glowColor = new Color(1f, 0.95f, 0.6f, 1f);
    [SerializeField, Range(0.1f, 8f)] private float pulseSpeed = 2.2f;
    [SerializeField, Range(0f, 1f)] private float glowStrength = 0.6f;

    [Header("Halo")]
    [SerializeField, Range(1f, 1.8f)] private float haloMaxScale = 1.25f;
    [SerializeField, Range(0f, 1f)] private float haloMaxAlpha = 0.35f;

    private SpriteRenderer spriteRenderer;
    private SpriteRenderer haloRenderer;
    private Transform haloTransform;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        EnsureHalo();
    }

    private void Update()
    {
        if (spriteRenderer == null)
            return;

        EnsureHalo();

        float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
        float glowAmount = pulse * glowStrength;

        spriteRenderer.color = Color.Lerp(baseColor, glowColor, glowAmount);

        if (haloRenderer == null || haloTransform == null)
            return;

        if (haloRenderer.sprite != spriteRenderer.sprite)
            haloRenderer.sprite = spriteRenderer.sprite;

        Color haloColor = glowColor;
        haloColor.a = pulse * haloMaxAlpha;
        haloRenderer.color = haloColor;

        float haloScale = Mathf.Lerp(1f, haloMaxScale, pulse);
        haloTransform.localScale = new Vector3(haloScale, haloScale, 1f);
    }

    private void EnsureHalo()
    {
        if (haloRenderer != null && haloTransform != null)
            return;

        Transform existing = transform.Find("GoalGlowHalo");
        if (existing == null)
        {
            GameObject haloObject = new GameObject("GoalGlowHalo");
            haloObject.transform.SetParent(transform, false);
            existing = haloObject.transform;
        }

        haloTransform = existing;
        haloRenderer = existing.GetComponent<SpriteRenderer>();
        if (haloRenderer == null)
            haloRenderer = existing.gameObject.AddComponent<SpriteRenderer>();

        haloRenderer.sprite = spriteRenderer != null ? spriteRenderer.sprite : null;
        haloRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
        haloRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;
        haloRenderer.maskInteraction = spriteRenderer.maskInteraction;
    }
}
