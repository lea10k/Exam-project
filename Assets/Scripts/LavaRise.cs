using UnityEngine;

// *old wizard voice* MAKE IT RISE
public class LavaRise : MonoBehaviour
{
    public float baseSpeed = 0.01f; // starting speed
    //public float growthRate = 0.1f; // how fast the speed grows
    //public float gameOverHeight = 10f; // Height at which the game ends
    public Transform player; // Reference to the player

    //private float timeElapsed = 0f;
    private PlayerHealth playerHealth;
    private PlayerScore playerScore;

    void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
        playerScore = FindObjectOfType<PlayerScore>();

        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth component not found in the scene!");
        }

        if (playerScore == null)
        {
            Debug.LogError("PlayerScore component not found in the scene!");
        }
    }

    void Update()
    {
        //timeElapsed += Time.deltaTime;

        // Adjust speed based on player's score
        float score = playerScore != null ? Mathf.RoundToInt(playerScore.highestY) : 0;
        float speedMultiplier = 1f;

        if (score < 100)
        {
            speedMultiplier = 0.5f; // Slow
        }
        else if (score < 250)
        {
            speedMultiplier = 1f; // Medium
        }
        else if (score < 500)
        {
            speedMultiplier = 1.5f; // Fast
        }
        else
        {
            speedMultiplier = 2f; // Very fast
        }
        // old system of lava going up faster over time
        // Speed grows exponentially over time and is adjusted by the multiplier
        //float currentSpeed = baseSpeed * Mathf.Exp(growthRate * timeElapsed) * speedMultiplier;

        // Move the lava up
        //transform.position += Vector3.up * currentSpeed * Time.deltaTime;
        transform.position += Vector3.up * (baseSpeed * speedMultiplier);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1); // Reduce player's health by 1
            }

            // Grant trampoline power-up
            var trampoline = collision.gameObject.GetComponent<TrampolineJump>();
            var movement = collision.gameObject.GetComponent<PlayerMovement>();

            if (movement != null && trampoline == null)
            {
                trampoline = collision.gameObject.AddComponent<TrampolineJump>();
                trampoline.Initialize(movement);
                trampoline.Jump(40f); // Example jump height
                Debug.Log("Trampoline power-up granted due to lava collision.");
            }
        }
    }
}