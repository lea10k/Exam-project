using UnityEngine;

// *old wizard voice* MAKE IT RISE
public class LavaRise : MonoBehaviour
{
    public float baseSpeed = 0.5f; // starting speed
    public float growthRate = 0.1f; // how fast the speed grows
    public float gameOverHeight = 10f; // Height at which the game ends
    public Transform player; // Reference to the player

    private float timeElapsed = 0f;

    void Update()
    {
        timeElapsed += Time.deltaTime;

        // Speed grows exponentially over time
        float currentSpeed = baseSpeed * Mathf.Exp(growthRate * timeElapsed);

        // Move the lava up
        transform.position += Vector3.up * currentSpeed * Time.deltaTime;

    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            FindObjectOfType<GameOverManager>().TriggerGameOver(); // Trigger the game over
        }
}
}