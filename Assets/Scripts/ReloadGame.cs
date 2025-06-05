using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Simple helper to reload the current gameplay scene. Attach this script to a UI
/// button and wire up the ReloadCurrentScene method in the Button's OnClick list.
/// Optionally the scene can be reloaded by pressing the 'R' key.
/// </summary>
public class ReloadGame : MonoBehaviour

    private GUIStyle promptStyle;

    void Start()
    {
        // Initialize style for the reload hint text
        promptStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 18,
            alignment = TextAnchor.LowerCenter,
            normal = { textColor = Color.white }
        };
    }

    void Update()
    {
        // Allow keyboard shortcut for quick testing while playing
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadCurrentScene();
        }
    }

    void OnGUI()
    {
        if (promptStyle != null)
        {
            Rect rect = new Rect(0, Screen.height - 30, Screen.width, 30);
            GUI.Label(rect, "Press 'R' to reload level", promptStyle);
        }
    }


    /// <summary>
    /// Reloads the active scene and resets time scale.
    /// </summary>
    public void ReloadCurrentScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
