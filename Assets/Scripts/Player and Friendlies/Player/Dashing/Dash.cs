using System.Collections;
using UnityEngine;

public class Dash : MonoBehaviour{
    #region INSPECTOR VARIABLES
    [Tooltip("The maximum ammount of dashes the player has")]
    [Range(0, 100)]
    [SerializeField] int maximumDashes;    
    [Tooltip("How far can the player dash")]
    [Range(0f, 100f)]
    [SerializeField] float length;
    [Tooltip("How far from the wall the player will be spawned if theres not enough space to dash")]
    [Range(0f, 100f)]
    [SerializeField] float collisionOffset;
    [Tooltip("How much force is added to the player momentum at the end of the dash")]
    [SerializeField] float force;
    [Tooltip("How much time before the player restores movement")]
    [Range(0f, 5f)]
    [SerializeField] float holdTime;

    [Space]
    [Header("Required Componenets & Parameters")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Rigidbody2D rigidbody;
    [SerializeField] BasicMovement basicMovement;
    [SerializeField] Grapple grapple;
    #endregion
    
    #region STATES
    bool isHolding; // when the player is holding in the air waiting for either the delay or cancel input
    #endregion

    #region PRIVATE VARIABLES
    float delayCurrentTime = 0f;

    // ---- CACHE ----
    Coroutine delayCache;
    float ORIGINAL_GRAVITY_SCALE;
    Vector2 ORIGINAL_VELOCITY;
    #endregion


    #region EXECUTION
    void Awake() {
        ORIGINAL_GRAVITY_SCALE = rigidbody.gravityScale;  
        ORIGINAL_VELOCITY = rigidbody.velocity;
    }

    void Update(){
        if(PlayerInputManager.Maps.Player.Dash.triggered){
            RemoveVelocity();
            DisableGravity();
            DisableOtherMovementMechanics();

            ApplyDash();
            isHolding = false; // allows us to dash while we are still holding
            SetDelay();
        }

        if(isHolding)
            Delay();
    }
    #endregion

    #region DASH
    void ApplyDash(){
        Vector2 direction = GetPlayerToMouseDirection();
        RaycastHit2D ray = CastRayInDirection(direction);
        transform.position = GetEndPoint(ray, direction);
    }

    RaycastHit2D CastRayInDirection(Vector2 direction){
        return Physics2D.Raycast(transform.position, direction, length, groundLayer);
    }

    Vector2 GetEndPoint(RaycastHit2D ray, Vector2 direction){
        return ray.collider == null ? (Vector2)transform.position + direction * length : ray.point - direction * collisionOffset;
    }

    Vector2 GetPlayerToMouseDirection(){
        Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        direction.z = 0f;
        direction.Normalize();
        return direction;
    }
    #endregion

    #region DELAY
    void SetDelay(){
        if(isHolding)
            return;

        isHolding = true;
        delayCurrentTime = holdTime;
    }

    void Delay(){
        // Debug.Log("time :" + delayCurrentTime);
        if(delayCurrentTime > 0.0f){
            delayCurrentTime -= Time.deltaTime;
            return;
        }

        EnableGravity();
        EnableOtherMovementMechanics();        
        isHolding = false;
    }
    #endregion

    #region OTHER
    void DisableOtherMovementMechanics(){
        if(basicMovement.enabled)
            basicMovement.enabled = false;
        if(grapple.enabled)
            grapple.enabled = false;
    }

    void EnableOtherMovementMechanics(){
        if(!basicMovement.enabled)
            basicMovement.enabled = true;
        if(!grapple.enabled)
            grapple.enabled = true;
    }

    void DisableGravity(){
        rigidbody.gravityScale = 0f;
    }

    void EnableGravity(){
        rigidbody.gravityScale = ORIGINAL_GRAVITY_SCALE;
    }

    void RemoveVelocity(){
        rigidbody.velocity = Vector2.zero;
    }
    #endregion
}