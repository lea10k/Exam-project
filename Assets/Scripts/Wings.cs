using UnityEngine;

public class Wings : MonoBehaviour
{
    private PlayerMovement _movement;
    private float _flyDuration;
    private float _endTime;
    private bool _isFlying = false;

    private float _flySpeed = 6f;          // wie schnell nach oben
    private float _horizontalSpeed = 8f;   // seitliche Geschwindigkeit

    public void Initialize(PlayerMovement movement)
    {
        _movement = movement;
    }

    public void Fly(float duration)
    {
        _flyDuration = duration;
        _endTime = Time.time + duration;
        _isFlying = true;

        _movement.Rb.gravityScale = 0f;
        _movement.Animator.SetBool("IsJumping", true);
    }

    [System.Obsolete]
    private void Update()
    {
        if (!_isFlying) return;

        // Flugzeit abgelaufen?
        if (Time.time > _endTime)
        {
            _isFlying = false;
            _movement.Rb.gravityScale = 3f; // oder dein ursprünglicher Wert
            _movement.Animator.SetBool("IsJumping", false);
            Destroy(this);
            return;
        }

        // Bewegung: automatisch nach oben
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        Vector2 velocity = new Vector2(horizontalInput * _horizontalSpeed, _flySpeed);
        _movement.Rb.velocity = velocity;

        // Optional: Flip beim Richtungswechsel
        if (horizontalInput != 0)
        {
            Vector3 scale = _movement.transform.localScale;
            scale.x = Mathf.Sign(horizontalInput) * Mathf.Abs(scale.x);
            _movement.transform.localScale = scale;
        }
    }
}
