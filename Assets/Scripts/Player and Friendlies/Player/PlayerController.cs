using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Pathfinding;

public class PlayerController : MonoBehaviour
{
    #region Public Variables
    [Header("Basic Movment")]
    public float horizontalVelocity = 5f;
    [SerializeField] float maxFallingVelocity = 20f;
    [SerializeField] float groundedAccelerationTime = 0.05f;
    [SerializeField] float groundedDecelerationTime = 0.05f;
    [SerializeField] float jumpForce = 400f;
    [SerializeField] float coyoteTime = 0.5f;
    [SerializeField] float jumpBufferTime = 0.5f;
    [SerializeField] float jumpCooldownTime = 0.1f;

    [Space]
    [Header("Dashing")]
    public int dashesCount = 1;
    [SerializeField] float dashForce = 15f;
    [SerializeField] float dashingTime = 0.2f;
    [SerializeField] float dashingWallBoxSize;
    [Tooltip("If the player collides with a wall while dashing he will have this time stuck on the wall to jump")]
    [SerializeField] float wallHangTime = 1.5f;
    [SerializeField] float wallJumpForce;
    [SerializeField] AnimationCurve dashSlowDownCurve;
    [SerializeField] string playerLayer;
    [SerializeField] string flyerLayer;
    

    [Space]
    [Header("Rolling")]
    [SerializeField] float rollingJumpForced = 10f;
    [SerializeField] float minSpeedToRoll = 5f;
    [SerializeField] AnimationCurve rollingSpeedCurve;
    [Tooltip("This should be equal to the amount of keys in the rolling curve - 1")]
    [SerializeField] int rollingLastKey;
    [Tooltip("If the roll time is greater than this key's time, then we let the player jump higher")]
    [SerializeField] int jumpForceKey;

    [Space]
    [Header("Grappling Hook")]
    public float grapplingDelay = 2f;
    [SerializeField] float grapplingSpeed = 2f;
    [SerializeField] float distanceToDetachGrapple = 5f;
    [SerializeField] float grapplingAcceleration = 0.5f;
    [SerializeField] float finalForceOfGrapple = 5f;
    public float grappleDistance = 15f;
    [SerializeField] float grapplingCameraZoomOffset;
    [Tooltip("Positive value")]
    [SerializeField] float grapplingCameraZoomOutTime;
    [Tooltip("Negative value")]
    [SerializeField] float grapplingCameraZoomInTime;
    [SerializeField] float grapplingCameraShakeStrength;
    [SerializeField] Transform grappleOrigin;
    [SerializeField] LayerMask anchorLayer;
    [SerializeField] GameObject GrapplingArrowsParent;
    [SerializeField] GameObject[] GrapplingArrows;

    [Space]
    [Header("Knockbak")]
    [SerializeField] Vector2 enemyKnockForce;
    [SerializeField] float enemyKnockLinearDrag;
    [SerializeField] float enemyKnockGravity;


    [Space]
    [Header("Camera")]
    [SerializeField] float defaultCameraZoom = 8f;


    [Space]
    [Header("Other Required Variables")]
    [SerializeField] Vector2 groundBoxCastSize;
    [SerializeField] Vector2 wallBoxCastSize;

    [Space]
    [Header("Other Requiered Components")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform groundBoxCastPosition;
    [SerializeField] Transform rightWallBoxCastPosition;
    [SerializeField] Transform leftWallBoxCastPosition;
    [SerializeField] GameObject weaponsObject;

    [Space]
    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] PhysicsMaterial2D fullFrictionMaterial;
    [SerializeField] PhysicsMaterial2D zeroFrictionMaterial;
    [SerializeField] PhysicsMaterial2D bouncyMaterial;
    public Rigidbody2D rigidBody;
    [SerializeField] SpriteRenderer weaponSprite;

    [Header("Scripts")]
    [Space]
    [SerializeField] GrapplingStringController stringController;
    [SerializeField] CameraController cameraController;
    [SerializeField] CompanionAbilitiesController companionAbilitiesController;

    #endregion

    #region Public And Hidden Varriables
    [Space] //in case of debugging
    [HideInInspector] public int dashesLeft;
    [HideInInspector] public float dashDelayTimer;
    [HideInInspector] public static GameObject player;
    [HideInInspector] public static bool isKnocked;
    [HideInInspector] public bool canGrapple = true;

    // ---------- States ----------
    [HideInInspector] public bool isRolling;
    [HideInInspector] public bool isDashing;
    [HideInInspector] public bool isJustDashing;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool isJustLanded;
    [HideInInspector] public bool isJustHitWall;
    [HideInInspector] public bool isMoving;
    [HideInInspector] public bool isJumping;
    [HideInInspector] public bool isGrappling;
    [HideInInspector] public bool isJustGrappling;
    [HideInInspector] public bool isJustFinishedGrappling;
    [HideInInspector] public bool isJustBrokeGrappling;
    [HideInInspector] public bool grapplingLoaded;
    [HideInInspector] public bool isWallHanging; // player is hitting a wall while dashing
    [HideInInspector] public bool isCollidingWithCollider; // Basically walls and ground
    [HideInInspector] public bool isCollidingWithLeftWall;
    [HideInInspector] public bool isCollidingWithRightWall;
    #endregion

