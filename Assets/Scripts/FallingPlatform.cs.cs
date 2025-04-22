using System.Collections;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
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

        // If player collides with platform
        if (collision.transform.CompareTag("Player"))
        {
            // startCoroutine means here: start StartFall() now, but execute it partly on multiple frames
            StartCoroutine(StartFall()); 
        }
    }

    private IEnumerator StartFall()
    {
        falling = true;

        // wait for fallDelay seconds then continue => fall after fallDelay seconds
        yield return new WaitForSeconds(fallDelay); 

        rb.bodyType = RigidbodyType2D.Dynamic;

        Destroy(gameObject, destroyDelay); // Destroy platform 
    }
}
