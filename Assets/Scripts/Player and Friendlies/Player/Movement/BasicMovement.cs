using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class BasicMovement : MonoBehaviour
{
    #region INSPECTOR VARIABLES
    [Header("Movement")]
    public float horizontalVelocity = 5f;
    [SerializeField] float maxFallingVelocity = -20f;
    [SerializeField] float groundedAccelerationTime = 0.05f;
    [SerializeField] float groundedDecelerationTime = 0.05f;
    [Space]
    [Header("Jumping")]
    [SerializeField] float jumpForce = 15f;
    [SerializeField] float coyoteTime = 0.5f;
    [SerializeField] float jumpBufferTime = 0.5f;
    [Tooltip("The time before the jumping state variable is reset. "+
             "This state doesnt affect jumping, but rather other classes "+
             "checking for the state of the jump.")]
    [SerializeField] float jumpingStateCooldownTime = 0.1f;

    [Space]
    [Header("Required Components & Values")]
    [SerializeField] Vector2 groundBoxCastSize;
    [SerializeField] Transform groundBoxCastPosition;
    [SerializeField] LayerMask groundLayer;
    public Rigidbody2D rigidBody;
    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] PhysicsMaterial2D fullFrictionMaterial;
    [SerializeField] PhysicsMaterial2D zeroFrictionMaterial;
    #endregion

    #region STATE VARIABLES & OTHER STATIC VARIABLES
    [HideInInspector] private static bool isGrounded;
    [HideInInspector] private static bool isMovingOnGround;
    [HideInInspector] private static bool isJustLanded;
    [HideInInspector] private static bool isJumping;

    // ---- OTHER ----
    [HideInInspector] public static bool movementActive = true;
    [HideInInspector] public static bool jumpingActive = true;
    #endregion

    #region PRIVATE VARAIBLES
    Coroutine justLandedCache = null;
    Vector2 inputDirection = Vector2.zero;
    float refVelocity = 0f;
    float? lastGroundedTime;
    float? jumpInputReceivedTime;

    // ---- INPUT ----
    bool jumpInputReceived;
    #endregion

    #region CONSTANTS
    const int NO_INPUT = 0;
    #endregion

    #region GETTERS & SETTERS
    public static bool IS_GROUNDED{
        get{
            return isGrounded;
        }
    }

    public static bool IS_MOVING_ON_GROUND{
        get{
            return isMovingOnGround;
        }
    }

    public static bool IS_JUST_LANDED{
        get{
            return isJustLanded;
        }
    }

    public static bool IS_JUMPING{
        get{
            return isJumping;
        }
    }
    #endregion

    #region EXECUTION METHODS
    void Start(){
    }

    void Update(){
        GroundCheck();
    }

    void FixedUpdate(){
        CapFallingVelocity();

        if(movementActive)
            ApplyMovement();
        if(jumpingActive)
            ApplyJump(jumpForce);
    }
    #endregion

    #region MOVEMENT
    void ApplyMovement(){
        float targetVelocity = inputDirection.x * horizontalVelocity;
        
        if (inputDirection.x != NO_INPUT)
            Accelerate(targetVelocity);
        else if (inputDirection.x == NO_INPUT && isGrounded)
            Decelerate();
        
        HandleFirction();
    }

    private void CapFallingVelocity(){
        if (rigidBody.velocity.y < maxFallingVelocity)
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, maxFallingVelocity);
    }

    void Accelerate(float targetVelocity){
        rigidBody.velocity = new Vector2(Mathf.SmoothDamp(rigidBody.velocity.x, targetVelocity, ref refVelocity, groundedAccelerationTime), rigidBody.velocity.y);
    }

    void Decelerate(){
        rigidBody.velocity = new Vector2(Mathf.SmoothDamp(rigidBody.velocity.x, 0, ref refVelocity, groundedDecelerationTime), rigidBody.velocity.y);
    }

    void HandleFirction(){
        //Setting the x velocity to 0 while idle
        if (NoHorizontalMovement()){
            SetFullFriction();
            isMovingOnGround = false; 
        }
        else{
            RemoveFriction();
            isMovingOnGround = isGrounded;
        }
    }

    public void SetFullFriction(){
        if(boxCollider.sharedMaterial != fullFrictionMaterial)
            boxCollider.sharedMaterial = fullFrictionMaterial;
    }
    
    public void RemoveFriction(){
        if(boxCollider.sharedMaterial != zeroFrictionMaterial)
            boxCollider.sharedMaterial = zeroFrictionMaterial;
    }

    bool NoHorizontalMovement(){
        return rigidBody.velocity.x < 0.25f && rigidBody.velocity.x > -1f && inputDirection.x == 0;
    }
    #endregion

    #region JUMPING
    void ApplyJump(float jumpForce){
        if(lastGroundedTime == null || jumpInputReceivedTime == null)
            return;
        if(Time.time - lastGroundedTime > coyoteTime)
            return;
        if(Time.time - jumpInputReceivedTime > jumpBufferTime)
            return;

        rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpForce);
        StartCoroutine(EnableThenDisable(_ => isJumping = _, jumpingStateCooldownTime));

        lastGroundedTime = null;
        jumpInputReceivedTime = null;
    }
    #endregion

    #region OTHER METHODS
    void GroundCheck(){
        if (Physics2D.OverlapBox(groundBoxCastPosition.position, groundBoxCastSize, 0, groundLayer)){
            isGrounded = true;

            lastGroundedTime = Time.time;

            // TODO: REMEMBER TO DO THIS ONCE YOU IMPLEMENT DASHING
            // if(justLandedCache == null && !isDashing) //!isDashing in case dashing while grounded
            if(justLandedCache == null)
                justLandedCache = StartCoroutine(EnableThenDisable(_ => isJustLanded = _, 0.1f));
        }
        else{
            isGrounded = false;
            justLandedCache = null;
        }
    }

    //This function takes a function with a boolean argument as an alternative for pointers
    //e.g.  EnableThenDisable(_ => globalVariable = _, 0.1f);
    IEnumerator EnableThenDisable(Action<bool> switcher, float time){
        switcher(true); // true => global = true;
        yield return new WaitForSeconds(time);
        switcher(false); // false => global = false; 
    }
    #endregion

    #region INPUT HANDLERS
    public void OnXHoldInput(InputAction.CallbackContext context){
        inputDirection.x = context.ReadValue<float>();
    }

    public void OnYHoldInput(InputAction.CallbackContext context){
        inputDirection.y = context.ReadValue<float>();
    }

    public void JumpInput(InputAction.CallbackContext context){
        if(context.started){
            jumpInputReceived = true;
            jumpInputReceivedTime = Time.time;
        }
        else
            jumpInputReceived = false;
    }
    #endregion
}