using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void LaunchGame()
    {
        SceneManager.LoadSceneAsync("GameplayScene");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