    #region Private Variables
    int grappleButtonPresses;
    int rollingDirection;


    float input;
    float originalCoyoteTime;
    float originaljumpBufferTime;
    float originalGravityScale;
    float originalDistanceToDetach;
    float rollingCurveCurrentTime = 0f;
    float originalJumpForce;
    float dashTimer;
    float dashSlowDownTimer;
    float wallHangTimer;
    
    public bool jumpInputReceived;
    bool dashInputReceived;
    bool grappleRayIsHit;
    bool canRoll;
    bool applyRollForce = true;

    Vector3 grapplingDirection = Vector3.zero;
    Vector2 originalColliderSize = Vector2.zero;
    Vector2 dashingColliderSize = Vector2.zero;
    Vector2 dashingDirection = Vector2.zero;
    Vector2 finalGrapplingForceDirection = Vector2.zero;
    Vector2 finalGrapplingForceDirectionOld = Vector2.zero;
    Vector2 originalWallBoxCastSize = Vector2.zero;

    PhysicsMaterial2D originalMaterial;
    GameObject anchor;
    SpriteRenderer anchorSpriteRenderer;
    SpriteRenderer anchorIndicatorSpriteRender;

    // ---------- Coroutine cache ----------
    Coroutine justLandedCache = null;
    Coroutine justHitWallCache = null;
    Coroutine justCanGrappleCache = null;


    // ---------- Refrences for smoothdamp ----------
    float refVelocity = 0f;
    Vector2 refVelocitVector2 = Vector2.zero;

    // ---------- Constants ----------
    const float NoGravity = 0f;
    readonly Vector2 NoInput = Vector2.zero;
    
    #endregion

    #region Awake, Start, Update, OnX functions
    private void Start()
    {
        player = gameObject;

        originalCoyoteTime = coyoteTime;
        originalGravityScale = rigidBody.gravityScale;
        originalJumpForce = jumpForce;
        dashesLeft = dashesCount;
        originalColliderSize = boxCollider.size;
        dashingColliderSize = new Vector2(originalColliderSize.x-0.01f, originalColliderSize.y - 0.01f);
        wallHangTimer = wallHangTime;
        originalDistanceToDetach = distanceToDetachGrapple;
        originalWallBoxCastSize = wallBoxCastSize;

        originaljumpBufferTime = jumpBufferTime;
        jumpBufferTime = 0f;

        //it will reset next frame
        //but unity breaks if youre trying to access something that is null
        //even tho you have a condition to check if its null
        //so youre forced to assign it to some value
        //anchor = new GameObject("DEFAULT ANCHOR");
    }

    private void Update()
    {
        GroundCheck();
        WallCheck(rightWallBoxCastPosition.position, wallBoxCastSize, _ => isCollidingWithRightWall = _);
        WallCheck(leftWallBoxCastPosition.position, wallBoxCastSize, _ => isCollidingWithLeftWall = _);
        MouseClicksCounter();
        GrappleRay();
        WhileGroundedVariablesReset();

        if(!(isKnocked || isGrappling))    
            Roll();

        if(!isGrappling && !isKnocked)
            GetDashInput();

        if (isKnocked)
            StartCoroutine("ResetKnock");

        if (isKnocked || isRolling || isDashing)
            WeaponsSwitch(false);
        else
            WeaponsSwitch(true);

        ///umm, these shouldnt be here, they should have their own functions, fix it at some point
        //Coyote and jump buffering timers for jumping
        if (isGrounded)
            coyoteTime = originalCoyoteTime;
        else
            coyoteTime -= Time.deltaTime;

        if (jumpBufferTime > 0f)
            jumpBufferTime -= Time.deltaTime;

        //Displaying grappling string and other effects
        if (isGrappling)
        {
            stringController.gameObject.SetActive(true);
            stringController.GrapplingStringTarget(grappleOrigin.transform, anchor.transform);
            //Camera shake and zoom out
            //cameraController.CameraShake(grapplingCameraShakeStrength);
            //cameraController.CameraZoom(grapplingCameraZoomOffset, grapplingCameraZoomOutTime, defaultCameraZoom);
        }
        else if (!isGrappling)
        {
            //Resetting
            stringController.gameObject.SetActive(false);
            //cameraController.CameraZoom(0, grapplingCameraZoomInTime, defaultCameraZoom);
        }

        

        //DEBUGGING

        //MOVEMENT
        //Debug.Log("Input = " + input);
        //Debug.Log("Player velocity: X = " + rigidBody.velocity.x + " Y =" + rigidBody.velocity.y);
        //if(isGrappling)
        //Debug.Log("Current Input direction (GetKeyDown)::: " + GetInputDirection(false, true));
        //Debug.Log("Current Input direction (GetKey)::: " + GetInputDirection(true, false));

        //GRAPPLING
        //Debug.Log("Distance to grapple point = " + Vector3.Distance(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition))); // Remember that theres a difference of 10 which is dealt with in Grapple()
        //Debug.Log("Current position = " + transform.position);
        //Debug.Log("Current mouse position = " + Camera.main.ScreenToWorldPoint(Input.mousePosition));

        //BOOLS
        //Debug.Log("isKnocked = " + isKnocked

        
    }

