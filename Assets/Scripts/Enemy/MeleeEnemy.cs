using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    // Assign the ROOT player here (for general reference)
    public Transform player; 
    // NEW: Assign the CHILD object (the one with the collider/visuals) here!
    public Transform playerBody; 

    public bool isFlipped = false;
    public int attackDamage = 20;
    public int enragedAttackDamage = 40;
    public Vector3 attackOffset;
    public float attackRange = 0.5f;
    public LayerMask attackMask;

	public float attackCooldown = 1.5f; // Seconds between hits
	private float nextAttackTime = 0f;

	

    public void LookAtPlayer()
    {
        Vector3 targetPos = GetTargetPosition();

        Vector3 flipped = transform.localScale;
        flipped.z *= -1f;

        if (transform.position.x > targetPos.x && isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = false;
        }
        else if (transform.position.x < targetPos.x && !isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = true;
        }
    }

    public void Attack()
    {
        ApplyDamage(1); // Helper method to avoid repeating code
    }

    public void EnragedAttack()
    {
        ApplyDamage(enragedAttackDamage);
    }

	private void ApplyDamage(int damage)
	{
		// Check if enough time has passed
		if (Time.time < nextAttackTime) return;

		Vector3 pos = transform.position;
		pos += transform.right * attackOffset.x;
		pos += transform.up * attackOffset.y;

		Collider2D colInfo = Physics2D.OverlapCircle(pos, attackRange, attackMask);
		if (colInfo != null)
		{
			Health playerHealth = colInfo.GetComponentInParent<Health>();
			if (playerHealth != null)
			{
				playerHealth.Takedamage(damage, 1);
				// Set the timer for the next allowed attack
				nextAttackTime = Time.time + attackCooldown;
			}
		}
	}

    public Vector3 GetTargetPosition()
    {
        // If you assigned a specific body child, use its center. 
        // Otherwise, fall back to the collider center.
        if (playerBody != null) return playerBody.position;
        
        return player.GetComponentInChildren<Collider2D>().bounds.center;
    }

    void OnDrawGizmos()
    {
        if (player != null)
        {
            Vector3 target = GetTargetPosition();
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, target);
            Gizmos.DrawWireSphere(target, 0.2f); // Mark the actual target point
        }
        
        // Visualize the attack circle
        Vector3 pos = transform.position;
        pos += transform.right * attackOffset.x;
        pos += transform.up * attackOffset.y;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, attackRange);
    }
}