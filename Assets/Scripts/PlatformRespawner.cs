using System.Collections;
using UnityEngine;

public class PlatformRespawner : MonoBehaviour
{
    [Header("Prefab to Respawn")]
    public GameObject platformPrefab; // The platform to respawn

    [Header("Respawn Settings")]
    public Vector2 respawnPosition; // Where to respawn the platform
    public float delay = 2f; // Delay before the platform is spawned

    // Call this method to start the respawn process
    public void StartRespawn()
    {
        StartCoroutine(RespawnDelayed());
    }

    // Waits for delay, then instantiates the platform at saved position
    private IEnumerator RespawnDelayed()
    {
        yield return new WaitForSeconds(delay);

        if (platformPrefab != null)
        {
            Instantiate(platformPrefab, respawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogError("platformPrefab is not assigned in PlatformRespawner!");
        }

        // Destroy this helper after it's done
        Destroy(gameObject);
    }
}
