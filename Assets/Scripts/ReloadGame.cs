using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Simple helper to reload the current gameplay scene. Attach this script to a UI
/// button and wire up the ReloadCurrentScene method in the Button's OnClick list.
/// Optionally the scene can be reloaded by pressing the 'R' key.
/// </summary>
public class ReloadGame : MonoBehaviour
{
    void Update()
    {
        // Allow keyboard shortcut for quick testing while playing
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadCurrentScene();
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
