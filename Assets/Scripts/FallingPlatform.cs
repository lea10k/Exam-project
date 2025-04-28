using System.Collections;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private float fallDelay = 1f;
    [SerializeField] private float destroyDelay = 2f;

    public float timeToReloadPlatform = 2f;

    private bool falling = false;
    private Rigidbody2D rb; // Rb of platform


    private void Awake()
    {
        // Get rb of current platform
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (falling == true) return;
        if (!collision.transform.CompareTag("Player")) return;
        
        // startCoroutine means here: start StartFall() now, but execute it partly on multiple frames 
        if (collision.relativeVelocity.y <= 0f)
        {
            StartCoroutine(StartFall());
        }
    }


    private IEnumerator StartFall()
    {
        falling = true;

        yield return new WaitForSeconds(fallDelay);
        rb.bodyType = RigidbodyType2D.Dynamic;

        // Remember position before destroying
        Vector2 spawnPos = transform.position;

        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject); // Destroy platform

        // Create a temporary helper to handle delayed respawn
        GameObject respawner = new GameObject("PlatformRespawner");
        PlatformRespawner pr = respawner.AddComponent<PlatformRespawner>();
        
        pr.platformPrefab = platformPrefab;
        pr.respawnPosition = spawnPos;
        pr.delay = timeToReloadPlatform;
        pr.StartRespawn();
    }

}
