using UnityEngine;

public class Wings : MonoBehaviour
{   
    // Need reference to PlayerMovement.cs in order to control player's Rb, animation etc. 
    private PlayerMovement _movement;
    private float _flyDuration;
    private float _endTime;
    private bool _isFlying = false;

    private float _flySpeed = 6f;          
    private float _horizontalSpeed = 8f;   

    // Connects Wings.cs with player 
    public void Initialize(PlayerMovement movement)
    {
        _movement = movement;
    }

    public void Fly(float duration)
    {
        _flyDuration = duration;
        _endTime = Time.time + duration;
        _isFlying = true;

        _movement.Rb.gravityScale = 0f; // Activate floating mode 
        _movement.Animator.SetBool("IsJumping", true); // Activate jump animation
    }

    [System.Obsolete]
    private void Update()
    {
        if (!_isFlying) return;

        // What will happen when the time is over?
        if (Time.time > _endTime)
        {
            _isFlying = false;
            _movement.Rb.gravityScale = 4f; // Activate gravity again
            _movement.Animator.SetBool("IsJumping", false);
            Destroy(this); // Delete Wings.cs from player
            return;
        }

        // Bewegung: automatisch nach oben
        float horizontalInput = Input.GetAxisRaw("Horizontal"); // Reads arrow keys: -1 / 0 / 1
        Vector2 velocity = new Vector2(horizontalInput * _horizontalSpeed, _flySpeed); // Sets movement of player
        _movement.Rb.velocity = velocity;

        // Flip character
        if (horizontalInput != 0)
        {
            // How tall is the player => current scale (x,y,z)
            // x determines if sprite shows to the left (negative) or right (positive)
            Vector3 scale = _movement.transform.localScale; 
            // Alters signature of the x-scale
            scale.x = Mathf.Sign(horizontalInput) * Mathf.Abs(scale.x); 
            // Application of new scale
            _movement.transform.localScale = scale;
        }
    }
}
