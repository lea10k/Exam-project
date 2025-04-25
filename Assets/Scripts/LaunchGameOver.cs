using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro; // Import TextMeshPro namespace

public class GameOverManager : MonoBehaviour
{
    public GameObject postProcessingVolume; // Assign the Post-Processing Volume in the Inspector
    public GameObject gameOverCanvas; // Assign the GameOver Canvas GameObject in the Inspector
    public GameObject scoreTextObject; // Assign the Score Text GameObject in the Inspector
    public Button replayButton; // Assign the Replay Button in the Inspector
    public Button mainMenuButton; // Assign the Main Menu Button in the Inspector
    private bool isGameOver = false;

    public void TriggerGameOver()
    {
        if (isGameOver) return;

        isGameOver = true;

        // Enable grayscale effect
        if (postProcessingVolume != null)
        {
            postProcessingVolume.SetActive(true);
        }
        else
        {
            Debug.LogWarning("PostProcessingVolume is not assigned in the Inspector!");
        }

        // Start a coroutine to delay the display of the GameOver Canvas
        StartCoroutine(DisplayGameOverUIWithDelay(1.0f)); // Replace 1.0f with the desired delay in seconds

        // Disable player input
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Debug.Log("Player found!");

            // Disable PlayerMovement script
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
                Debug.Log("PlayerMovement script disabled!");
            }
            else
            {
                Debug.LogWarning("PlayerMovement script not found on the Player GameObject!");
            }
        }
        else
        {
            Debug.LogWarning("Player GameObject not found!");
        }
    }

    private System.Collections.IEnumerator DisplayGameOverUIWithDelay( float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSecondsRealtime(delay);

        // Enable the GameOver Canvas
        if (gameOverCanvas != null)
        {
            Debug.Log("Enabling GameOver Canvas");
            gameOverCanvas.SetActive(true);
        }
        else
        {
            Debug.LogWarning("GameOverCanvas is not assigned in the Inspector!");
        }

        // Replace the Score Text content with the content of another object
        if (scoreTextObject != null)
        {
            Debug.Log("Activating Score Text");

            // Fetch the text from another object (replace "SourceTextObject" with the actual object name)
            GameObject sourceTextObject = GameObject.Find("score txt"); // Replace with the actual object name
            if (sourceTextObject != null)
            {
                TextMeshProUGUI sourceText = sourceTextObject.GetComponent<TextMeshProUGUI>();
                if (sourceText != null)
                {
                    TextMeshProUGUI scoreText = scoreTextObject.GetComponent<TextMeshProUGUI>();
                    scoreText.text = sourceText.text; // Replace the content of the score text with the content of the source text

                    // Make the sourceTextObject disappear
                    sourceTextObject.SetActive(false);
                    Debug.Log("SourceTextObject has been hidden.");
                }
                else
                {
                    Debug.LogWarning("SourceTextObject does not have a TextMeshProUGUI component!");
                }
            }
            else
            {
                Debug.LogWarning("SourceTextObject not found in the scene!");
            }
        }
        else
        {
            Debug.LogWarning("ScoreTextObject is not assigned in the Inspector!");
        }

        // Show buttons
        if (replayButton != null && mainMenuButton != null)
        {
            Debug.Log("Activating Buttons");
            replayButton.gameObject.SetActive(true);
            mainMenuButton.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("ReplayButton or MainMenuButton is not assigned in the Inspector!");
        }

        // Freeze the game AFTER the delay
        Time.timeScale = 0;

        // Add button listeners
        replayButton.onClick.AddListener(ReplayGame);
        mainMenuButton.onClick.AddListener(GoToMainMenu);
    }

    private void ReplayGame()
    {
        // Reset time scale and reload the current scene
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void GoToMainMenu()
    {
        // Reset time scale and load the main menu scene
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu"); // Replace "MainMenu" with your main menu scene name
    }
}