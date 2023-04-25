using UnityEngine;

public class CameraBasicFollow : MonoBehaviour, CameraStrategy{
    #region INSPECTOR VARIABLES
    [Header("WINDOW")]
    [SerializeField] bool enableWindow = true;
    [Tooltip("The size of the window that the camera will not allow the player to go out of its bounds."+
             "(Enable Draw Window in the tools section)")]
    [SerializeField] Vector2 windowSize;
    [Tooltip("The position offset of the window that the camera will not allow the player to go out of its bounds."+
             "(Enable Draw Window in the tools section)")]
    [SerializeField] Vector2 windowOffset;

    [Space]
    [Header("PLATFORM SNAP")]
    [SerializeField] bool enablePlatfromSnapping = true;
    [Tooltip("The vertical position offset that the camera will snap to when player is grounded. (Enable Draw Snap Line for extra info)")]
    [SerializeField] float platformSnapOffset;
    [Tooltip("The time it takes for the camera to move vertically to the snap line. (Enable Draw Snap Line for extra info)")]
    [Range(0f, 5f)]
    [SerializeField] float timeToSnapToPlatform = 0.75f;

    [Space]
    [Header("ANCHOR SNAP")]
    [SerializeField] bool enableAnchorSnapping = true;
    [Tooltip("The vertical position offset that the camera will snap to when player is grounded. (Enable Draw Snap Line for extra info)")]
    [SerializeField] float anchorSnapOffset;
    [Tooltip("The time it takes for the camera to move vertically to the snap line. (Enable Draw Snap Line for extra info)")]
    [Range(0f, 5f)]
    [SerializeField] float timeToSnapToAnchor = 0.75f;

    [Space]
    [Header("TOOLS")]
    [SerializeField] bool enableTools;
    [SerializeField] bool drawWindow;
    [SerializeField] bool drawPlatformSnapLine;
    [SerializeField] bool drawAnchorSnapLine;

    [Space]
    [Header("REQUIRED COMPONENTS")]
    [SerializeField] LayerMask groundLayer;
    #endregion

    #region PRIVATE VARIABLES
    float? groundPos = null;
    Vector3 refPlatfromSnapVelocity;
    Vector3 refAnchorSnapVelocity;

    // ---- Camera ----
    float leftScreenEdge;
    float bottomScreenEdge;
    float cameraHeight;
    float cameraWidth;
    #endregion

    #region EXECUTION
    void Start(){
        // How much the width of the screen is bigger than the height of the screen
        float screenAspectRatio = 16f/9f;
        // float screenAspectRatio = (float)Screen.height / (float)Screen.width;
        cameraHeight = Camera.main.orthographicSize * 2f;
        cameraWidth = cameraHeight * screenAspectRatio;
    }

    public void Execute(){
        leftScreenEdge = Camera.main.transform.position.x - (cameraWidth / 2f);
        bottomScreenEdge = Camera.main.transform.position.y - (cameraHeight / 2f);

        if(enableWindow)
            ConstraintToWindow();
        if(enablePlatfromSnapping)
            SnapToPlatform();
        if(enableAnchorSnapping)
            SnapToAnchor();
    }

    void OnDrawGizmos(){
        if(!enableTools)
            return;

        DrawWindow();
        DrawSnapLine(drawPlatformSnapLine, platformSnapOffset, Color.green);
        DrawSnapLine(drawAnchorSnapLine, anchorSnapOffset, Color.red);
    }
    #endregion

    #region WINDOW
    void ConstraintToWindow(){
        Vector2 palyerPos = Player.Position;

        float rightBorderPos = (leftScreenEdge + windowOffset.x) + windowSize.x;
        float leftBorderPos = (leftScreenEdge + windowOffset.x);
        float topBorderPos = (bottomScreenEdge + windowOffset.y) + windowSize.y;
        float bottomBorderPos = (bottomScreenEdge + windowOffset.y);

        if(palyerPos.x > rightBorderPos)
            transform.position += new Vector3(palyerPos.x - rightBorderPos,0f,0f);
        else if(palyerPos.x < leftBorderPos)
            transform.position -= new Vector3(leftBorderPos - palyerPos.x,0f,0f);

        if(palyerPos.y > topBorderPos)
            transform.position += new Vector3(0f, palyerPos.y - topBorderPos,0f);
        else if(palyerPos.y < bottomBorderPos)
            transform.position -= new Vector3(0f, bottomBorderPos - palyerPos.y,0f);
    }
    bool GotOutOfVerticalBounds(){
        Vector2 palyerPos = Player.Position;
        float topBorderPos = (transform.position.y + windowOffset.y) + windowSize.y;
        float bottomBorderPos = (transform.position.y + windowOffset.y) - windowSize.y;

        if(palyerPos.y == topBorderPos || palyerPos.y == bottomBorderPos)
            return true;
        return false;
    }
    #endregion