    private void FixedUpdate()
    {
        CapFallingVelocity();
        
        if (isWallHanging)
            WallHang();

        if (dashInputReceived)
        {
            SetupDash();
            dashInputReceived = false;
        }

        if (!(isGrappling || isRolling || isDashing || isWallHanging))
        {
            ApplyMovement();
            ApplyJump(jumpForce);
        }

        if (applyRollForce)
            ApplyRollForce();

        if (!(isRolling || isDashing || isKnocked || isWallHanging))
            Grapple(originalGravityScale);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isCollidingWithCollider = true;
    }

    private void OnCollisionStay2D(Collision2D other) 
    {
        CheckForWallHang();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isCollidingWithCollider = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDashing && collision.gameObject.CompareTag("Flyer"))
            Knockback(collision);
    }
    #endregion

    #region MOVEMENT
    public void MovementInput(InputAction.CallbackContext context)
    {
        input = context.ReadValue<float>();
    }

    void ApplyMovement()
    {
        float targetVelocity = input * horizontalVelocity;
        
        if (input != 0)
            Accelerate(targetVelocity);
        else if (input == 0 && isGrounded)
            Decelerate();
        
        HandleFirction();
    }

    private void CapFallingVelocity()
    {
        if (rigidBody.velocity.y < maxFallingVelocity)
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, maxFallingVelocity);
    }

    void Accelerate(float targetVelocity)
    {
        //If the player is in the air and we recieve input and the current velocity of the player is already greater than the player maximum velocity
        //then we dont apply the movement line of code because it will slow the player down from his high speed to a lower speed.
        //Else if hes in the air and his current velocity without any input is less than the max velocity and the player gives input
        //then we apply the movment code
        //If hes grounded we always apply it because if not, the player will maintain the current velocity which is greater than the allowed max velocity while grounded.
        if ((rigidBody.velocity.x < targetVelocity && targetVelocity > 0) || (rigidBody.velocity.x > targetVelocity && targetVelocity < 0) && !isGrounded)
            rigidBody.velocity = new Vector2(Mathf.SmoothDamp(rigidBody.velocity.x, targetVelocity, ref refVelocity, groundedAccelerationTime), rigidBody.velocity.y);
        else if (isGrounded)
            rigidBody.velocity = new Vector2(Mathf.SmoothDamp(rigidBody.velocity.x, targetVelocity, ref refVelocity, groundedAccelerationTime), rigidBody.velocity.y);
    }

    void Decelerate()
    {
        rigidBody.velocity = new Vector2(Mathf.SmoothDamp(rigidBody.velocity.x, 0, ref refVelocity, groundedDecelerationTime), rigidBody.velocity.y);
    }

    void HandleFirction()
    {
        //Setting the x velocity to 0 while idle
        if (rigidBody.velocity.x < 1f && rigidBody.velocity.x > -1f && input == 0 && !isKnocked && !isRolling)
        {
            boxCollider.sharedMaterial = fullFrictionMaterial;
            isMoving = false; //irrelevant here, but relevant to know the stater if we are running or not in Move()
        }
        else
        {
            boxCollider.sharedMaterial = zeroFrictionMaterial;
            isMoving = isGrounded; // equivalent to isMoving = isGrounded ? true : false;
        }
    }
    #endregion

    #region JUMP
    
    public void JumpInput(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            jumpInputReceived = true;
            jumpBufferTime = originaljumpBufferTime;
        }
        else
            jumpInputReceived = false;
    }

    void ApplyJump(float jumpForce)
    {
        //why the hell i barely commented this
        if (coyoteTime > 0f && jumpBufferTime > 0f && !isJumping && !isWallHanging)
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpForce);

            jumpBufferTime = 0f;

            StartCoroutine(EnableThenDisable(_ => isJumping = _, jumpCooldownTime));
        }

        if(jumpInputReceived && rigidBody.velocity.y > 0f)
            coyoteTime = 0f;
    }
    #endregion

    #region DASH
    void Dash(Vector2 direction) 
    {
        if (rigidBody.gravityScale != NoGravity)
        {
            rigidBody.gravityScale = NoGravity;
            ApplyDashForce(direction);
            SetupDashKeys();
        }

        StartCoroutine(StopDashInTime(dashingTime, direction));
    }

    void SetupDash()
    {
        if(!(isDashing || isWallHanging || isRolling) && dashesLeft != 0)
        {
            isDashing = true;
            isMoving = false;
            dashesLeft--;
            StartCoroutine(EnableThenDisable(_ => isJustDashing = _, 0.1f));
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(playerLayer), LayerMask.NameToLayer(flyerLayer), false);

            SetupDashVariables();
            Dash(dashingDirection);
        }
    }

    private void ApplyDashForce(Vector2 direction)
    {
        rigidBody.velocity = new Vector2(0f, 0f); // we start the dash with 0 velocity to give it an oomph 
        rigidBody.AddForce(new Vector2(dashForce * direction.x, dashForce * direction.y), ForceMode2D.Impulse);
    }

    private void SetupDashKeys()
    {
        //We are using a curve in the inspector to slow down the player after the dash instead of keeping the momentum
        //We here set the first key value equal to the dash force so it would start slowing the player from that value to the desired value in the curve
        dashSlowDownCurve.RemoveKey(0);
        dashSlowDownCurve.AddKey(0f, dashForce);
    }

    private IEnumerator StopDashInTime(float time, Vector2 direction)
    {
        yield return new WaitForSeconds(time);
        
        if(isWallHanging)
            yield break;

        //Stopping the dash with the curve values (in the inspector)
        while(dashSlowDownTimer <= dashSlowDownCurve.keys[1].time)
        {
            dashSlowDownTimer += Time.deltaTime;
            float dashCurveCurrentValue = dashSlowDownCurve.Evaluate(dashSlowDownTimer);
            rigidBody.velocity = new Vector2(direction.x * dashCurveCurrentValue, direction.y * dashCurveCurrentValue);
            yield return null;
        }
        
        ResetDashVariables();
    }
    
    private void SetupDashVariables()
    {
        boxCollider.size = dashingColliderSize; //we smallen the collider in case the player is dashing while colliding
        wallHangTimer = wallHangTime;
        wallBoxCastSize.x = dashingWallBoxSize;
    }

    private void ResetDashVariables()
    {
        dashSlowDownTimer = 0f;
        rigidBody.gravityScale = originalGravityScale;
        boxCollider.size = originalColliderSize;
        wallBoxCastSize = originalWallBoxCastSize;
        //removing player's ability to collide with enemies when collided
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(playerLayer), LayerMask.NameToLayer(flyerLayer), true);
        isDashing = false;
    }

    private void GetDashInput()
    {
        dashingDirection = GetOnHoldInput();
        if(Input.GetKeyDown(KeyCode.LeftShift) && dashingDirection != Vector2.zero)
            dashInputReceived = true;
    }
    #endregion

    #region WALL HANG
    // If the player hits a wall while dashing he gets stuck to it for some time
    private void WallHang()
    {
        SetupWallHangVariables();
        EliminateVelocity();
        EliminateGravity();

        wallHangTimer -= Time.deltaTime;

        if (wallHangTimer <= 0)
            ResetWallHangVariables();
        
        if (jumpInputReceived && wallHangTimer <= (wallHangTime - 0.15f))
            StartCoroutine(WallJump());
    }

    IEnumerator WallJump()
    {
        ResetWallHangVariables();
        yield return null;
        rigidBody.velocity = new Vector2((wallJumpForce*input)/2, wallJumpForce);
        StartCoroutine(EnableThenDisable((_ => isJumping = _), jumpCooldownTime));
    }

    void CheckForWallHang()
    {
        if(!isGrounded && (isCollidingWithLeftWall || isCollidingWithRightWall))
        {
            if(isDashing)
                isWallHanging = true;
        }
    }

    private void EliminateVelocity()
    {
        if(rigidBody.velocity != Vector2.zero)
            rigidBody.velocity = Vector2.zero;
    }

    private void EliminateGravity()
    {
        if (rigidBody.gravityScale != NoGravity)
            rigidBody.gravityScale = NoGravity;
    }

    private void SetupWallHangVariables()
    {
        if(isWallHanging == false)
            isWallHanging = true;

        if(isDashing == true)
            isDashing = false;
    }

    private void ResetWallHangVariables()
    {
        rigidBody.gravityScale = originalGravityScale;
        boxCollider.size = originalColliderSize;
        wallHangTimer = 0f;
        isWallHanging = false;
    }
    #endregion

    #region ROLL
    void Roll()
    {
        if (CanRoll())
        {
            canRoll = true;
            GetRollingDirection();
        }
        
        if(canRoll)
        {
            // if the player lands and is holding down 'S' then he can start rolling
            // it is separated so that the player can stop holding S while rolling
            if (RollingInitiated())
            {
                isRolling = true;
                isMoving = false; //irrelevant here, but relevant to know the stater if we are running or not in Move()
            }
        }

        if (isRolling)
        {
            applyRollForce = true; // ApplyRollForce() is called in FixedUpdat()
            if (jumpInputReceived)
            {
                if (InRollJumpZone())
                    ApplyJump(rollingJumpForced);
                else
                    ApplyJump(jumpForce);
            }
        }
        else if(isGrounded)
            canRoll = false;

        //Disabling the Roll
        if (CanceledRoll())
            StopRoll();
    }

    private bool RollingInitiated()
    {
        return Input.GetKey(KeyCode.S) && isJustLanded && !isDashing;
    }

    private void ApplyRollForce()
    {
        //Calculating the passed time of the roll
        rollingCurveCurrentTime += Time.deltaTime;
        //Getting the value on the curve during that time
        float rollingCurveValue = rollingSpeedCurve.Evaluate(rollingCurveCurrentTime);
        //Applying the velocity of the roll curve
        rigidBody.velocity = new Vector2(rollingCurveValue * rollingDirection, rigidBody.velocity.y);
    }

    private void StopRoll()
    {
        isRolling = false;
        rollingCurveCurrentTime = 0f;
        applyRollForce = false;
    }

    private void GetRollingDirection() 
    {
        if (rigidBody.velocity.x < -1)
            rollingDirection  = -1;
        else if (rigidBody.velocity.x > 1)
            rollingDirection = 1;
    }

    private bool CanRoll()
    {
        return !isGrounded && Mathf.Abs(rigidBody.velocity.x) > minSpeedToRoll;
    }

    bool InRollJumpZone()
    {
        //We only give the player extra jump force if the roll time has exceded the jump key in the curve
        return rollingCurveCurrentTime >= rollingSpeedCurve[jumpForceKey].time;
    }

    bool CanceledRoll()
    {
        return !isGrounded || isKnocked || isGrappling
                || isCollidingWithLeftWall
                || rollingCurveCurrentTime >= Mathf.Abs(rollingSpeedCurve.keys[rollingLastKey].time) // the curve timer has reached the curve last key
                || (Input.GetAxisRaw("Horizontal") != 0 && Input.GetAxisRaw("Horizontal") != rollingDirection); // got input in the opposite direction while rolling
    }

    void SetupRollKeys()
    {
        //Setting the first key of the rolling curve to match the current speed of the player so he would keep his momentum
        rollingSpeedCurve.RemoveKey(0);
        rollingSpeedCurve.AddKey(0f, Mathf.Abs(rigidBody.velocity.x) + 0.1f);
        //Edit the value of the key directly didnt work, so i just delete the old key and create a new one at the same time of the original key
    }
    #endregion

    #region GRAPPLE
    void Grapple(float originalGravity)
    {
        GrappleLoaded();

        //Activating the grapple
        if (StartedGrapple())
            SetupGrapplingVariables();
           

        if (isGrappling)
        {
            ApplyGrapplingForce();
            //Dispalying Grappling Arrows
            GrapplingArrowsParent.SetActive(true);
            UpdateFinalGrappleForce();
        }

        if (GrappleEnded())
        {
            if (isGrappling) // Used to stop applying these values when not grappling
            {
                if(anchor != null)
                {
                    //Giving the player a bonuse force for completing the grapple
                    if (Vector2.Distance(transform.position, anchor.transform.position) <= distanceToDetachGrapple)
                        ApplyFinalGrapplingForce();
                    else
                        StartCoroutine(EnableThenDisable(_ => isJustBrokeGrappling = _, 0.1f));
                }

                ResetGrapplingVariables(originalGravity);
            }

        }
    }

    void ApplyBouncyMaterial()
    {
        originalMaterial = boxCollider.sharedMaterial;
        boxCollider.sharedMaterial = bouncyMaterial;
    }

    void SetupGrapplingVariables()
    {
        isGrappling = true; //Used to disable Move() to not interfer with grappling
        StartCoroutine(EnableThenDisable(_ => isJustGrappling = _, 0.1f));
        isMoving = false; //irrelevant here, but relevant to know the stater if we are running or not in Move()
        EliminateGravity();
        //Applying bounciness so the player wont get stuck by roofs or walls
        ApplyBouncyMaterial();
    }

    bool StartedGrapple()
    {
        return grappleButtonPresses == 1 && grappleRayIsHit && !isGrappling; // !isGrappling so it wouldnt be spammed while grappling
    }

    void GrappleLoaded()
    {
        //Used to detect when the moment the player can grapple
        if(canGrapple && justCanGrappleCache == null)
            justCanGrappleCache = StartCoroutine(EnableThenDisable(_ => grapplingLoaded = _, 0.1f));
    }

    void ApplyGrapplingForce()
    {
        if(!anchor)
            return;

        //Finding the position and direction of the mouse
        grapplingDirection = anchor.transform.position - transform.position;
        //We set the position of the mouse on z to 0 because the player is on z = 0
        grapplingDirection.z = 0;
        //We normalize the vector so that it doesnt be faster when the driection vector is large and vice versa
        grapplingDirection.Normalize();


        //To avoid circiling around the anchor, we only apply force to the rigid body when affar and when close we atrract the player directly ignoring the physics
        if (Vector2.Distance(gameObject.transform.position, anchor.transform.position) > 4f)
        {
            //Adding force by setting the velocity directly
            rigidBody.velocity = Vector2.SmoothDamp(rigidBody.velocity, grapplingDirection * grapplingSpeed, ref refVelocitVector2, grapplingAcceleration);
            //Adding force by adding force to the rigidbody
            //rigidBody.AddForce(grapplingDirection * grapplingSpeed, ForceMode2D.Force);
        }
        else
            transform.Translate(grapplingDirection * Time.deltaTime * grapplingSpeed);
    }

    void UpdateFinalGrappleForce()
    {
        //This bit down here fucking destroyed me, because holding input is the best one for this and is annoying to deal with each frame
        //i kept trying and failing with stupid bools, then at the end after 6 hours (maybe more)
        //just took a deep breath and just drew the variables changing frame by frame and saw how fucking shtoopid i was
        //.....ummm, funny story, i found a better and easier way in 10 mins.... yeah...

        //Deciding the final force of grappling direction
        finalGrapplingForceDirection = GetOnHoldInput();

        //if the input changes since the last frame
        if (finalGrapplingForceDirection != finalGrapplingForceDirectionOld) 
        {
            finalGrapplingForceDirectionOld = finalGrapplingForceDirection; //change the old input
            DisplayGrapplingArrows(finalGrapplingForceDirection);
        }
    }

    bool GrappleEnded()
    {
        if(!anchor)
            return true;

        //reached destination or if canceled by user or hit by rocketato
        return Vector2.Distance(transform.position, anchor.transform.position) <= distanceToDetachGrapple 
                || (grappleButtonPresses >= 2 && isGrappling) 
                || isKnocked;

    }

    private void ResetGrapplingVariables(float originalGravity)
    {

        // Resetting
        isGrappling = false;
        finalGrapplingForceDirection = Vector2.zero;
        finalGrapplingForceDirectionOld = Vector2.zero;

        rigidBody.gravityScale = originalGravity;
        boxCollider.sharedMaterial = originalMaterial;

        finalGrapplingForceDirection = Vector2.zero;
        GrapplingArrowsParent.SetActive(false);
        DisplayGrapplingArrows(Vector2.zero);

        grappleButtonPresses = 0;

        StartCoroutine(DisableThenEnable(_ => canGrapple = _, grapplingDelay));
        justCanGrappleCache = null;
    }

    private void ApplyFinalGrapplingForce()
    {
        StartCoroutine(EnableThenDisable(_ => isJustFinishedGrappling = _, 0.1f));

        if (finalGrapplingForceDirection != Vector2.zero)
        {
            rigidBody.velocity = Vector2.zero;

            //We could have one, but i wanted to adjust it in some areas because it feels quite weak
            if (finalGrapplingForceDirection.y == 0)
                rigidBody.AddForce(new Vector2(finalGrapplingForceDirection.x * finalForceOfGrapple * 3f, finalForceOfGrapple * 1.5f), ForceMode2D.Impulse);
            else if (finalGrapplingForceDirection.x == 0 && finalGrapplingForceDirection.y > 0)
                rigidBody.AddForce(new Vector2(0, finalGrapplingForceDirection.y * finalForceOfGrapple * 3.5f), ForceMode2D.Impulse);
            else
                rigidBody.AddForce(new Vector2(finalGrapplingForceDirection.x * finalForceOfGrapple * 2f, finalGrapplingForceDirection.y * finalForceOfGrapple * 1.5f * 2f), ForceMode2D.Impulse);
        }
        else
            rigidBody.AddForce(new Vector2(grapplingDirection.x * finalForceOfGrapple, grapplingDirection.y * finalForceOfGrapple * 1.5f), ForceMode2D.Impulse);
    }
    #endregion

    #region GRAPPLE RAY
    void GrappleRay()
    {
        Vector3 rayDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        rayDirection.z = 0f;
        rayDirection.Normalize();
        RaycastHit2D grappleRay = Physics2D.Raycast(transform.position, rayDirection, grappleDistance, anchorLayer);

        if (AnchorDetected(grappleRay))
        {
            anchor = grappleRay.collider.gameObject;
            SetAnchorIndicatorToColor(grappleRay, Color.green);
            grappleRayIsHit = true;
        }
        else
        {
            SetAnchorToColor(Color.red);
            grappleRayIsHit = false;
        }

        ResetAnchor(grappleRay);

        //Debuging
        //Vector3 end = transform.position + rayDirection * grappleDistance;
        //Debug.DrawLine(transform.position, end);
        //Debug.Log(grappleRayIsHit);
    }

    private bool AnchorDetected(RaycastHit2D grappleRay)
    {
        return grappleRay.collider != null;
    }

    private void SetAnchorIndicatorToColor(RaycastHit2D grappleRay, Color color)
    {
        if(!anchor)
            return;

        anchorSpriteRenderer = anchor.GetComponent<SpriteRenderer>();
        anchorIndicatorSpriteRender = anchor.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        anchorSpriteRenderer.color = color;
        anchorIndicatorSpriteRender.color = color;
    }

    private void SetAnchorToColor(Color color)
    {
        if(!anchorSpriteRenderer || !anchorIndicatorSpriteRender)
            return;

        anchorSpriteRenderer.color = color;
        anchorIndicatorSpriteRender.color = color;
    }

    private void ResetAnchor(RaycastHit2D grappleRay)
    {
        //Resetting the anchor to null while not grappling 
        //and the ray is not hit because its used to count the grapple button presses
        if (!isGrappling && grappleRay.collider == null)
            anchor = null;
    }
    #endregion

    #region GRAPPLE ARROWS
    private void DisplayGrapplingArrows(Vector2 inputDirection)
    {
        SpriteRenderer arrowSprite = null;
        Color arrowColor = Color.green;

        //if theres no input
        if (inputDirection == NoInput)
        {
            arrowColor = DeactivateGrapplingArrows(arrowColor);
            return;
        }
        
        arrowSprite = GetActiveGrapplingArrow(inputDirection, arrowSprite);

        // Deactivate all the arrows even the one we want
        arrowColor = DeactivateGrapplingArrows(arrowColor);
        // And because we're still in the same we can activate the one we want again
        // Without even displaying the deavtivation
        ActivateGrapplingArrow(arrowSprite);
    }

    private void ActivateGrapplingArrow(SpriteRenderer arrowSprite)
    {
        Color arrowColor;
        arrowColor.a = 1f;
        arrowColor = Color.green;
        arrowSprite.color = arrowColor;
    }

    private Color DeactivateGrapplingArrows(Color arrowColor)
    {
        //we loop over the arrows
        foreach (GameObject arrow in GrapplingArrows)
        {
            //find the active one
            if (arrow.GetComponent<SpriteRenderer>().color == Color.green)
            {
                //Disable it
                arrowColor.a = 0.3f;
                arrowColor = Color.white;
                arrow.GetComponent<SpriteRenderer>().color = arrowColor;
            }
        }

        return arrowColor;
    }
    private SpriteRenderer GetActiveGrapplingArrow(Vector2 inputDirection, SpriteRenderer arrowSprite)
        {
            //Getting the arrow to activate based on the given direction
            if (inputDirection.x == 1f && inputDirection.y == 0f) //Right
                arrowSprite = GrapplingArrows[1].GetComponent<SpriteRenderer>();
            else if (inputDirection.x == 1f && inputDirection.y == 1f) //Top Right
                arrowSprite = GrapplingArrows[0].GetComponent<SpriteRenderer>();
            else if (inputDirection.x == 1f && inputDirection.y == -1f) //Bottom Right
                arrowSprite = GrapplingArrows[2].GetComponent<SpriteRenderer>();
            else if (inputDirection.x == -1f && inputDirection.y == 0f) //Left
                arrowSprite = GrapplingArrows[5].GetComponent<SpriteRenderer>();
            else if (inputDirection.x == -1f && inputDirection.y == 1f) //Top Left
                arrowSprite = GrapplingArrows[6].GetComponent<SpriteRenderer>();
            else if (inputDirection.x == -1f && inputDirection.y == -1f) //Bottom Left
                arrowSprite = GrapplingArrows[4].GetComponent<SpriteRenderer>();
            else if (inputDirection.x == 0f && inputDirection.y == 1f) //Top
                arrowSprite = GrapplingArrows[7].GetComponent<SpriteRenderer>();
            else if (inputDirection.x == 0f && inputDirection.y == -1f) //Bottom
                arrowSprite = GrapplingArrows[3].GetComponent<SpriteRenderer>();
            return arrowSprite;
        }

    #endregion

    #region KNOCKBACK
    void Knockback(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Flyer"))
        {
            Rigidbody2D foundRigidBody = collider.gameObject.GetComponent<Rigidbody2D>();
            DisableFlyerScripts(collider);
            ApplyKnockbackForceTo(collider, foundRigidBody);
            collider.gameObject.GetComponent<EnemyFlyerController>().gotKnocked = true;
        }
    }
    private void DisableFlyerScripts(Collider2D collider)
    {

        //Disabling scripts that can interfer with the force
        collider.gameObject.GetComponent<AIPath>().enabled = false;
        collider.gameObject.GetComponent<AIDestinationSetter>().enabled = false;
    }
    private void ApplyKnockbackForceTo(Collider2D collider, Rigidbody2D foundRigidBody)
    {
        //Deciding the direction of the force
        if (transform.position.x < collider.gameObject.GetComponent<Transform>().position.x)
            foundRigidBody.AddForce(enemyKnockForce, ForceMode2D.Impulse);
        else
            foundRigidBody.AddForce(new Vector2(enemyKnockForce.x * -1, enemyKnockForce.y), ForceMode2D.Impulse);

        foundRigidBody.drag = enemyKnockLinearDrag;
        foundRigidBody.gravityScale = enemyKnockGravity;
    }

    IEnumerator ResetKnock()
    {
        yield return new WaitForSeconds(0.3f);
        isKnocked = false;
    }
    #endregion
    
    #region Miscellaneous
    void MouseClicksCounter()
    {
        if(canGrapple)
        {
            if (grappleButtonPresses == 0) // Letting the player grapple with only right click
            {
                if (Input.GetMouseButtonDown(1) && anchor != null) //Only add button presses if the grapple ray hits an anchor
                    grappleButtonPresses++;
            }
            else if (grappleButtonPresses == 1)//Letting the player detach the grapple with either "Space" ket (or jumping button) or "right click" (or grappling button)
            {
                if ((Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Space)) && anchor != null)
                {
                    grappleButtonPresses++;
                }
            }
        }
    }

    //Disbales palyers weapon
    void WeaponsSwitch(bool status) 
    {
        
        if(status)
        {
            weaponsObject.SetActive(true);
            weaponSprite.enabled = true;
            companionAbilitiesController.enabled = true;
        }
        else
        {
            weaponsObject.SetActive(false);
            weaponSprite.enabled = false;
            companionAbilitiesController.enabled = false;
        }
    }
    #endregion

    #region Checks
    void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundBoxCastPosition.position, groundBoxCastSize, 0, groundLayer))
        {
            isGrounded = true;

            if(justLandedCache == null && !isDashing) //!isDashing in case dashing while grounded
                justLandedCache = StartCoroutine(EnableThenDisable(_ => isJustLanded = _, 0.1f));
        }
        else
        {
            isGrounded = false;
            justLandedCache = null;
        }
    }

    //The global variable needs to be function in this form _ => globalVariable = _
    void WallCheck(Vector3 boxPosition, Vector2 boxSize, Action<bool> globalVariable)
    {
        if (Physics2D.OverlapBox(boxPosition, boxSize, 0, groundLayer))
        {
            globalVariable(true);
            if(justHitWallCache == null && !isDashing) //!isDashing in case dashing while grounded
                justHitWallCache = StartCoroutine(EnableThenDisable(_ => isJustHitWall = _, 0.1f));
        }
        else
        {
            globalVariable(false);
            justHitWallCache = null;
        }
    }
    #endregion

    #region Multiuse Functions
    Vector2 GetOnHoldInput()
    {
        Vector2 inputVector = Vector2.zero;

        if (Input.GetKey(KeyCode.W)) //UP
            inputVector.y = 1;

        if (Input.GetKey(KeyCode.S)) //DOWN
            inputVector.y = -1;

        if (Input.GetAxisRaw("Vertical") == 0)
            inputVector.y = 0;

        if (Input.GetKey(KeyCode.A)) //LEFT
            inputVector.x = -1;

        if (Input.GetKey(KeyCode.D)) //RIGHT
            inputVector.x = 1;

        if (Input.GetAxisRaw("Horizontal") == 0)
            inputVector.x = 0;

        return inputVector;
    }
    Vector2 GetOnPressInput()
    {
        Vector2 inputVector = Vector2.zero;

        if (Input.GetKeyDown(KeyCode.W)) //UP
            inputVector.y = 1;

        if (Input.GetKeyDown(KeyCode.S)) //DOWN
            inputVector.y = -1;

        if (Input.GetAxisRaw("Vertical") == 0)
            inputVector.y = 0;

        if (Input.GetKeyDown(KeyCode.A)) //LEFT
            inputVector.x = -1;

        if (Input.GetKeyDown(KeyCode.D)) //RIGHT
            inputVector.x = 1;

        if (Input.GetAxisRaw("Horizontal") == 0)
            inputVector.x = 0;

        return inputVector;
    }
    Vector2 GetOnReleaseInput()
    {
        Vector2 inputVector = Vector2.zero;

        if (Input.GetKeyUp(KeyCode.W)) //UP
            inputVector.y = 1;

        if (Input.GetKeyUp(KeyCode.S)) //DOWN
            inputVector.y = -1;

        if (Input.GetAxisRaw("Vertical") == 0)
            inputVector.y = 0;

        if (Input.GetKeyUp(KeyCode.A)) //LEFT
            inputVector.x = -1;

        if (Input.GetKeyUp(KeyCode.D)) //RIGHT
            inputVector.x = 1;

        if (Input.GetAxisRaw("Horizontal") == 0)
            inputVector.x = 0;

        return inputVector;
    }

    private void WhileGroundedVariablesReset()
    {
        if(isGrounded)
        {
            if(!isDashing)
                dashesLeft = dashesCount;
        }
    }

    private bool VelocityNearZero(float offsetMargin)
    {
        return -offsetMargin >= rigidBody.velocity.x && rigidBody.velocity.x <= offsetMargin;
    }



    //This function takes a function with a boolean argument as an alternative for pointers
    //e.g.  EnableThenDisable(_ => globalVariable = _, 0.1f);
    IEnumerator EnableThenDisable(Action<bool> switcher, float time)
    {
        switcher(true); // true => global = true;
        yield return new WaitForSeconds(time);
        switcher(false); // false => global = false; 
    }

    IEnumerator DisableThenEnable(Action<bool> switcher, float time)
    {
        switcher(false);
        yield return new WaitForSeconds(time);
        switcher(true);
    }

    void EnableGlobalVariable(Action<bool> switcher)
    {
        switcher(true);
    }

    void DisableGlobalVariable(Action<bool> switcher)
    {
        switcher(false);
    }
    #endregion
}
