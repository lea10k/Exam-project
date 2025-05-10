using UnityEngine;

// *old wizard voice* MAKE IT RISE
public class LavaRise : MonoBehaviour
{
    public float baseSpeed = 0.01f; // starting speed
    public Transform player; // Reference to the player

    private PlayerHealth playerHealth;
    private PlayerScore playerScore;

    // Add these lines:
    private float pauseTimer = 0f;
    public float pauseDuration = 0.5f; // About 30 frames at 60fps
    private bool hasStarted = false;

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
        if (!hasStarted)
        {
            if (Input.anyKeyDown) // Or check for specific input, e.g. Input.GetButtonDown("Jump")
            {
                hasStarted = true;
            }
            else
            {
                return;
            }
        }

        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.deltaTime;
            return;
        }

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

        // Move the lava up
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

            // Set player's vertical velocity to 0 BEFORE trampoline power-up
            var rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            }

            // Grant trampoline power-up every time
            var trampoline = collision.gameObject.GetComponent<TrampolineJump>();
            var movement = collision.gameObject.GetComponent<PlayerMovement>();

            if (movement != null)
            {
                if (trampoline == null)
                {
                    trampoline = collision.gameObject.AddComponent<TrampolineJump>();
                    trampoline.Initialize(movement);
                }
                trampoline.Jump(40f); // Always apply the jump
                Debug.Log("Trampoline power-up granted due to lava collision.");
            }

            // Clamp player above lava
            if (player != null)
            {
                float lavaTop = transform.position.y;
                Vector3 playerPos = player.position;
                if (playerPos.y < lavaTop)
                {
                    player.position = new Vector3(playerPos.x, lavaTop, playerPos.z);
                }
            }

            // Pause lava for a short duration
            pauseTimer = pauseDuration;
        }
    }
}