using System.Collections;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] private float fallDelay = 1f;
    [SerializeField] private float destroyDelay = 2f;

    private bool falling = false;
    private Rigidbody2D rb;

    private void Awake()
    {
        // Suche Rigidbody2D am aktuellen Objekt oder einem Kind
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            rb = GetComponentInChildren<Rigidbody2D>();
        }

        if (rb == null)
        {
            Debug.LogError("Kein Rigidbody2D gefunden für FallingPlatform an: " + gameObject.name);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (falling) return;

        if (collision.transform.CompareTag("Player"))
        {
            StartCoroutine(StartFall());
        }
    }

    private IEnumerator StartFall()
    {
        falling = true;

        yield return new WaitForSeconds(fallDelay);

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }

        Destroy(gameObject, destroyDelay);
    }
}
