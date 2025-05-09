using System.Collections;
using UnityEngine;

public class PowerUp : MonoBehaviour
{   
    [Header("Durations")]
    public float doubleJumpDuration = 5f; 
    public float WingsDuration = 3f;

    [Header("Height")]
    public float TrampolineJumpHeight = 25f;

    [Header("Select Power Up")]
    public PowerUpType powerUpType;     // Type of power-up
    public GameObject pickupEffect;     // Effect to spawn on pickup

    private readonly int amountOfLives = 1;
    public enum PowerUpType 
    { 
        Trampoline, 
        Wings, 
        DoubleJump,
        Life,
        nothing
    } 
    

    void DeactivateCurrentPowerUps(GameObject player)
    {
        // Get all power up components 
        Wings wings = player.GetComponent<Wings>();
        TrampolineJump trampoline = player.GetComponent<TrampolineJump>();
        PlayerMovement movement = player.GetComponent<PlayerMovement>();

        // Deactivate wings effect if activated 
        if (wings != null) 
        {
            // Reset wings effect manually, otherwise jump height won't be as determined.
            if (movement != null && movement.Rb != null)
            {
                // Activate normal gravity again
                movement.Rb.gravityScale = 4f;
                movement.Animator.SetBool("IsJumping", false);
            }
            Destroy(wings);
            Debug.Log("Wings deactivated");
        }

        // Deactivate trampoline if activated 
        if (trampoline != null) 
        {
            Destroy(trampoline);
            Debug.Log("Trampoline deactivated");
        }

        // Deactivate double jump if activated 
        if (movement != null)
        {
            movement.DisableDoubleJump();
            Debug.Log("Double jump deactivated");
        }
    }

    [System.Obsolete]
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")){
            StartCoroutine(Pickup(other));
        }
            
    }

    [System.Obsolete]
    IEnumerator Pickup(Collider2D player){
        DeactivateCurrentPowerUps(player.gameObject);
        // Fetch the player's Movement component
        var movement = player.GetComponent<PlayerMovement>();
        
        if (movement != null)
        {
            // Apply the right power-up
            switch (powerUpType)
            {
                case PowerUpType.Trampoline:
                    // Spawn effect but rotate it upwards
                    GameObject effect1 = Instantiate(pickupEffect, transform.position, Quaternion.Euler(0, 0, 90));
                    // Destroy effect after animation => unless, its last frame stays on the screen 
                    Destroy(effect1, 0.317f);

                    var trampoline = player.gameObject.AddComponent<TrampolineJump>();
                    trampoline.Initialize(movement);

                    trampoline.Jump(TrampolineJumpHeight); 
                    Debug.Log("player jumps " + TrampolineJumpHeight); 
                    break;

                case PowerUpType.Wings:
                    GameObject effect2 = Instantiate(pickupEffect, transform.position, transform.rotation);
                    Destroy(effect2, 0.25f);

                    var wings = player.gameObject.AddComponent<Wings>();
                    wings.Initialize(movement);
                    wings.Fly(WingsDuration);
                    break;   

                case PowerUpType.DoubleJump:
                    GameObject effect3 = Instantiate(pickupEffect, transform.position, transform.rotation);
                    Destroy(effect3, 0.25f);

                    movement.EnableDoubleJump();
                    Debug.Log("Double jump activated");

                    GetComponent<Collider2D>().enabled = false;
                    GetComponent<SpriteRenderer>().enabled = false;

                    yield return new WaitForSeconds(doubleJumpDuration);

                    movement.DisableDoubleJump();
                    Debug.Log("Double jump deactivated");
                    break;

                case PowerUpType.Life:
                    GameObject effect4 = Instantiate(pickupEffect, transform.position, transform.rotation);
                    Destroy(effect4, 0.233f);

                    // Fetch the PlayerHealth component and heal the player
                    var playerHealth = player.GetComponent<PlayerHealth>();

                    if (playerHealth != null)
                    {
                        playerHealth.Heal(amountOfLives);
                        Debug.Log($"Healed the player by {amountOfLives} life/lives.");
                    }
                    else
                    {
                        Debug.LogError("Player is missing PlayerHealth component!");
                    }
                    break;

                case PowerUpType.nothing:
                    break;

                default:
                    Debug.LogWarning($"Unknown power-up type: {powerUpType}");
                    break;
            }
        }
        else
        {
            Debug.LogError("Player is missing Movement component!");
        }

        // Destroy the power-up
        Destroy(gameObject);
    }
}


