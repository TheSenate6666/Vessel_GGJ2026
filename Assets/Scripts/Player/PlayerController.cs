
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
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


    [Header("Form Visuals Setup")]
    // 1. Reference to the ROOT components (Fire God)
    [SerializeField] private SpriteRenderer rootSpriteRenderer; 
    [SerializeField] private Animator rootAnimator;

    // 2. Reference to the CHILD GameObjects
    [SerializeField] private GameObject moonVisualsObj;
    [SerializeField] private GameObject deathVisualsObj;
    
    // 3. We assume these children have Animators on them. We cache them in Awake.
    private Animator moonAnim;
    private Animator deathAnim;

    // The "Public" Accessor that your State Machine will read
    public Animator currentAnimator { get; private set; }
    public PlayerForm currentForm;
    public void OnSwitchToMoon(InputAction.CallbackContext context) { if(context.performed) SetForm(PlayerForm.Moon); }
    public void OnSwitchToFire(InputAction.CallbackContext context) { if(context.performed) SetForm(PlayerForm.Fire); }
    public void OnSwitchToDeath(InputAction.CallbackContext context) { if(context.performed) SetForm(PlayerForm.Death); }



    private void Awake()
    {
        instance= this;

        // Cache the Child Animators
        if(moonVisualsObj != null) moonAnim = moonVisualsObj.GetComponent<Animator>();
        if(deathVisualsObj != null) deathAnim = deathVisualsObj.GetComponent<Animator>();

        // Start as Fire (Root)
        SetForm(PlayerForm.Fire);
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

        // Reset everything first
        // 1. Hide the ROOT (Fire) visuals
        rootSpriteRenderer.enabled = false;
        
        // 2. Hide the CHILD visuals
        if(moonVisualsObj) moonVisualsObj.SetActive(false);
        if(deathVisualsObj) deathVisualsObj.SetActive(false);

        // 3. Activate the specific form
        switch (newForm)
        {
            case PlayerForm.Fire:
                // Fire is on the ROOT
                rootSpriteRenderer.enabled = true;
                rootAnimator.enabled = true;
                currentAnimator = rootAnimator;
                break;

            case PlayerForm.Moon:
                // Moon is a CHILD
                moonVisualsObj.SetActive(true);
                currentAnimator = moonAnim;
                // Optional: Disable root animator to save performance
                rootAnimator.enabled = false; 
                break;

            case PlayerForm.Death:
                // Death is a CHILD
                deathVisualsObj.SetActive(true);
                currentAnimator = deathAnim;
                rootAnimator.enabled = false;
                break;
        }

        Debug.Log("Switched to " + newForm);
    }


    private void UpdateAnimations()
    {
        if (currentAnimator == null) return;

        // Send parameters to the ACTIVE animator (whichever one it is)
        currentAnimator.SetFloat("horizontalSpeed", Mathf.Abs(horizontol));
        currentAnimator.SetBool("isGrounded", IsGrounded());
        currentAnimator.SetFloat("verticalVelocity", rb.linearVelocity.y);
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
         // Instead of flipping the root, we check which form is active
         Transform targetTransform = transform; // Default to root

         if (currentForm == PlayerForm.Moon) targetTransform = moonVisualsObj.transform;
         else if (currentForm == PlayerForm.Death) targetTransform = deathVisualsObj.transform;
         else targetTransform = transform; // Fire is root

         if (horizontol > 0)
            targetTransform.localScale = new Vector3(1, 1, 1);
         else if (horizontol < 0)
            targetTransform.localScale = new Vector3(-1, 1, 1);
    }

    public void ReloadScene(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Gets the name of the currently active scene and reloads it
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    
}