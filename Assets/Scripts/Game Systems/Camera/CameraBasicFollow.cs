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
    [Header("DUAL FORWARD FOCUS")]
    [SerializeField] bool enableDualForwardFocus = true;
    [Tooltip("The offset where the window will be position to the left side of the screen.")]
    [SerializeField] Vector2 leftWindowOffset;
    [Tooltip("The offset where the window will be position to the right side of the screen.")]
    [SerializeField] Vector2 rightWindowOffset;
    [Tooltip("The time taken to switch to one of the offset windows side")]
    [Range(0f, 5f)]
    [SerializeField] float sideSwitchTime; 
    
    [Space]
    [Header("TOOLS")]
    [SerializeField] bool enableTools;
    [SerializeField] bool drawWindow;
    [SerializeField] bool drawDualForwardFocusWindows;
    [SerializeField] bool drawPlatformSnapLine;
    [SerializeField] bool drawAnchorSnapLine;

    [Space]
    [Header("REQUIRED COMPONENTS")]
    [SerializeField] LayerMask groundLayer;
    #endregion


    #region PRIVATE VARIABLES
    int lastHorizontalBorderTouched; // -1 left border, 1 right border
    int horizontalSideToSwitchTo;  // -1 left border, 1 right border
    float? groundPos = null;
    bool switchingSides;

    // ---- Camera ----
    float leftScreenEdge;
    float bottomScreenEdge;
    float cameraHeight;
    float cameraWidth;

    // ---- Constants ---
    const int LEFT = -1;
    const int RIGHT = 1;

    // ---- Cache ----
    Vector3 refPlatfromSnapVelocity;
    Vector3 refAnchorSnapVelocity;
    Vector2 refWindowSwitchVelocity;
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
        if(enableDualForwardFocus)
            MoveWindowToApproriateSide();
    }

    void OnDrawGizmos(){
        if(!enableTools)
            return;

        DrawWindow(drawWindow, windowOffset, Color.yellow);
        DrawWindow(drawDualForwardFocusWindows, leftWindowOffset, Color.red);
        DrawWindow(drawDualForwardFocusWindows, rightWindowOffset, Color.green);
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

        if(palyerPos.x > rightBorderPos){
            transform.position += new Vector3(palyerPos.x - rightBorderPos,0f,0f);
            lastHorizontalBorderTouched = RIGHT;
        }
        else if(palyerPos.x < leftBorderPos){
            transform.position -= new Vector3(leftBorderPos - palyerPos.x,0f,0f);
            lastHorizontalBorderTouched = LEFT;
        }

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

    #region DUAL FORWARD FOCUS
    void MoveWindowToApproriateSide(){
        if(!switchingSides){
            if(lastHorizontalBorderTouched == RIGHT)
                horizontalSideToSwitchTo = LEFT;
            else 
                horizontalSideToSwitchTo = RIGHT;
        }
        
        if(horizontalSideToSwitchTo == RIGHT)
            MoveWindow(rightWindowOffset);
        else
            MoveWindow(leftWindowOffset);
    }

    void MoveWindow(Vector2 offset){
        switchingSides = true;
        
        windowOffset = Vector2.SmoothDamp(windowOffset, offset, ref refWindowSwitchVelocity, sideSwitchTime);
        
        if(ApproximatelyEqualVectors(windowOffset, offset, 0.01f)){
            switchingSides = false;
            windowOffset = offset;
        }
    }

    bool ApproximatelyEqualVectors(Vector2 a, Vector2 b, float floatingPointDifference){
        if((b.x - floatingPointDifference < a.x && a.x < b.x + floatingPointDifference) &&
           (b.y - floatingPointDifference < a.y && a.y < b.y + floatingPointDifference))
            return true;
        return false;
    }
    #endregion

    #region TOOLS
    void DrawWindow(bool active, Vector2 offset, Color color){
        if(!active)
            return;
        
        // How much the width of the screen is bigger than the height of the screen
        float screenAspectRatio = 16f/9f;
        // float screenAspectRatio = (float)Screen.height / (float)Screen.width;
        float cameraHeight = Camera.main.orthographicSize * 2f;
        float cameraWidth = cameraHeight * screenAspectRatio;

        leftScreenEdge = Camera.main.transform.position.x - (cameraWidth / 2f);
        bottomScreenEdge = Camera.main.transform.position.y - (cameraHeight / 2f);
        
        // top
        Debug.DrawLine(new Vector2(leftScreenEdge + offset.x, bottomScreenEdge + offset.y + windowSize.y),
            new Vector2(leftScreenEdge + offset.x + windowSize.x, bottomScreenEdge + offset.y + windowSize.y), color);
        // bottom
        Debug.DrawLine(new Vector2(leftScreenEdge + offset.x, bottomScreenEdge + offset.y),
            new Vector2(leftScreenEdge + offset.x + windowSize.x, bottomScreenEdge + offset.y), color);

        // left
        Debug.DrawLine(new Vector2(leftScreenEdge + offset.x, bottomScreenEdge + offset.y),
            new Vector2(leftScreenEdge + offset.x, bottomScreenEdge + offset.y + windowSize.y), color);
        // right
        Debug.DrawLine(new Vector2(leftScreenEdge + offset.x + windowSize.x, bottomScreenEdge + offset.y),
            new Vector2(leftScreenEdge + offset.x + windowSize.x, bottomScreenEdge + offset.y + windowSize.y), color);
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