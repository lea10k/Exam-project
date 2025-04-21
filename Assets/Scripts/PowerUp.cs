using System.Collections;
using UnityEngine;

public class PowerUp : MonoBehaviour
{   
    public float duration = 5f; // duration of (double jump) power up
    public bool doubleJumpActive = false; // Variable just to check if double jump is activated in the inspector
    public float jumpHeight = 20f;
    public enum PowerUpType 
    { 
        Trampoline, 
        Wings, 
        DoubleJump,
        nothing
    } 
    public PowerUpType powerUpType;     // Type of power-up
    public GameObject pickupEffect;     // Effect to spawn on pickup

    [System.Obsolete]

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")){
            StartCoroutine(Pickup(other));
        }
            
    }

    [System.Obsolete]
    IEnumerator Pickup(Collider2D player){
    
        // 2) Fetch the player's Movement component
        var movement = player.GetComponent<PlayerMovement>();
        var trampoline = player.GetComponent<TrampolineJump>();
        var wings = player.GetComponent<Wings>();
        
        if (movement != null)
        {
            // 3) Apply the right power-up
            switch (powerUpType)
            {
                case PowerUpType.Trampoline:
                     // 1) Spawn effect
                    GameObject effect1 = Instantiate(pickupEffect, transform.position, Quaternion.Euler(0, 0, 90));
                    // Destroy effect after animation => unless, its last frame stays on the screen 
                    Destroy(effect1, 0.317f);

                    trampoline = player.gameObject.AddComponent<TrampolineJump>();
                    trampoline.Initialize(movement);

                    trampoline.Jump(jumpHeight); 
                    Debug.Log("player jumps " + jumpHeight);
                    break;

                case PowerUpType.Wings:
                    GameObject effect2 = Instantiate(pickupEffect, transform.position, transform.rotation);
                    Destroy(effect2, 0.25f);

                    wings = player.gameObject.AddComponent<Wings>();
                    wings.Initialize(movement);
                
                    wings.Fly(duration);
                    break;   

                case PowerUpType.DoubleJump:
                     // 1) Spawn effect
                    GameObject effect3 = Instantiate(pickupEffect, transform.position, transform.rotation);
                    // Destroy effect after animation => unless, its last frame stays on the screen 
                    Destroy(effect3, 0.25f);

                    movement.EnableDoubleJump();
                    doubleJumpActive = true;

                    GetComponent<Collider2D>().enabled = false;
                    GetComponent<SpriteRenderer>().enabled = false;

                    yield return new WaitForSeconds(duration);

                    movement.DisableDoubleJump();
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

        // 4) Destroy the power-up
        Destroy(gameObject);
    }
}
       

