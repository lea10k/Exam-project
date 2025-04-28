using UnityEngine;

public class DoubleJump : IJumpBehaviour
{
    private readonly PlayerMovement _movement;
    private bool alreadyJumped;

    public DoubleJump(PlayerMovement movement)
    {
        _movement = movement;
    }

    [System.Obsolete]
    public void Jump()
    {
        if (_movement.IsGrounded())
        {
            // first jump, same as single
            _movement.Rb.velocity = new Vector2(_movement.Rb.velocity.x, _movement.JumpingPower);
            _movement.Animator.SetBool("IsJumping", true);
            alreadyJumped = false; // recharge double-jump
        }
        else if (!alreadyJumped)
        {
            // in air & double-jump still available
            _movement.Rb.velocity = new Vector2(_movement.Rb.velocity.x, _movement.JumpingPower);
            alreadyJumped = true;
        }
    }

    public void Reset()
    {
        // reset on landing
        alreadyJumped = false;
    }
}
