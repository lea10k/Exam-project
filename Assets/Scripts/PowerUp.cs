using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public GameObject pickupEffect; // Effect to spawn on pickup
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            Pickup();
        }
    }

    void Pickup()
    {
        // Spawn an effect
        Instantiate(pickupEffect, transform.position, transform.rotation);

        // Apply effect to the player

        Destroy(gameObject); // Destroy the power-up after pickup

    }
}