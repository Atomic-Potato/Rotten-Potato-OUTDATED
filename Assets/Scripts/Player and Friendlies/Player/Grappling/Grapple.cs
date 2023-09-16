using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;  

public class Grapple : MonoBehaviour{

    #region INSPECTOR VARIABLES
    [SerializeField] float speed = 10f;
    [SerializeField] float accelerationTime = 10f;
    [Range(0f,5f)] [Tooltip("Delay before the player can grapple again")]
    [SerializeField] float delayTime = 0.75f;
    [Space]
    [Tooltip("When the distance between the player and the anchor is less than" + 
             "the \"Closure Distance\", then the player is translated straight to the anchor."+
             " This helps avoiding getting stuck in loops around anchors")]
    [SerializeField] float closureDistance = 4f;
    [Tooltip("When the distance between the player and the anchor is less than" + 
             "the \"Closure Distance\", then the player is translated straight to the anchor "+
             "with a speed of \"Closure Speed\"."+
             " This helps avoiding getting stuck in loops around anchors")]
    [SerializeField] float closureSpeed = 15f;
    [Space]
    [Tooltip("When the distance is less than this, the player will stop"+ 
             "and gets attached to the anchor")]
    [SerializeField] float distanceToAttach = 1f;
    [Tooltip("How much to offset the player position after being attached")]
    [SerializeField] Vector2 attachOffset;
    [Tooltip("The detach force that will be used when player speed is less than Max Detach Speed")]
    [SerializeField] float highDetachForce;
    [Tooltip("The detach force that will be used when player speed is greater than Max Detach Speed")]
    [SerializeField] float lowDetachForce;
    [Tooltip("At which speed will Low Detach Force be used instead of High Detach Force")]
    [SerializeField] float maxDetachSpeed; // Cant think of more descriptive name

    [Space]
    [Header("Required Components")]
    [SerializeField] LayerMask anchorLayer;
    [Tooltip("The layers that will block the detection of the anchor (including the anchor layer)")]
    [SerializeField] LayerMask collisionLayers;
    [SerializeField] Rigidbody2D rigidBody;

    [Space]
    [SerializeField] BasicMovement basicMovement;

    [Space]
    [Header("Debugging")]
    [SerializeField] bool enableDebugging;
    [SerializeField] bool drawDetectionLine;
    #endregion

    #region PRIVATE VARAIBALES
    float AnchorDetectionDistance;
    static Vector2 grappleEndPoint;
    static Vector2 grappleStartPoint;
    static Vector2 anchorPosition;
    Vector2 movementDirection;
    bool velocityRemoved;
    static GameObject ANCHOR;

    // ---- INPUT ----
    bool inputReceived;
    bool inputHoldReceived;
    bool jumpInputReceived;
    bool cancelInputReceived;

    // ---- CONSTANTS ----
    const float NO_GRAVITY = 0f;
    // ---- CONSTANTS-ish ----
    float ORIGINAL_GRAVITY_SCALE;

    // ---- CACHE ----
    Coroutine delayCache;
    Coroutine movementDirectionCache;
    float speedBeforeGrapple = 0f;
    Vector2 previousPosition;
    Vector2 currentPosition;

    // ---- References ----
    Vector2 refVelocity; 
    #endregion

    #region STATE VARIABLES
    static bool isGrappling;
    static bool isOnAnchor;
    static bool canGrapple = true;
    #endregion

    #region GETTERS & SETTERS
    // ---- STATES ----
    public static bool IS_GRAPPLING{
        get{
            return isGrappling;
        }
    }

    public static bool IS_ON_ANCHOR{
        get{
            return isOnAnchor;
        }
    }

    public static bool CAN_GRAPPLE{
        get{
            return canGrapple;
        }
    }

    // ---- OTHERS ----
    public static Vector2 GRAPPLE_END_POINT{
        get{
            return grappleEndPoint;
        }
    }

    public static Vector2 GRAPPLE_START_POINT{
        get{
            return grappleStartPoint;
        }
    }

    public static Vector2? ANCHOR_POSITION{
        get{
            if(ANCHOR == null)
                return null;
            return ANCHOR.transform.position;
        }
    }
    #endregion

    #region EXECUTION METHODS
    private void Start(){
        ORIGINAL_GRAVITY_SCALE = rigidBody.gravityScale;

        AnchorDetectionDistance = GetScreenDiagonalLength();

        PlayerInputManager.Maps.Player.Grapple.performed += _ => inputHoldReceived = true;
        PlayerInputManager.Maps.Player.Grapple.canceled += _ => inputHoldReceived = false;
    }

