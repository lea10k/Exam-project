using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float attackCooldown = 0.5f;

    private bool isAttacking = false;
    private float lastAttackTime = -Mathf.Infinity;
    private bool isFacingRight = true;

    private void Update()
    {
        // Determine facing direction from localScale.x
        isFacingRight = transform.localScale.x > 0f;

        if (Input.GetKeyDown(KeyCode.E) && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
        }
    }

    private void Attack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        animator.SetBool("IsAttacking", true);

        // Determine rotation based on facing direction
        Quaternion projRotation = isFacingRight ? Quaternion.identity : Quaternion.Euler(0, 180, 0);

        // Instantiate and launch projectile
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, projRotation);
        Rigidbody2D projRb = proj.GetComponent<Rigidbody2D>();
        if (projRb != null)
        {
            float direction = isFacingRight ? 1f : -1f;
            projRb.linearVelocity = new Vector2(direction * projectileSpeed, 0f);
        }

        // Reset attack state after a short delay
        Invoke(nameof(ResetAttack), 0.2f);
    }

    private void ResetAttack()
    {
        isAttacking = false;
        animator.SetBool("IsAttacking", false);
    }
}
