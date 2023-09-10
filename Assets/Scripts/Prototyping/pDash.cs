using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;

public class pDash : MonoBehaviour
{
    [Range(0, 100)]
    [SerializeField] int count = 1;
    [Range(0.1f, 100f)]
    [SerializeField] float distance;
    [Range(0.1f, 5f)]
    [SerializeField] float time;

    [Space]
    [Header("Hold & Hold Jump")]
    [Range(0f, 5f)]
    [SerializeField] float holdTime;
    [Range(0.1f, 100f)]
    [SerializeField] float holdJumpDistance;
    [Range(0.1f, 5f)]
    [SerializeField] float holdJumpTime;

    [Space]
    [Range(0, 1)]
    [SerializeField] int restoredDashesCount;

    [Space]
    [SerializeField] Rigidbody2D rigidbody;
    [SerializeField] Collider2D collider;
    [SerializeField] PhysicsMaterial2D noFrictionMaterial;
    [SerializeField] BasicMovement basicMovement;

    [Space]
    [Header("Gizmos")]
    [SerializeField] bool gizmosDrawDistance;
    [SerializeField] bool gizmosUseDistanceDirectionToMouse;
    [SerializeField] Vector2 gizmosDistanceDirection = Vector2.right;

    int _dashesLeft;
    float _initialVelocity;
    float _initialGravity;
    float _holdTimer;
    Vector2 _direction;
    Coroutine _dashCache;
    Coroutine _jumpCache;

    bool _isCanHold;
    bool _isHolding;
    bool _isDashing;
    public bool IsDashing => _isDashing;
    bool _isReceivingJumpInput;

    void OnDrawGizmos() 
    {
        if (gizmosDrawDistance)
        {
            Gizmos.color = Color.green;

            Vector2 direction = gizmosUseDistanceDirectionToMouse ? GetMouseDirection() : gizmosDistanceDirection;
            Gizmos.DrawRay(transform.position, direction * distance);
        }
    }

    void Awake()
    {
        _initialVelocity = GetInitialVelocityNoAcceleration(distance, time);
        _initialGravity = rigidbody.gravityScale;
        _dashesLeft = count;
    }

    void Update()
    {
        if (BasicMovement.IS_GROUNDED && (!_isDashing || !_isHolding))
        {
            _dashesLeft = count;
        }

        if (_isCanHold)
        {
            Hold();
        }

        Debug.Log("Can hold: " + _isCanHold + "\nIs holding: " + _isHolding);
    }

    void OnTriggerEnter(Collider other) 
    {
        if(!_isDashing)
        {
            return;
        }
        
        if (other.gameObject.tag == "Enemy")
        {
            _dashesLeft += restoredDashesCount;
        }    
    }

    public void Dash(InputAction.CallbackContext context)
    { 
        if (IsAbleToDash())
        {
            _dashCache = StartCoroutine(ExecuteDash());
        }

        IEnumerator ExecuteDash()
        {
            _isDashing = true;
            _isCanHold = false;
            _direction = GetMouseDirection();

            DisableHostileCollision();
            StopMovement();

            rigidbody.AddForce(_initialVelocity * _direction, ForceMode2D.Impulse);
            yield return new WaitForSeconds(time);
            rigidbody.velocity = Vector2.zero;

            _isCanHold = true;

            EnableHostileCollision();          

            _dashesLeft--;
            _dashCache = null;
            _isDashing = false;
        }

        #region Local Methods
        bool IsAbleToDash()
        {
            return _dashCache == null && context.performed && _dashesLeft > 0;
        }

        void StopMovement()
        {
            basicMovement.enabled = false;
            rigidbody.velocity = Vector2.zero;
            rigidbody.gravityScale = 0f;
        }

        void DisableHostileCollision()
        {
            // TODO:
            // Removes collision with projectiles and enemies
        }

        void EnableHostileCollision()
        {
            // TODO:
            // Enable collision with projectiles and enemies
        }
        #endregion
    }

    void Hold()
    {
        if (!_isHolding)
        {
            _holdTimer = 0f;
            StopMovement();
        }

        _isHolding = true;

        if (_isReceivingJumpInput)
        {
            Jump();
            StopHolding();
        }

        if (_isHolding)
        {
            _holdTimer += Time.deltaTime;
        }

        if(_holdTimer >= holdTime)
        {
            StopHolding();
            RemoveFriction();
        }

        #region Local Methods
        void StopHolding()
        {
            _isHolding = false;
            _isCanHold = false;
            RestoreMovement();
        } 

        void RestoreMovement()
        {
            rigidbody.gravityScale = _initialGravity;
            basicMovement.enabled = true;
        }

        void RemoveFriction()
        {
            collider.sharedMaterial = noFrictionMaterial;
        }

        void StopMovement()
        {
            basicMovement.enabled = false;
            rigidbody.velocity = Vector2.zero;
            rigidbody.gravityScale = 0f;
        }

        void Jump()
        {
            float initialJumpVelocity = GetInitialVelocity(holdJumpDistance, holdJumpTime);
            rigidbody.AddForce(initialJumpVelocity * Vector2.up, ForceMode2D.Impulse);
        }
        #endregion
    }




    public Vector2 GetMouseDirection()
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        direction.Normalize();
        return direction;
    }

    public float GetInitialVelocityNoAcceleration(float distance, float time)
    {
        // Dervied from the kinematic equations
        return distance / time;
    }

    public float GetInitialVelocity(float distance, float time)
    {
        // Derived from the kinematic equations
        float acceleration = rigidbody.mass * rigidbody.gravityScale;
        return (distance / time) + (0.5f * acceleration * time); 
    }

    public void DetectJumpInput(InputAction.CallbackContext context)
    {    
        _isReceivingJumpInput = context.performed;

        if (_isReceivingJumpInput && _jumpCache == null)
        {
            _jumpCache = StartCoroutine(ResetInputNextFrame());
        }

        IEnumerator ResetInputNextFrame()
        {
            yield return null;
            _isReceivingJumpInput = false;
            _jumpCache = null;
        }
    }
}
