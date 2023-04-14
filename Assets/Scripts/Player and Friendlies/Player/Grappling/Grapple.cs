using System.Collections;
using System.Collections.Generic;
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
    [Tooltip("The ammount of force the player will be launced in after jumping off the anchor")]
    [SerializeField] float detachForce;

    [Space]
    [Header("Required Components")]
    [SerializeField] LayerMask anchorLayer;
    [Tooltip("The layers that will block the detection of the anchor (including the anchor layer)")]
    [SerializeField] LayerMask collisionLayers;
    [SerializeField] Rigidbody2D rigidBody;

    [Space]
    [SerializeField] BasicMovement basicMovement;
    // [SerializeField] Roll roll;
    // [SerializeField] Dash dash;
    #endregion

    #region PRIVATE VARAIBALES
    float AnchorDetectionDistance;
    GameObject ANCHOR;

    // ---- INPUT ----
    bool inputReceived;
    bool inputHoldReceived;
    bool jumpInputReceived;
    bool cancelInputReceived;

    // ---- CONSTANTS ----
    const float NO_GRAVITY = 0f;

    // ---- CACHE ----
    Coroutine delayCache;
    // ---- References ----
    Vector2 refVelocity; 
    #endregion

    #region STATE VARIABLES
    static bool isGrappling;
    static bool isOnAnchor;
    static bool canGrapple = true;
    #endregion

    #region GETTERS & SETTERS
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
    #endregion

    #region EXECUTION METHODS
    private void Start(){
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
            
            DisableAnchorDetection();
            DisableOtherMovementMechanics();
            basicMovement.RemoveFriction();
            isGrappling = true;
        }
        else if(isGrappling && !isOnAnchor){
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
        if(isGrappling){
            MoveTowardsAnchor();
        }
    }
    #endregion
    
    #region FINDING THE ANCHOR
    GameObject FindAnchor(){
        Vector3 direction  = GetPlayerToMouseDirection(); 
        RaycastHit2D grappleRay = Physics2D.Raycast(transform.position, direction, AnchorDetectionDistance, collisionLayers);
        
        Debug.DrawLine(transform.position, grappleRay.point, Color.yellow);

        if (AnchorDetected(grappleRay)){
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

    bool AnchorDetected(RaycastHit2D grappleRay){
        if(grappleRay.collider == null)
            return false;

        // layer.value = 1 << layerNumber = 2^layerNumber
        int anchorLayerNumber = (int)Mathf.Log(anchorLayer.value, 2);
        if(grappleRay.collider.gameObject.layer != anchorLayerNumber)
            return false; 

        return true;
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
        transform.Translate(GetAnchorDirection() * Time.deltaTime * closureSpeed);
    }

    void ApplyVelocityTowardsAnchor(){
        Vector2 direction = GetAnchorDirection();
        rigidBody.velocity = Vector2.SmoothDamp(rigidBody.velocity, GetAnchorDirection() * speed, ref refVelocity, accelerationTime);
    }
    #endregion

    #region ATTACHING AND DETACHING FROM ANCHOR
    void AttachToAnchor(){
        rigidBody.velocity = new Vector3(0f,0f,0f);
        transform.position = ANCHOR.transform.position + (Vector3)attachOffset;
    }

    void DetachFromAnchor(){
        StartDelay();
        EnableGravity();
        EnableAnchorDetection();

        Vector3 direction = GetPlayerToMouseDirection();
        ImpulsePlayerInDirection(direction);

        EnableOtherMovementMechanics();
    }

    void ImpulsePlayerInDirection(Vector3 direction){
        rigidBody.AddForce(direction * (detachForce - rigidBody.gravityScale), ForceMode2D.Impulse);
    }
    #endregion

    #region OTHER
    void Reset(){
        StartDelay();
        EnableOtherMovementMechanics();
        EnableAnchorDetection();
        EnableGravity();
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
        if(basicMovement.enabled)
            basicMovement.enabled = false;

        /* TODO:
        if(roll.enabled)
            roll.enabled = false;

        if(dash.enabled)
            dash.enabled = false;
        */
    }

    void EnableOtherMovementMechanics(){
       if(!basicMovement.enabled)
            basicMovement.enabled = true;

        /* TODO:
        if(!roll.enabled)
            roll.enabled = true;

        if(!dash.enabled)
            dash.enabled = true;
        */ 
    }

    void DisableAnchorDetection(){
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
        // TO DO
        rigidBody.gravityScale = 5f;
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
