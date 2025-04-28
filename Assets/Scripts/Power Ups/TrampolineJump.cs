using Unity.VisualScripting;
using UnityEngine;

public class TrampolineJump : MonoBehaviour{
    
    private PlayerMovement _movement;

    public void Initialize(PlayerMovement movement){
        _movement = movement;
    }

    [System.Obsolete]
    public void Jump(float jumpHeight)
    {
        _movement.Rb.velocity = new Vector2(_movement.Rb.velocity.x, jumpHeight);
        _movement.Animator.SetBool("IsJumping", true);
    }
}