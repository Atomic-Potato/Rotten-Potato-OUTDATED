using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class pDash : MonoBehaviour
{
    #region Inspector variables
    [Range(0, 100)]
    [SerializeField] int count = 1;
    [Range(0.1f, 100f)]
    [SerializeField] float distance;
    [Range(0.1f, 5f)]
    [SerializeField] float time;

    [Space]
    [Header("Hold & Hold Jump")]
    [Tooltip("Negative time means infinite time.")]
    [Range(-1f, 5f)]
    [SerializeField] float holdTime;
    [Range(0.1f, 100f)]
    [SerializeField] float holdJumpDistance;
    [Range(0.1f, 5f)]
    [SerializeField] float holdJumpTime;

    [Space]
    [Range(0, 5)]
    [SerializeField] int restoredDashesCount;

    [Space]
    [SerializeField] Rigidbody2D rigidbody;
    [Tooltip("The collider that is used for solid/static objects collisions such as the ground.")]
    [SerializeField] Collider2D playerCollider;
    [Tooltip("The collider used to detect hitting enemies.")]
    [SerializeField] Collider2D detectionCollider;
    [SerializeField] PhysicsMaterial2D noFrictionMaterial;

    [Space]
    [Header("Gizmos")]
    [SerializeField] bool gizmosDrawDistance;
    [SerializeField] bool gizmosUseDistanceDirectionToMouse;
    [SerializeField] Vector2 gizmosDistanceDirection = Vector2.right;
    #endregion

    #region Global Variables
    static pDash _instance;
    public static pDash Instance => _instance;

    static bool isDashingCache;
    public static bool IsDashing => isDashingCache;
    static bool isHoldingCache;
    public static bool IsHolding => isHoldingCache;
    static bool isDamagedDashingCache;
    public static bool IsDamagedDashing => isDamagedDashingCache;

    static int _dashesLeft;
    public static int DashesLeft => _dashesLeft;
    float _initialVelocity;
    float _initialGravity;
    float _holdTimer;
    float _dashTimer;
    Vector2? _direction;
    Coroutine _dashCache;
    Coroutine _damageDashCache;
    Coroutine _jumpCache;

    bool _isCanDash;
    bool _isCanHold;
    bool _isHolding;
    bool _isDashing;
    bool _isDamagedDashing;
    bool _isReceivingDashInput;
    bool _isReceivingJumpInput;

    public Action<int> AlterDashCount;
    #endregion

    #region Execution
    void OnDrawGizmos() 
    {
        if (gizmosDrawDistance)
        {
            Gizmos.color = Color.green;

            Vector2 direction = gizmosUseDistanceDirectionToMouse ? (Vector2)GetAimingDirection() : gizmosDistanceDirection;
            Gizmos.DrawRay(transform.position, direction * distance);
        }
    }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;    
        }

        AlterDashCount = UpdateDashCount;

        _initialVelocity = GetInitialVelocityNoAcceleration(distance, time);
        _initialGravity = rigidbody.gravityScale;
        AlterDashCount?.Invoke(count - _dashesLeft);
    }

    void Update()
    {
        UpdateCache();

        if (IsAbleToDash())
        {
            Dash();
        }

        if (_isCanHold)
        {
            Hold();
        }

        if (BasicMovement.IS_GROUNDED && !_isDashing && !_isHolding && _dashesLeft < count)
        {
            AlterDashCount?.Invoke(count - _dashesLeft);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Collider2D otherCollider = other.gameObject.GetComponent<Collider2D>();
        if (!detectionCollider.IsTouching(otherCollider))
        {
            return;
        }


        if(!_isDashing || _isDamagedDashing)
        {
            return;
        }
        
        if (other.gameObject.tag == "Enemy")
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            enemy.Damage();
            StopDash();
            AlterDashCount?.Invoke(restoredDashesCount);
        }
    }
    #endregion

    void UpdateCache()
    {
        if (isHoldingCache != _isHolding)
            isHoldingCache = _isHolding;
        if (isDashingCache != _isDashing)
            isDashingCache = _isDashing;
        if (isDamagedDashingCache != _isDamagedDashing)
            isDamagedDashingCache = _isDamagedDashing;
    }

    #region Dash
    void Dash()
    {
        if (_dashesLeft <= 0 && _dashTimer >= time)
        {
            _isCanDash = false;
            return;
        }

        if (!_isDashing)
        {
            if (_isHolding)
            {
                StopHolding();
            }

            _direction = GetAimingDirection();
            if (_direction == null)
            {
                AbortDash();
                return;
            }

            _isDashing = true;
            _dashTimer = 0f;
            AlterDashCount?.Invoke(-1);
            StopMovement();

            rigidbody.AddForce(_initialVelocity * (Vector2)_direction, ForceMode2D.Impulse);
        }

        if (_dashTimer < time)
        {
            _dashTimer += Time.deltaTime;
        }

        if (_dashTimer >= time)
        {
            StopDash();
        }
    }

    void StopHolding()
    {
        _isCanHold = false;
        _isHolding = false;
    }

    public void Dash(bool isDamageDash, Vector2? direction, float time = 0.1f, float distance = 3)
    {
        if (direction == null)
        {
            return;
        }

        if (_damageDashCache == null)
        {
            _damageDashCache = StartCoroutine(ExecuteDash());
        }

        IEnumerator ExecuteDash()
        {
            if (isDamageDash)
            {
                _isDamagedDashing = true;
            }
            else
            {
                AlterDashCount?.Invoke(-1);
            }

            _isDashing = true;

            if (_isHolding)
            {
                StopHolding();
            }

            StopMovement();
            float initialVelocity = GetInitialVelocityNoAcceleration(distance, time);

            rigidbody.AddForce(initialVelocity * (Vector2)direction, ForceMode2D.Impulse);
            yield return new WaitForSeconds(time);

            StopDash();
            _damageDashCache = null;
        }
    }

    bool IsAbleToDash()
    {
        if (_isReceivingDashInput)
        {
            _isCanDash = true;
        }

        return _isCanDash;
    }

    void StopMovement()
    {
        BasicMovement.IsMovementActive = false;
        BasicMovement.IsJumpingActive = false;
        rigidbody.velocity = Vector2.zero;
        rigidbody.gravityScale = 0f;
    }

    public void StopDash()
    {
        rigidbody.velocity = Vector2.zero;
        _isCanDash = false;
        _isCanHold = true;
        _isDamagedDashing = false;
        _isDashing = false;
    }

    public void AbortDash()
    {
        _isCanDash = false;
        _isDamagedDashing = false;
        _isDashing = false;
    }
    #endregion

    void Hold()
    {
        if (!_isHolding)
        {
            _isHolding = true;
            _holdTimer = 0f;
            StopMovement();
        }

        if (_isReceivingJumpInput)
        {
            Jump();
            StopHolding();
        }

        if (holdTime > 0f)
        {
            if (_isHolding)
            {
                _holdTimer += Time.deltaTime;
            }

            if(_holdTimer >= holdTime)
            {
                StopHolding();
                RemoveFriction();
            }
        }

        #region Local Methods
        void StopHolding()
        {
            _isHolding = false;
            _isCanHold = false;
            RestoreMovement();
        } 

        

        void RemoveFriction()
        {
            playerCollider.sharedMaterial = noFrictionMaterial;
        }

        void StopMovement()
        {
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

    #region Methods

    void UpdateDashCount(int i)
    {
        _dashesLeft += i;

    }

    void RestoreMovement()
    {
        rigidbody.gravityScale = _initialGravity;
        BasicMovement.IsMovementActive = true;
        BasicMovement.IsJumpingActive = true;
    }

    public Vector2? GetAimingDirection()
    {
        if (PlayerInputManager.IsUsingGamePad)
        {
            if (PlayerInputManager.Aim == Vector2.zero)
            {
                if (PlayerInputManager.DirectionRaw == Vector2.zero)
                {
                    return null;
                }
                return PlayerInputManager.DirectionRaw;
            }
            else if (PlayerInputManager.Aim != Vector2.zero)
            {
                return PlayerInputManager.Aim.normalized;
            }
            return null;
        }
        else
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mousePosition - transform.position;
            direction.Normalize();
            return direction;
        }
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
    #endregion

    #region Input
    public void DetectDashInput(InputAction.CallbackContext context)
    {
        _isReceivingDashInput = context.performed;

        if (_isReceivingDashInput && _dashCache == null)
        {
            _dashCache = StartCoroutine(ResetInputNextFrame());
        }

        IEnumerator ResetInputNextFrame()
        {
            yield return null;
            _isReceivingDashInput = false;
            _dashCache = null;
        }
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
    #endregion
}
