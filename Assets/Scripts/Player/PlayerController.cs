
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;


public class PlayerController : MonoBehaviour
{
    [Header("Player References")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator anim;
    public static PlayerController instance;

    

    [Header("Player Movement Settings")]
    [SerializeField] float speed;
    [SerializeField] float jumpingForce;
    [SerializeField] float fallGravityMultiplier;
    [SerializeField] float lowJumpMultiplier;
    [SerializeField] float xvelocityjumpdivision;
    private float horizontol;


    [Header("Player GroundSettings")]
    [SerializeField] LayerMask groundLayers;
    [SerializeField] Transform groundCheck;

    [Header("Combat Settings")]
     private StateMachine meleeStateMachine;
    [SerializeField] public Collider2D hitbox;
    [SerializeField] public GameObject Hiteffect;



    private void Awake()
    {
        instance= this;
    }

    public void Start()
    {
        meleeStateMachine = GetComponent<StateMachine>();
    }



    public void Move(InputAction.CallbackContext context)
    {
        horizontol = context.ReadValue<Vector2>().x;
    }

    public void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontol * speed, rb.linearVelocity.y);

        if (rb.linearVelocity.y < 0&& !IsGrounded())
        //srb.linearVelocityX =  new Vector(horizontol * speed/xvelocityjumpdivision);

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
        Debug.Log("Jumped ");
        if (context.performed && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingForce);
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            
            // Safety check: is the state machine missing?
            if (meleeStateMachine == null)
            {
                Debug.LogError("StateMachine component missing from " + gameObject.name);
                return;
            }

            // Use ?. to safely check the type. If CurrentState is null, the check just returns false.
            if (meleeStateMachine.CurrentState?.GetType() == typeof(IdleCombatState))
            {
                meleeStateMachine.SetNextState(new GroundEntryState());
                
            }
            else if (meleeStateMachine.CurrentState is MeleeBaseState activeMeleeState)
            {
                activeMeleeState.AttackPressedTimer = 2;
                
            }
        }
    }

}