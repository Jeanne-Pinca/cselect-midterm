using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelAndProgressUI : MonoBehaviour
{
    [Header("Level Counter")]
    [SerializeField] private TMP_Text levelCounterText;

    [Header("Fill Progress")]
    [SerializeField] private Slider fillProgressSlider;
    [SerializeField] private TMP_Text fillPercentText;
    [SerializeField] private int particlesToFill = 100;
    [SerializeField] private float fillGainMultiplier = 0.5f;

    [Header("Deterioration")]
    [SerializeField] private bool enableDeterioration = true;
    [SerializeField] private float deteriorationParticlesPerSecond = 2f;
    [SerializeField] private float deteriorationDelaySeconds = 0.75f;

    [Header("Completion")]
    [SerializeField] private Button completionButton;

    private float currentParticles;
    private float lastFillTime;
    private bool hasReachedFull;

    private void Awake()
    {
        SetCompletionVisible(false);
    }

    private void Start()
    {
        UpdateLevelCounter();
        ResetProgress();
    }

    private void Update()
    {
        ApplyDeterioration();
    }

    public void ResetProgress()
    {
        currentParticles = 0f;
        lastFillTime = Time.time;
        hasReachedFull = false;
        RefreshProgressUi();
    }

    public void AddParticles(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        if (fillGainMultiplier <= 0f)
        {
            return;
        }

        lastFillTime = Time.time;
        currentParticles = Mathf.Min(currentParticles + (amount * fillGainMultiplier), particlesToFill);
        RefreshProgressUi();
    }

    public void CompleteLevel()
    {
        currentParticles = Mathf.Max(0f, particlesToFill);
        lastFillTime = Time.time;
        RefreshProgressUi();
    }

    public float GetProgress01()
    {
        if (particlesToFill <= 0)
        {
            return 1f;
        }

        return currentParticles / particlesToFill;
    }

    private void ApplyDeterioration()
    {
        if (!enableDeterioration || particlesToFill <= 0 || currentParticles <= 0f || hasReachedFull)
        {
            return;
        }

        if (deteriorationParticlesPerSecond <= 0f)
        {
            return;
        }

        if (deteriorationDelaySeconds > 0f && (Time.time - lastFillTime) < deteriorationDelaySeconds)
        {
            return;
        }

        currentParticles = Mathf.Max(0f, currentParticles - (deteriorationParticlesPerSecond * Time.deltaTime));
        RefreshProgressUi();
    }

    private void UpdateLevelCounter()
    {
        if (levelCounterText == null)
        {
            return;
        }

        string sceneName = SceneManager.GetActiveScene().name;
        int trailingNumber = ExtractTrailingNumber(sceneName);

        levelCounterText.text = trailingNumber > 0 ? $"Level {trailingNumber}" : sceneName;
    }

    private void RefreshProgressUi()
    {
        float progress01 = GetProgress01();
        hasReachedFull = progress01 >= 1f;

        if (fillProgressSlider != null)
        {
            fillProgressSlider.minValue = 0f;
            fillProgressSlider.maxValue = 1f;
            fillProgressSlider.value = progress01;
        }

        if (fillPercentText != null)
        {
            fillPercentText.text = $"{Mathf.RoundToInt(progress01 * 100f)}%";
        }

        SetCompletionVisible(hasReachedFull);
    }

    private void SetCompletionVisible(bool isVisible)
    {
        if (completionButton != null)
        {
            completionButton.gameObject.SetActive(isVisible);
            completionButton.interactable = isVisible;
        }
    }

    private static int ExtractTrailingNumber(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return -1;
        }

        int end = text.Length - 1;
        while (end >= 0 && char.IsDigit(text[end]))
        {
            end--;
        }

        if (end == text.Length - 1)
        {
            return -1;
        }

        string numberPart = text.Substring(end + 1);
        return int.TryParse(numberPart, out int value) ? value : -1;
    }
}
