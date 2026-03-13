using UnityEngine;
using UnityEngine.SceneManagement;

public static class ProjectCursor
{
    private const string CursorResourceName = "cursor";
    private static readonly Vector2 CursorHotspot = new Vector2(2f, 2f);

    private static Texture2D cursorTexture;
    private static bool hasLoggedUnreadableWarning;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        SceneManager.sceneLoaded += HandleSceneLoaded;

        Application.focusChanged -= HandleFocusChanged;
        Application.focusChanged += HandleFocusChanged;

        LoadCursorTexture();
        ApplyCursor();
    }

    private static void HandleSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        ApplyCursor();
    }

    private static void HandleFocusChanged(bool hasFocus)
    {
        if (hasFocus)
            ApplyCursor();
    }

    private static void LoadCursorTexture()
    {
        if (cursorTexture != null)
            return;

        cursorTexture = Resources.Load<Texture2D>(CursorResourceName);

        if (cursorTexture == null)
            Debug.LogWarning("Project cursor texture not found at Assets/Resources/cursor.png");
    }

    private static void ApplyCursor()
    {
        if (cursorTexture == null)
            return;

        if (!cursorTexture.isReadable)
        {
            if (!hasLoggedUnreadableWarning)
            {
                hasLoggedUnreadableWarning = true;
                Debug.LogWarning("Cursor texture is not CPU-readable. Enable Read/Write in the texture import settings.");
            }
            return;
        }

        Cursor.visible = true;
        Cursor.SetCursor(cursorTexture, CursorHotspot, CursorMode.Auto);
    }
}