using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
public class ProgressRevealSprite : MonoBehaviour
{
    [Header("Progress Source")]
    [SerializeField] private LevelAndProgressUI levelAndProgressUI;

    [Header("Reveal")]
    [SerializeField, Range(0f, 1f)] private float hiddenAlpha = 0f;
    [SerializeField, Range(0f, 1f)] private float visibleAlpha = 1f;
    [SerializeField, Min(0f)] private float fadeSpeed = 1.5f;

    [Header("Background")]
    [SerializeField] private bool forceBehindLevel = true;
    [SerializeField] private int backgroundSortingOrder = -10;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (levelAndProgressUI == null)
            levelAndProgressUI = FindObjectOfType<LevelAndProgressUI>();

        if (forceBehindLevel && spriteRenderer != null)
            spriteRenderer.sortingOrder = backgroundSortingOrder;

        ApplyAlpha(hiddenAlpha);
    }

    private void Update()
    {
        if (spriteRenderer == null)
            return;

        if (levelAndProgressUI == null)
            levelAndProgressUI = FindObjectOfType<LevelAndProgressUI>();

        float progress = levelAndProgressUI != null ? levelAndProgressUI.GetProgress01() : 0f;
        float targetAlpha = Mathf.Lerp(hiddenAlpha, visibleAlpha, progress);
        float nextAlpha = Mathf.MoveTowards(spriteRenderer.color.a, targetAlpha, fadeSpeed * Time.deltaTime);
        ApplyAlpha(nextAlpha);
    }

    private void ApplyAlpha(float alpha)
    {
        if (spriteRenderer == null)
            return;

        Color color = spriteRenderer.color;
        color.a = Mathf.Clamp01(alpha);
        spriteRenderer.color = color;
    }
}
