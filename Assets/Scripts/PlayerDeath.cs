using UnityEngine;
using UnityEngine.SceneManagement;

// MAKE IT DIE (<3 on the pinguino)
public class PlayerLavaDeath : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Lava")) // tag of the object that represent the lava
        {
            // go to the menu scene :D (this is temporary since it would be better to display some kind of score and game over stuff)
            SceneManager.LoadScene("MainMenu");
        }
    }
}
