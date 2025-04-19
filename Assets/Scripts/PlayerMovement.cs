using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    [Header("Movement Settings")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpingPower = 16f;

    // Platform interaction fields (public so MovingPlatform can assign them)
    [HideInInspector] public bool isOnPlatform;
    [HideInInspector] public Rigidbody2D platformRb;

    // Expose private fields via public read-only properties for jump behaviours
    public Rigidbody2D Rb => rb;
    public Animator Animator => animator;
    public float JumpingPower => jumpingPower;

    private IJumpBehaviour _jumpBehaviour;
    private float _horizontal;
    private bool _isFacingRight = true;

    private void Awake()
    {
        // default to single jump behavior
        _jumpBehaviour = new SingleJump(this);
    }

    [System.Obsolete]
    private void Update()
    {
        // --- Running & Animations ---
        _horizontal = Input.GetAxisRaw("Horizontal");
        animator.SetFloat("Speed", Mathf.Abs(_horizontal));

        // --- Jump Input ---
        if (Input.GetButtonDown("Jump"))
        {
            _jumpBehaviour.Jump();
        }
        // Short‑hop logic for variable jump height
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        // handle facing direction
        Flip();
    }

    [System.Obsolete]
    private void FixedUpdate()
    {
        // Calculate combined horizontal velocity: player input + platform if standing on one
        float xVelocity = speed * _horizontal;
        if (isOnPlatform && platformRb != null)
        {
            xVelocity += platformRb.velocity.x;
        }
        rb.velocity = new Vector2(xVelocity, rb.velocity.y);

        // Reset jump animation and jump behavior when grounded
        if (IsGrounded())
        {
            animator.SetBool("IsJumping", false);
            _jumpBehaviour.Reset();
        }
    }

    /// <summary>
    /// Returns true if the groundCheck point overlaps the groundLayer.
    /// </summary>
    public bool IsGrounded()
    {
        // Check overlap with ground layers or if standing on a platform
        bool onGround = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        return onGround || isOnPlatform;
    }

    private void Flip()
    {
        if ((_isFacingRight && _horizontal < 0f) || (!_isFacingRight && _horizontal > 0f))
        {
            _isFacingRight = !_isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1f;
            transform.localScale = scale;
        }
    }

    /// <summary>
    /// Switches the player's jump behavior to double jump.
    /// </summary>
    public void EnableDoubleJump()
    {
        _jumpBehaviour = new DoubleJump(this);
    }
}
