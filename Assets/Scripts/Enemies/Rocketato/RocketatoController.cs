using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class RocketatoController : MonoBehaviour
{
    #region Public Variables
    public string targetTag = "Player";
    public float speed = 10f;
    [SerializeField] float initialRotationSpeed = 20f;
    [Tooltip("If the value is negative distanceToCook will be used")]
    [SerializeField] float timeToCook = 9f;
    [Tooltip("If timeToCook is negative this will be used")]
    [Range(0, 100f)]
    [SerializeField] float distanceToCook = 7f;
    [SerializeField] float cookingTime = 2.5f;
    [SerializeField] float launchedSpeed = 100f;

    [Header("Riding")]
    [SerializeField] float exitJumpForce;
    [SerializeField] Transform playerRidePositionLeft;
    [SerializeField] Transform playerRidePositionRight;
    [SerializeField] CapsuleCollider2D rideTrigger;

    [Header("Knockback")]
    [SerializeField] float enemyKnockLinearDrag = 1f;
    public static float timeToDisableKnock = 1f;
    [SerializeField] float enemyKnockGravityForce = 7f;
    [Space]
    [SerializeField] Vector2 playerKnockForce;
    [SerializeField] Vector2 enemyKnockForce;

    [Header("Self Destruct")]
    [SerializeField] LayerMask destructTriggeringLayers;
    [SerializeField] Animation destructAnimation;
    [SerializeField] BoxCollider2D selfDestructCollider;


    [Header("Required Components")]    
    public Rigidbody2D rigidBody;
    
    [Header("Settings")]
    [SerializeField] bool showLogs;
    #endregion

    #region Private Variables
    float originalSpeed;
    Transform targetTransform;
    Logger logger = null;
    PlayerController playerController;
    GameObject playerObject;
    Collider2D mouseCollider;
    Collider2D playerCollider;

    // State booleans
    bool isMoving;
    bool isCooking;
    bool isLaunched;
    bool isJustStopped;
    bool isDestroyed;

    //Coroutine cache
    Coroutine cookTimeCache = null;
    Coroutine destructCache = null;
    Coroutine rocketDashCache = null;

    //References
    float xVelocityRef = 0f;
    float yVelocityRef = 0f;
    float angVelocityRef = 0f;
    #endregion

    #region Start and FixedUpdate
    private void Start()
    {
        //Doing required calculations
        speed *= 15f;
        launchedSpeed *= 15f;

        //Finding required components
        targetTransform = GameObject.FindGameObjectWithTag(targetTag).transform;
        logger = GameObject.FindGameObjectWithTag("Logger").GetComponent<Logger>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
        playerController = playerObject.GetComponent<PlayerController>();
        playerCollider = playerObject.GetComponent<Collider2D>();
        mouseCollider = GameObject.FindGameObjectWithTag("Mouse").GetComponent<Collider2D>();
    }

    private void Update()
    {
        CheckRideInteraction();    
    }

    private void FixedUpdate()
    {
        if(!(isCooking || isLaunched))
        {
            if(timeToCook >= 0f)
            {
                if(cookTimeCache == null)
                {
                    Log("Started cook time check");
                    cookTimeCache = StartCoroutine(CookInTime());
                }
            }
            else
            {
                Log("Started cook distance check");
                CookInDistance((Vector2)targetTransform.position);
            }
        }

        if(!(isCooking || isLaunched))
            Move(targetTransform.position, speed, initialRotationSpeed);
    }
    #endregion

    #region Movement
    private void Move(Vector3 targetPosition, float velocity, float rotationSpeed)
    {
        isMoving = true;

        Vector3 direction = (targetPosition - transform.position).normalized;
        float rotationMultiplier = Vector3.Cross(direction, transform.right).z;

        rigidBody.angularVelocity = -rotationMultiplier * initialRotationSpeed;
        rigidBody.velocity = transform.right * velocity * Time.deltaTime;
    }

    private IEnumerator Launch(float velocity)
    {
        isLaunched = true;
        while(isLaunched)
        {
            CheckForSelfDestructCollision();
            rigidBody.velocity = transform.right * velocity * Time.deltaTime;
            yield return null;
        }
    }
    #endregion

    #region Cooking
    private IEnumerator Cook()
    {
        Log("Started Cooking");

        isCooking = true;
        isMoving = false;

        // Stop the rocket normal velocity and angular velocity
        Coroutine cache = null;
        while(!isJustStopped)
        {
            if(cache == null)
                cache = StartCoroutine(Stop());
            yield return null;
        }
        isJustStopped = false;

        yield return new WaitForSeconds(cookingTime);

        isCooking = false;

        StartCoroutine(Launch(launchedSpeed));
    }

    private IEnumerator CookInTime()
    {
        yield return new WaitForSeconds(timeToCook);
        StartCoroutine(Cook());
    }

    private void CookInDistance(Vector2 target)
    {
        if(Vector2.Distance(target, (Vector2)transform.position) <= distanceToCook)
            StartCoroutine(Cook());
    }

    private IEnumerator Stop()
    {
        while(rigidBody.angularVelocity < -0.1f || rigidBody.angularVelocity > 0.1f)
        {
            Log("Stopping");
            //normal velocity reduction
            rigidBody.velocity = new Vector2(   Mathf.SmoothDamp(rigidBody.velocity.x, 0f, ref xVelocityRef, 0.25f),
                                                Mathf.SmoothDamp(rigidBody.velocity.y, 0f, ref yVelocityRef, 0.25f));
            //angular velocity reduction
            rigidBody.angularVelocity = Mathf.SmoothDamp(rigidBody.angularVelocity, 0f, ref angVelocityRef, 0.25f);

            yield return null;
        }

        rigidBody.velocity = new Vector2(0f,0f);
        rigidBody.angularVelocity = 0f;

        isJustStopped = true;
    }
    #endregion

    #region Riding
    private void CheckRideInteraction()
    {
        if(CanRideRocket())
        {
            Log("Player rotation: " + playerObject.transform.rotation.z * Mathf.Rad2Deg);
            if(rideTrigger.IsTouching(mouseCollider) && playerController.interactInputReceived)
            {
                Log("Clicked on rocket");
                playerController.isBusy = true;
                rocketDashCache = StartCoroutine(DashToRocket());
            }
        }
    }

    private bool CanRideRocket()
    {
        return  mouseCollider != null && rocketDashCache == null 
                && isCooking && !playerController.isRidingRocket
                && !playerController.isBusy;
    }

    private IEnumerator DashToRocket()
    {
        Vector2 direction = (transform.position - playerController.transform.position).normalized;
        playerController.CustomDash(direction);

        while(!playerCollider.IsTouching(rideTrigger))
            yield return null;

        playerController.StopCustomDash();
        rocketDashCache = null;
        EnterRocket();
    }

    private void EnterRocket()
    {
        playerController.isRidingRocket = true;
        playerController.RemoveGravity();

        //Adjusting angle
        playerController.rigidBody.velocity = new Vector2(0f, 0f);
        playerObject.transform.parent = gameObject.transform;
        //Get the higher ride position
        Vector2 ridePos = playerRidePositionLeft.position.y >= playerRidePositionRight.position.y ?
                            playerRidePositionLeft.position : playerRidePositionRight.position;
        playerController.transform.position = new Vector3(ridePos.x, ridePos.y, playerController.transform.position.z); 
        playerObject.transform.rotation = Quaternion.AngleAxis(0f, Vector3.forward);
    }

    #endregion

    #region Self Destruct
    private void CheckForSelfDestructCollision()
    {
        if(destructCache == null)
        {
            if(selfDestructCollider.IsTouchingLayers(destructTriggeringLayers))
            {
                float animLen = destructAnimation != null ?  destructAnimation.clip.length : 0f;
                destructCache = StartCoroutine(DestroySelf(animLen));
            }
        }
    }
    
    private IEnumerator DestroySelf(float animationLength)
    {
        if(playerController.isRidingRocket)
            FreePlayer();
            
        isDestroyed = true;
        yield return new WaitForSeconds(animationLength);
        Destroy(gameObject);
    }

    private void FreePlayer()
    {
        playerController.isBusy = false;
        playerController.isRidingRocket = false;
        playerObject.transform.parent = null;
        playerController.RestoreGravity();
        playerController.ApplyJump(exitJumpForce);
    }
    #endregion

    #region Unorganized
    private void OnTriggerEnter2D(Collider2D collider)
    {
        originalSpeed = speed;

        //Slow to half of the current speed when inside ground
        if (collider.gameObject.CompareTag("Ground"))
            speed = speed / 2;
        
        if (collider.gameObject.CompareTag("InstaKill"))
            Destroy(gameObject);

        Knockback(collider);
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        //Slow to half of the current speed when inside ground
        if (collider.gameObject.CompareTag("Ground"))
            speed = originalSpeed;
    }


    void Knockback(Collider2D collider)
    {
        if (collider.CompareTag("Flyer"))
        {
            Rigidbody2D foundRigidBody = collider.GetComponent<Rigidbody2D>();

            //Disabling scripts that can interfer with the force
            collider.GetComponent<AIPath>().enabled = false;
            collider.GetComponent<AIDestinationSetter>().enabled = false;

            //Deciding the direction of the force
            if (transform.position.x < collider.GetComponent<Transform>().position.x)
                foundRigidBody.AddForce(enemyKnockForce, ForceMode2D.Impulse);
            else
                foundRigidBody.AddForce(new Vector2(enemyKnockForce.x * -1, enemyKnockForce.y), ForceMode2D.Impulse);

            foundRigidBody.drag = enemyKnockLinearDrag;
            foundRigidBody.gravityScale = enemyKnockGravityForce;

            collider.GetComponent<EnemyFlyerController>().gotKnocked = true;
        }

        /*
        if (collider.CompareTag("Player"))
        {
            Rigidbody2D playerRigidBody = collider.GetComponent<Rigidbody2D>();

            //If the rocketato knocks the player on the ground he wount move because of the full friction
            PlayerController.isKnocked = true;

            //Deciding the direction of the force
            if (transform.position.x < collider.GetComponent<Transform>().position.x)
                playerRigidBody.AddForce(playerKnockForce * 100f, ForceMode2D.Force);
            else
                playerRigidBody.AddForce(new Vector2(playerKnockForce.x * -1f * 100f, playerKnockForce.y), ForceMode2D.Force);
        }
        */
    }
    #endregion

    #region Miscellanous
    private void Log(string message)
    {
        if(logger != null && showLogs)
            logger.Log(message, this);
    }
    #endregion
}
