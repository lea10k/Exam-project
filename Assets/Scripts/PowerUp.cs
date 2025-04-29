using System.Collections;
using UnityEngine;

public class PowerUp : MonoBehaviour
{   
    [Header("Durations")]
    public float doubleJumpDuration = 5f; 
    public float WingsDuration = 3f;

    [Header("Height")]
    public float TrampolineJumpHeight = 40f;

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


    [System.Obsolete]

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")){
            StartCoroutine(Pickup(other));
        }
            
    }

    [System.Obsolete]
    IEnumerator Pickup(Collider2D player){
    
        // Fetch the player's Movement component
        var movement = player.GetComponent<PlayerMovement>();
        var trampoline = player.GetComponent<TrampolineJump>();
        var wings = player.GetComponent<Wings>();
        
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

                    trampoline = player.gameObject.AddComponent<TrampolineJump>();
                    trampoline.Initialize(movement);

                    trampoline.Jump(TrampolineJumpHeight); 
                    Debug.Log("player jumps " + TrampolineJumpHeight);
                    break;

                case PowerUpType.Wings:
                    GameObject effect2 = Instantiate(pickupEffect, transform.position, transform.rotation);
                    Destroy(effect2, 0.25f);

                    wings = player.gameObject.AddComponent<Wings>();
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


