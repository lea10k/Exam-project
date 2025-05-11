using UnityEngine;

public class monsterBehavior : MonoBehaviour
{
    public float speed = 2f; // Movement speed
    public float pauseDuration = 1f; // Maximum pause duration
    public float groundCheckDistance = 0.5f; // Distance to check for ground
    public LayerMask groundLayer; // LayerMask for ground detection
    private bool movingRight = true; // Direction of movement
    private bool isPaused = false; // Whether the monster is paused
    private Animator animator; // Reference to the Animator component
    private bool isAttacking = false; // Whether the monster is attacking
    private Collider2D[] monsterColliders; // Array to hold all Collider2D components

    void Start()
    {
        animator = GetComponent<Animator>();
        monsterColliders = GetComponents<Collider2D>(); // Get all Collider2D components
    }

    void Update()
    {
        if (!isPaused && !isAttacking)
        {
            // Check for ground ahead
            if (!IsGroundAhead())
            {
                // Reverse direction if no ground ahead
                movingRight = !movingRight;
                FlipSprite(movingRight ? 1 : -1);
                return;
            }

            // Move the monster
            float moveDirection = movingRight ? 1 : -1;
            transform.Translate(Vector2.right * moveDirection * speed * Time.deltaTime);

            // Update Animator parameters
            animator.SetFloat("movementX", moveDirection);
            animator.SetFloat("movementY", 0); // No vertical movement

            // Flip the sprite based on direction
            FlipSprite(moveDirection);
        }
        else if (isAttacking)
        {
            // Trigger attack animation
            animator.SetTrigger("attack");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Start attack animation
            isAttacking = true;
            animator.SetTrigger("attack");

            // Damage the player
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }
            // enemy basically die to prevent multiple damage
            // Disable all colliders after attacking
            foreach (var collider in monsterColliders)
            {
                collider.enabled = false;
            }

            // Change sorting layer to "Background"
            var spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingLayerName = "DeadMonster";
                Debug.Log("Monster moved to Background layer after death.");
            }
        }
        else if (collision.gameObject.CompareTag("Projectile"))
        {
            // Monster dies when hit by a projectile
            Die();

            // Destroy the projectile
            Destroy(collision.gameObject);
        }
    }

    private void Die()
    {
        isAttacking = false;
        isPaused = true;
        animator.SetTrigger("death"); // Optional: trigger death animation if exists

        // Disable all colliders
        foreach (var collider in monsterColliders)
        {
            collider.enabled = false;
        }

        // Change sorting layer to "DeadMonster"
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingLayerName = "DeadMonster";
            Debug.Log("Monster moved to Background layer after death.");
        }

        // Destroy the monster after a short delay to allow animation to play
        //Destroy(gameObject);
    }

    private System.Collections.IEnumerator PauseMovement()
    {
        isPaused = true;
        float randomPause = Random.Range(0.5f, pauseDuration);
        yield return new WaitForSeconds(randomPause);
        isPaused = false;
    }

    private void FlipSprite(float moveDirection)
    {
        // Flip the sprite by changing the localScale.x
        Vector3 scale = transform.localScale;
        scale.x = moveDirection > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    private bool IsGroundAhead()
    {
        // Cast a ray downward at the edge of the monster
        Vector2 rayOrigin = transform.position + (movingRight ? Vector3.right : Vector3.left) * 0.5f;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, groundCheckDistance, groundLayer);

        // Return true if the ray hits any collider in the ground layer
        return hit.collider != null;
    }
}