    private void Update(){
        if(!canGrapple)
            return;

        // Not grappling -> Grappling -> Reached Anchor
        if(!isGrappling && !isOnAnchor){ // TODO: Allow grappling to multiple anchors
            ANCHOR = FindAnchor();
            if(ANCHOR == null)
                return;
            if(!inputReceived)
                return;
            
            StartCoroutine(DelayInputHold()); // In case if the player clicks right next to the anchor
                                              // This way they are given time to react if the dont
                                              // wanna get launched straight out of the anchor
                                              // or just get attached to it
            DisableGravity();
            DisableAnchorDetection();
            DisableOtherMovementMechanics();
            basicMovement.RemoveFriction();
            speedBeforeGrapple = GetSpeedIgnoringFallVelocity();
            isGrappling = true;
        }
        else if(isGrappling && !isOnAnchor){
            grappleStartPoint = transform.position;

            if(GetDistanceToAnchor() > closureDistance){
                // This is done before we reach the colsure distance
                // since the direction can change quite drastically
                // and we just want the general movement direction
                CalculateMovementDirection();
            }

            if(GetDistanceToAnchor() > distanceToAttach){
                /*  TODO
                if dash input received:
                    EnableOtherMovementMechanics();
                    EnableAnchorDetection();
                    EnableGravity();
                    isGrappling = false;

                    dash.Dash();
                */

                // if cancled grappling:
                if(cancelInputReceived)
                    Reset();
            }
            else{ // Reached the anchor
                if(inputHoldReceived){
                    ImpulsePlayerInDirection(movementDirection, CalculateDetachForce() + speedBeforeGrapple);
                    Reset();
                    return;
                }
            
                AttachToAnchor();
                isGrappling = false;
                isOnAnchor = true;
            }
        }
        else if(isOnAnchor){
            if(jumpInputReceived){
                DetachFromAnchor();
                isOnAnchor = false;
                isGrappling = false;
                return;
            }

            GameObject nextAnchor = FindAnchor();
            if(nextAnchor == null)
                return;
            if(!inputReceived)
                return;

            EnableAnchorDetection();
            ANCHOR = nextAnchor;
            DisableAnchorDetection();
            isOnAnchor = false;
            isGrappling = true;
        }
        
    }
    
    private void FixedUpdate() {
        if(isGrappling && !isOnAnchor){
            MoveTowardsAnchor();
        }
    }
    #endregion
    
    #region FINDING THE ANCHOR
    GameObject FindAnchor(){
        Vector3 direction  = GetPlayerToMouseDirection(); 
        RaycastHit2D grappleRay = Physics2D.Raycast(transform.position, direction, AnchorDetectionDistance, collisionLayers);
        
        // DEBUGGING
        if(enableDebugging && drawDetectionLine)
            Debug.DrawLine(transform.position, grappleRay.point, Color.yellow);

        if (AnchorDetected(grappleRay.collider)){
            grappleEndPoint = grappleRay.point;
            return grappleRay.collider.gameObject;
        }

        return null;
    }

    Vector3 GetPlayerToMouseDirection(){
        Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        direction.z = 0f;
        direction.Normalize();

        return direction;
    }

    bool AnchorDetected(Collider2D collider){
        if(collider == null)
            return false;

        // layer.value = 1 << layerNumber = 2^layerNumber
        int anchorLayerNumber = (int)Mathf.Log(anchorLayer.value, 2);
        if(collider.gameObject.layer != anchorLayerNumber)
            return false; 

        if(!CheckIfAnchorIsVisible(collider.gameObject))
            return false;

        return true;
    }

    bool CheckIfAnchorIsVisible(GameObject anchor){
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        Collider2D anchorCollider = anchor.GetComponent<Collider2D>();

        if (GeometryUtility.TestPlanesAABB(planes, anchorCollider.bounds))
            return true;
        return false;
    }
    #endregion

    #region MOVING TOWARDS THE ANCHOR
    void MoveTowardsAnchor(){
        if(rigidBody.gravityScale != NO_GRAVITY)
            DisableGravity();

        Vector3 direction = GetAnchorDirection();

        if (GetDistanceToAnchor() < closureDistance){
            TranslateToAnchor();
            return;
        }

        ApplyVelocityTowardsAnchor();
    }

    Vector3 GetAnchorDirection(){
        Vector3 direction = ANCHOR.transform.position - transform.position;
        direction.z = 0;
        direction.Normalize();

        return direction;
    }

    float GetDistanceToAnchor(){
        return Vector2.Distance(gameObject.transform.position, ANCHOR.transform.position);
    }

