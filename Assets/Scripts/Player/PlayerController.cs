
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    [Header("Player References")]
    [SerializeField] Rigidbody2D rb;

    [Header("Player Settings")]
    [SerializeField] float speed;
    [SerializeField] float jumpingForce;
    [SerializeField] float fallGravityMultiplier;
    [SerializeField] float lowJumpMultiplier;
    [SerializeField] float xvelocityjumpdivision;


    [Header("Player Settings")]
    [SerializeField] LayerMask groundLayers;
    [SerializeField] Transform groundCheck;


    private float horizontol;

    public void Move(InputAction.CallbackContext context)
    {
        horizontol = context.ReadValue<Vector2>().x;
    }

    public void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontol * speed, rb.linearVelocity.y);

        if (rb.linearVelocity.y < 0&& !IsGrounded())
        //rb.linearVelocityX =  new Vector(horizontol * speed/xvelocityjumpdivision);

    if (rb.linearVelocity.y < 0)
    {
        // Falling
        rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallGravityMultiplier - 1) * Time.fixedDeltaTime;
    }
    else if (rb.linearVelocity.y > 0 && !IsGrounded())
    {
        // Jump cut (short hop)
        rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
    }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCapsule(groundCheck.position, new Vector2(1.6f, 0.25f),CapsuleDirection2D.Horizontal,0,groundLayers);  
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingForce);
        }
    }

}