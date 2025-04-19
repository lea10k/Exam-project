using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType 
    { 
        Trampoline, 
        Wings, 
        DoubleJump 
    } 
    public PowerUpType powerUpType;     // Type of power-up
    public GameObject pickupEffect;     // Effect to spawn on pickup

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")){
            Pickup(other);
        }
            
    }

    void Pickup(Collider2D player){
         // 1) Spawn effect
        Instantiate(pickupEffect, transform.position, transform.rotation);

        // 2) Fetch the player's Movement component
        var movement = player.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            // 3) Apply the right power-up
            switch (powerUpType)
            {
                /*case PowerUpType.Trampoline:
                    //movement.EnableTrampoline();   // you’ll need to add this
                    break;

                case PowerUpType.Wings:
                    //movement.EnableWings();        // and this
                    break; */

                case PowerUpType.DoubleJump:
                    movement.EnableDoubleJump();   // already implemented
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
       

