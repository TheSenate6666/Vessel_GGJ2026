
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public enum PlayerForm { Moon, Fire, Death }
public class PlayerController : MonoBehaviour
{
    [Header("Player References")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator anim;
    public static PlayerController instance;

    public Transform playerLookTransform;

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


    [Header("Form Settings")]
    public PlayerForm currentForm;
    [SerializeField] private RuntimeAnimatorController moonController;
    [SerializeField] private RuntimeAnimatorController fireController;
    [SerializeField] private RuntimeAnimatorController deathController;

    // Optional: If the forms have different sprites/meshes
    [SerializeField] private GameObject moonVisuals;
    [SerializeField] private GameObject fireVisuals;
    [SerializeField] private GameObject deathVisuals;

    public void OnSwitchToMoon(InputAction.CallbackContext context) { if(context.performed) SetForm(PlayerForm.Moon); }
    public void OnSwitchToFire(InputAction.CallbackContext context) { if(context.performed) SetForm(PlayerForm.Fire); }
    public void OnSwitchToDeath(InputAction.CallbackContext context) { if(context.performed) SetForm(PlayerForm.Death); }



    private void Awake()
    {
        instance= this;
    }

    public void Start()
    {
        meleeStateMachine = GetComponent<StateMachine>();
    }

    public void Update()
    {
        UpdateAnimations();
        FlipSprite();
    }

    private void SetForm(PlayerForm newForm)
    {
        currentForm = newForm;

        // 1. Swap the Animator Controller
        switch (newForm)
        {
            case PlayerForm.Moon:
                anim.runtimeAnimatorController = moonController;
                break;
            case PlayerForm.Fire:
                anim.runtimeAnimatorController = fireController;
                break;
            case PlayerForm.Death:
                anim.runtimeAnimatorController = deathController;
                break;
        }

        // 2. Swap Visuals (if they are different objects/meshes)
        moonVisuals.SetActive(newForm == PlayerForm.Moon);
        fireVisuals.SetActive(newForm == PlayerForm.Fire);
        deathVisuals.SetActive(newForm == PlayerForm.Death);
        
        Debug.Log("Switched to " + newForm + " Form");
    }


    private void UpdateAnimations()
    {
        // 1. Pass the absolute horizontal movement (0 to 1) for Idle/Run transition
        anim.SetFloat("horizontalSpeed", Mathf.Abs(horizontol));

        // 2. Pass the grounded status
        anim.SetBool("isGrounded", IsGrounded());

        // 3. Pass vertical velocity to distinguish between jumping and falling
        anim.SetFloat("verticalVelocity", rb.linearVelocity.y);
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

    private void FlipSprite()
    {
        // Safety check: ensure you've assigned the child in the inspector
        if (playerLookTransform == null) return;

        // Flips ONLY the visual child object based on movement direction
        if (horizontol > 0)
        {
            playerLookTransform.localScale = new Vector3(1, 1, 1);
        }
        else if (horizontol < 0)
        {
            playerLookTransform.localScale = new Vector3(-1, 1, 1);
        }

    }

    
}