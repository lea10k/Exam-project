using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f; // Bewegungsgeschwindigkeit

    private Rigidbody2D rb;
    private Vector2 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Zugriff auf Rigidbody2D
    }

    void Update()
    {
        // Eingabe holen (A/D oder Pfeiltasten für Horizontal, W/S für Vertikal)
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector2(moveX, moveY).normalized;
    }

    void FixedUpdate()
    {
        // Bewegung anwenden
        rb.linearVelocity = moveDirection * moveSpeed;
    }
}