    void TranslateToAnchor(){
        if(!velocityRemoved){
            rigidBody.velocity = Vector2.zero;
            velocityRemoved = true;
        }
        transform.Translate(GetAnchorDirection() * Time.deltaTime * closureSpeed);
    }

    void ApplyVelocityTowardsAnchor(){
        Vector2 direction = GetAnchorDirection();
        rigidBody.velocity = Vector2.SmoothDamp(rigidBody.velocity, GetAnchorDirection() * speed, ref refVelocity, accelerationTime);
    }
    #endregion

    #region ATTACHING AND DETACHING FROM ANCHOR
    void AttachToAnchor(){
        rigidBody.velocity = Vector2.zero;
        transform.position = ANCHOR.transform.position + (Vector3)attachOffset;
    }

    void DetachFromAnchor(){
        Vector3 direction = GetPlayerToMouseDirection();
        ImpulsePlayerInDirection(direction, CalculateDetachForce() + speedBeforeGrapple);
        Reset();
    }

    void ImpulsePlayerInDirection(Vector2 direction, float force){
        rigidBody.velocity = direction * force;
    }

    float CalculateDetachForce(){
        return speedBeforeGrapple <= maxDetachSpeed ? highDetachForce : maxDetachSpeed + lowDetachForce;
    }
    #endregion

    #region OTHER
    void CalculateMovementDirection(){
        if((Vector2)transform.position == previousPosition)
            return;
        movementDirection = ((Vector2)transform.position - previousPosition).normalized;
        previousPosition = transform.position;
    }
    IEnumerator DelayInputHold(){
        inputHoldReceived = false;
        yield return new WaitForSeconds(0.2f);
        inputHoldReceived = PlayerInputManager.Maps.Player.Grapple.ReadValue<float>() > 0.0f;
    }
    float GetSpeedIgnoringFallVelocity(){
        Vector2 velocity = Vector2.zero;
        velocity.x = rigidBody.velocity.x;
        velocity.y = rigidBody.velocity.y > 0.0f ? rigidBody.velocity.y : 0.0f;
        return velocity.magnitude;
    }

    void Reset(){
        StartDelay();
        EnableOtherMovementMechanics();
        EnableAnchorDetection();
        EnableGravity();
        speedBeforeGrapple = 0f;
        velocityRemoved = false;
        isGrappling = false;
    }

    void StartDelay(){
        if(delayCache == null)
            delayCache = StartCoroutine(StartDelayHandler());
    }

    IEnumerator StartDelayHandler(){
        canGrapple = false;
        yield return new WaitForSeconds(delayTime); 
        canGrapple = true;

        delayCache = null;
    }
    float GetScreenDiagonalLength(){
        // TODO
        return 100f;
    }

    void DisableOtherMovementMechanics(){
        if(BasicMovement.IsMovementActive)
            BasicMovement.IsMovementActive = false;
        if(BasicMovement.IsJumpingActive)
            BasicMovement.IsJumpingActive = false;

        /* TODO:
        if(roll.enabled)
            roll.enabled = false;

        if(dash.enabled)
            dash.enabled = false;
        */
    }

    void EnableOtherMovementMechanics(){
        if(!BasicMovement.IsMovementActive)
            BasicMovement.IsMovementActive = true;
        if(!BasicMovement.IsJumpingActive)
            BasicMovement.IsJumpingActive = true;

        /* TODO:
        if(!roll.enabled)
            roll.enabled = true;

        if(!dash.enabled)
            dash.enabled = true;
        */ 
    }

    void DisableAnchorDetection(){
        // Collider2D collider = ANCHOR.GetComponent<Collider2D>();
        CircleCollider2D collider = ANCHOR.GetComponent<CircleCollider2D>();
        if(collider.enabled)
            collider.enabled = false;
    }

    void EnableAnchorDetection(){
        CircleCollider2D collider = ANCHOR.GetComponent<CircleCollider2D>();
        if(!collider.enabled)
            collider.enabled = true;
    }

    void DisableGravity(){
        rigidBody.gravityScale = 0f;
    }

    void EnableGravity(){
        rigidBody.gravityScale = ORIGINAL_GRAVITY_SCALE;
    }
    #endregion

    #region INPUT
    public void GetInput(InputAction.CallbackContext context){
        inputReceived = context.performed ? true : false;
    }

    public void GetJumpInput(InputAction.CallbackContext context){
        jumpInputReceived = context.started ? true : false; 
    }

    public void GetCancelInput(InputAction.CallbackContext context){
        cancelInputReceived = context.started ? true : false;
    }
    #endregion
}
