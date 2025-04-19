using UnityEngine;

public class SingleJump : IJumpBehaviour
{
    private readonly PlayerMovement _movement;

    public SingleJump(PlayerMovement movement)
    {
        _movement = movement;
    }

    [System.Obsolete]
    public void Jump()
    {
        // only allow jump when grounded
        if (_movement.IsGrounded())
        {
            _movement.Rb.velocity = new Vector2(_movement.Rb.velocity.x, _movement.JumpingPower);
            _movement.Animator.SetBool("IsJumping", true);
            // no extra state to track
        }
    }

    public void Reset()
    {
        // nothing to reset for single jump
    }
}
