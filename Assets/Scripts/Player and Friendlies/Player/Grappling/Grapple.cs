using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grapple : MonoBehaviour{

    #region INSPECTOR VARIABLES
    [SerializeField] float speed = 10f;
    [SerializeField] float accelerationTime = 10f;
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

    [Space]
    [Header("Required Components")]
    [SerializeField] LayerMask anchorLayer;
    [SerializeField] Rigidbody2D rigidBody;

    [Space]
    [SerializeField] BasicMovement basicMovement;
    // [SerializeField] Roll roll;
    // [SerializeField] Dash dash;
    #endregion

    #region PRIVATE VARAIBALES
    bool inputReceived;
    float AnchorDetectionDistance;
    GameObject ANCHOR;

    // ---- CONSTANTS ----
    const float NO_GRAVITY = 0f;
    #endregion

    #region STATE VARIABLES
    static bool isGrappling;
    static bool isOnAnchor;

    // ---- References ----
    Vector2 refVelocity; 
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
    #endregion

    #region EXECUTION METHODS
    void Start(){
        AnchorDetectionDistance = GetScreenDiagonalLength();
    }

    void Update(){
        // Not grappling -> Grappling -> Reached Anchor
        if(!isGrappling){ // TODO: Allow grappling to multiple anchors
            ANCHOR = FindAnchor();
            if(ANCHOR == null)
                return;
            
            if(!inputReceived)
                return;
            
            DisableAnchorDetection();
            DisableOtherMovementMechanics();
            isGrappling = true;
        }
        else if(isGrappling){
            if(GetDistanceToAnchor() > distanceToAttach){
                MoveTowardsAnchor();
                /*  TODO
                if dash input received:
                    EnableOtherMovementMechanics();
                    EnableAnchorDetection();
                    EnableGravity();
                    isGrappling = false;

                    dash.Dash();
                
                if cancled grappling:
                    EnableOtherMovementMechanics();
                    EnableAnchorDetection();
                    EnableGravity();
                    isGrappling = false;
                */
            }
            else{ // Reached the anchor
                AttachToAnchor();
                isGrappling = false;
                isOnAnchor = true;
                return;
            }
        }
        else if(isOnAnchor){
            /*
            get player aiming direction

            if jump input received:
                launch player in aiming direction
                EnableAnchorDetection();
                EnableOtherMovementMechanics();
                EnableGavity();
            */
        }
        
    }
    #endregion
    
    #region FINDING THE ANCHOR
    GameObject FindAnchor(){
        Vector3 direction  = GetPlayerToMouseDirection(); 
        RaycastHit2D grappleRay = Physics2D.Raycast(transform.position, direction, AnchorDetectionDistance, anchorLayer);
        
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
        return grappleRay.collider != null;
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

    #region OTHER
    void AttachToAnchor(){
        rigidBody.velocity = new Vector3(0f,0f,0f);
        transform.position = ANCHOR.transform.position + (Vector3)attachOffset;
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
        float input = context.ReadValue<float>();
        inputReceived = input == 1 ? true : false; 
    }
    #endregion
}
