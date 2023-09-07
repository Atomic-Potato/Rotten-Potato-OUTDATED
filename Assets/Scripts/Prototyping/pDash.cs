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
    Vector2 _direction;
    Coroutine _dashCache;
    Coroutine _jumpCache;

    public bool _isDashing;
    public bool IsDashing => _isDashing;
    public bool _isReceivingJumpInput;

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
        Debug.Log(_isReceivingJumpInput);

        if (BasicMovement.IS_GROUNDED)
        {
            _dashesLeft = count;
        }
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
            float holdTimer = 0f;

            _isDashing = true;   
            _direction = GetMouseDirection();

            DisableHostileCollision();
            RemoveFriction();
            StopMovement();

            rigidbody.AddForce(_initialVelocity * _direction, ForceMode2D.Impulse);
            yield return new WaitForSeconds(time);
            rigidbody.velocity = Vector2.zero;

            while (holdTimer < holdTime)
            {
                Debug.Log(holdTimer);
                if (_isReceivingJumpInput)
                {
                    float initialJumpVelocity = GetInitialVelocity(holdJumpDistance, holdJumpTime);
                    rigidbody.AddForce(initialJumpVelocity * Vector2.up, ForceMode2D.Impulse);
                    break;
                }

                holdTimer += Time.deltaTime;
                yield return null;
            }

            RestoreMovement();
            EnableHostileCollision();          

            _dashesLeft--;
            _dashCache = null;
            _isDashing = false;
        }

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

        void RestoreMovement()
        {
            rigidbody.gravityScale = _initialGravity;
            basicMovement.enabled = true;
        }

        void RemoveFriction()
        {
            collider.sharedMaterial = noFrictionMaterial;
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
