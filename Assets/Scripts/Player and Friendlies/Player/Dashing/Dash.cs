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
    [Tooltip("How much time before the player will appear at the end point of the dash")]
    [Range(0f, 5f)]
    [SerializeField] float dashingTime; 
    [Tooltip("How much time before the player restores movement")]
    [Range(0f, 5f)]
    [SerializeField] float holdTime;

    [Space]
    [Header("Required Componenets & Parameters")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Rigidbody2D rigidbody;
    [SerializeField] Collider2D collider;
    [SerializeField] SpriteRenderer playerSpriteRenderer;
    [SerializeField] SpriteRenderer companionSpriteRenderer;
    [SerializeField] BasicMovement basicMovement;
    [SerializeField] Grapple grapple;
    #endregion
    
    #region STATES
    static bool isDashing;
    static bool isHolding; // when the player is holding in the air waiting for either the delay or cancel input
    #endregion

    #region PRIVATE VARIABLES
    Vector2 direction;
    float lastSpeed;
    float delayCurrentTime = 0f;

    // ---- CACHE ----
    Coroutine dashingTimeCache;
    float ORIGINAL_GRAVITY_SCALE;
    Vector2 ORIGINAL_VELOCITY;
    #endregion

    #region GETTERS & SETTERS
    public static bool IS_DASHING{
        get{
            return isDashing;
        }
    }

    public static bool IS_HOLDING{
        get{
            return isHolding;
        }
    }
    #endregion

    #region EXECUTION
    void Awake() {
        ORIGINAL_GRAVITY_SCALE = rigidbody.gravityScale;  
        ORIGINAL_VELOCITY = rigidbody.velocity;
    }

    void Update(){
        if(PlayerInputManager.Maps.Player.Dash.triggered){
            direction = GetPlayerToMouseDirection();
            lastSpeed = GetSpeed();
            
            RemoveVelocity();
            DisableGravity();
            DisableOtherMovementMechanics();
            HidePlayer();
            HideCompanion();
            DisableCollision();


            StartDashTime();
            isHolding = false; // allows us to dash while we are still holding
        }

        if(isHolding)
            HoldDelay();
    }
    #endregion

    #region DASH
    void ApplyDash(){
        RaycastHit2D ray = CastRayInDirection();
        transform.position = GetEndPoint(ray);
    }

    RaycastHit2D CastRayInDirection(){
        return Physics2D.Raycast(transform.position, direction, length, groundLayer);
    }

    Vector2 GetEndPoint(RaycastHit2D ray){
        return ray.collider == null ? (Vector2)transform.position + direction * length : ray.point - direction * collisionOffset;
    }

    Vector2 GetPlayerToMouseDirection(){
        Vector3 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        dir.z = 0f;
        dir.Normalize();
        return dir;
    }
    #endregion

    #region DASHING TIME
    void StartDashTime(){
        if(dashingTimeCache == null)
            dashingTimeCache = StartCoroutine(DashTime());
    }

    IEnumerator DashTime(){

        isDashing = true;
        yield return new WaitForSeconds(dashingTime);
        isDashing = false;
        dashingTimeCache = null;
        
        ApplyDash();
        
        SetHoldDelay();
        ShowPlayer();
        ShowCompanion();
        EnableCollision();
    }
    #endregion

    #region HOLD DELAY
    void SetHoldDelay(){
        if(isHolding)
            return;

        isHolding = true;
        delayCurrentTime = holdTime;
    }

    void HoldDelay(){
        if(delayCurrentTime > 0.0f){
            delayCurrentTime -= Time.deltaTime;
            return;
        }

        ImpulsePlayerInDirection();
        
        EnableGravity();
        EnableOtherMovementMechanics();
        isHolding = false;
    }
    #endregion

    #region IMPULSE
    void ImpulsePlayerInDirection(){
        Debug.Log(force + lastSpeed);
        rigidbody.velocity =  direction * (force + lastSpeed);
    }

    float GetSpeed(){
        Vector2 velocity = rigidbody.velocity;
        if(velocity.y < 0f)
            velocity.y = 0f;
        return velocity.magnitude;
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

    void HidePlayer(){
        if(playerSpriteRenderer.enabled)
            playerSpriteRenderer.enabled = false;
    }

    void ShowPlayer(){
        if(!playerSpriteRenderer.enabled)
            playerSpriteRenderer.enabled = true;
    }

    void HideCompanion(){
        if(companionSpriteRenderer.enabled)
            companionSpriteRenderer.enabled = false;
    }

    void ShowCompanion(){
        if(!companionSpriteRenderer.enabled)
            companionSpriteRenderer.enabled = true;
    }

    void DisableCollision(){
        if(collider.enabled)
            collider.enabled = false;
    }

    void EnableCollision(){
        if(!collider.enabled)
            collider.enabled = true;
    }
    #endregion
}