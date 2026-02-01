using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ArrowProjectile : MonoBehaviour
{
   public float speed = 15f;
    public float lifeTime = 3f;
     public int arrowDamage;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Moves the arrow in the direction the shooting point was facing
        rb.linearVelocity = transform.right * speed;
        
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Look for the EnemyHealth script you provided
        EnemyHealth enemy = collision.GetComponent<EnemyHealth>();

        if (enemy != null)
        {
            // Calling your specific method: Takedamage(damage, multiplier)
            enemy.Takedamage(arrowDamage, 1);
            Destroy(gameObject);
        }
        
        // Optional: Destroy arrow if it hits walls (Layer 0 or specific "Ground" tag)
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}