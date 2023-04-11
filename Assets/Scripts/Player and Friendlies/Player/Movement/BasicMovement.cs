using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] float jumpCooldownTime = 0.1f;

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

    #region STATE VARIABLES
    [HideInInspector] private static bool isGrounded;
    [HideInInspector] private static bool isMovingOnGround;
    [HideInInspector] private static bool isJustLanded;
    [HideInInspector] private static bool isJumping;
    #endregion

    #region PRIVATE VARAIBLES
    Coroutine justLandedCache = null;
    Vector2 inputDirection = Vector2.zero;
    float refVelocity = 0f;

    bool jumpInputReceived;

    #endregion

    #region CONSTANTS
    const int NO_INPUT = 0;
    // TODO: see how you can assign a value to these once
    float ORIGNIAL_COYOTE_TIME;
    float ORIGINAL_JUMP_BUFFER_TIME;
    float ORIGNAL_JUMP_FORCE;
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
        ORIGNIAL_COYOTE_TIME = coyoteTime;
        ORIGNAL_JUMP_FORCE = jumpForce;
        ORIGINAL_JUMP_BUFFER_TIME = jumpBufferTime;

        jumpBufferTime = 0f;
    }

    void Update(){
        GroundCheck();
        DecreaseCoyoteTime();
        DecreaseJumpBufferTime();
        if(jumpInputReceived){
            jumpBufferTime = ORIGINAL_JUMP_BUFFER_TIME;
        }
    }

    void FixedUpdate(){
        CapFallingVelocity();
        ApplyMovement();
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
        //why the hell i barely commented this
        if (coyoteTime > 0f && jumpBufferTime > 0f && !isJumping){
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpForce);
            jumpBufferTime = 0f;
            StartCoroutine(EnableThenDisable(_ => isJumping = _, jumpCooldownTime));
        }

        if(jumpInputReceived && rigidBody.velocity.y > 0f)
            coyoteTime = 0f;
    }

    void DecreaseCoyoteTime(){
        if (isGrounded)
            coyoteTime = ORIGNIAL_COYOTE_TIME;
        else
            coyoteTime -= Time.deltaTime;
    }

    void DecreaseJumpBufferTime(){
        if (jumpBufferTime > 0f)
            jumpBufferTime -= Time.deltaTime;
    }
    #endregion

    #region OTHER METHODS
    void GroundCheck(){
        if (Physics2D.OverlapBox(groundBoxCastPosition.position, groundBoxCastSize, 0, groundLayer)){
            isGrounded = true;

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
        }
        else
            jumpInputReceived = false;
    }
    #endregion
}