    #region PLATFORM SNAPPING
    void SnapToPlatform(){
        GetGroundPosition();
        if(groundPos == null)
            return;

        // ToDo: Fix smooth damp not moving to exactly the desired location
        transform.position = Vector3.SmoothDamp(transform.position, 
            new Vector3(transform.position.x, (float)groundPos + ((cameraHeight/2f) - (windowOffset.y + platformSnapOffset)), transform.position.z), 
            ref refPlatfromSnapVelocity, timeToSnapToPlatform);
    }

    void GetGroundPosition(){
        // ToDo: use the collider bounds to get the platform position
        // Bounds colliderBounds = Player.Instance.GetComponent<BoxCollider2D>().bounds;
        
        if(BasicMovement.IS_GROUNDED){
            if(groundPos == null){
                RaycastHit2D ray = Physics2D.Raycast(Player.Instance.transform.position, Vector2.down, 10f, groundLayer);
                groundPos = ray.point.y;
            }
        }
        
        if(GotOutOfVerticalBounds() || !BasicMovement.IS_GROUNDED){
            groundPos = null;
        }
    }
    #endregion

    #region ANCHOR SNAPPING
    void SnapToAnchor(){
        if(!Grapple.IS_ON_ANCHOR)
            return;

        transform.position = Vector3.SmoothDamp(transform.position, 
            new Vector3(transform.position.x, ((Vector2)Grapple.ANCHOR_POSITION).y + ((cameraHeight/2f) - (anchorSnapOffset + windowOffset.y)), transform.position.z), 
            ref refAnchorSnapVelocity, timeToSnapToPlatform);
    }
    #endregion

    #region TOOLS
    void DrawWindow(){
        if(!drawWindow)
            return;
        
        // How much the width of the screen is bigger than the height of the screen
        float screenAspectRatio = 16f/9f;
        // float screenAspectRatio = (float)Screen.height / (float)Screen.width;
        float cameraHeight = Camera.main.orthographicSize * 2f;
        float cameraWidth = cameraHeight * screenAspectRatio;

        leftScreenEdge = Camera.main.transform.position.x - (cameraWidth / 2f);
        bottomScreenEdge = Camera.main.transform.position.y - (cameraHeight / 2f);
        
        // top
        Debug.DrawLine(new Vector2(leftScreenEdge + windowOffset.x, bottomScreenEdge + windowOffset.y + windowSize.y),
            new Vector2(leftScreenEdge + windowOffset.x + windowSize.x, bottomScreenEdge + windowOffset.y + windowSize.y), Color.yellow);
        // bottom
        Debug.DrawLine(new Vector2(leftScreenEdge + windowOffset.x, bottomScreenEdge + windowOffset.y),
            new Vector2(leftScreenEdge + windowOffset.x + windowSize.x, bottomScreenEdge + windowOffset.y), Color.yellow);

        // left
        Debug.DrawLine(new Vector2(leftScreenEdge + windowOffset.x, bottomScreenEdge + windowOffset.y),
            new Vector2(leftScreenEdge + windowOffset.x, bottomScreenEdge + windowOffset.y + windowSize.y), Color.yellow);
        // right
        Debug.DrawLine(new Vector2(leftScreenEdge + windowOffset.x + windowSize.x, bottomScreenEdge + windowOffset.y),
            new Vector2(leftScreenEdge + windowOffset.x + windowSize.x, bottomScreenEdge + windowOffset.y + windowSize.y), Color.yellow);
    }

    void DrawSnapLine(bool active, float offset, Color color){
        if(!active)
            return;
        Debug.DrawLine(new Vector2(leftScreenEdge + windowOffset.x - .5f, bottomScreenEdge + windowOffset.y + offset), 
                       new Vector2(leftScreenEdge + windowOffset.x + windowSize.x + .5f, bottomScreenEdge + windowOffset.y + offset), 
                       color);
    }
    #endregion
}