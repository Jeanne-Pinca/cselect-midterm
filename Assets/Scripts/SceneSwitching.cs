using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitching : MonoBehaviour
{
    [Header("Direct Level Load")]
    [SerializeField] private string levelTwoSceneName = "Level2";
    [SerializeField] private int levelTwoBuildIndex = 2;

    [Header("Win Screen")]
    [SerializeField] private string winSceneName = "WinScreen";
    [SerializeField] private int winSceneBuildIndex = -1;

    [Header("Testing")]
    [SerializeField] private int lastPlayableLevelBuildIndex = -1;

    public void LoadLevelTwo()
    {
        if (!string.IsNullOrWhiteSpace(levelTwoSceneName) && Application.CanStreamedLevelBeLoaded(levelTwoSceneName))
        {
            SceneManager.LoadScene(levelTwoSceneName);
            return;
        }

        if (levelTwoBuildIndex >= 0 && levelTwoBuildIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(levelTwoBuildIndex);
            return;
        }

        Debug.LogError("Could not load Level2. Add Level2 to Build Settings or set a valid scene name/build index.", this);
    }

    public void LoadNextLevelByBuildIndex()
    {
        int currentBuildIndex = SceneManager.GetActiveScene().buildIndex;

        if (lastPlayableLevelBuildIndex >= 0 && currentBuildIndex >= lastPlayableLevelBuildIndex)
        {
            LoadWinScreen();
            return;
        }

        int nextBuildIndex = currentBuildIndex + 1;

        if (nextBuildIndex >= 0 && nextBuildIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextBuildIndex);
            return;
        }

        LoadWinScreen();
    }

    public void LoadWinScreen()
    {
        if (!string.IsNullOrWhiteSpace(winSceneName) && Application.CanStreamedLevelBeLoaded(winSceneName))
        {
            SceneManager.LoadScene(winSceneName);
            return;
        }

        if (winSceneBuildIndex >= 0 && winSceneBuildIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(winSceneBuildIndex);
            return;
        }

        Debug.LogWarning("No next level and no valid win scene found. Add WinScreen to Build Settings or set win scene fields.", this);
    }
}
