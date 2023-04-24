using UnityEngine;

public class CameraBasicFollow : MonoBehaviour, CameraStrategy{
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

    [Header("TOOLS")]
    [SerializeField] bool enableTools;
    [SerializeField] bool drawWindow;
    [SerializeField] bool drawPlatformSnapLine;
    [SerializeField] bool drawAnchorSnapLine;

    float? groundPos = null;
    Vector3 refPlatfromSnapVelocity;
    Vector3 refAnchorSnapVelocity;

    #region EXECUTION
    public void Execute(){
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

        float rightBorderPos = (transform.position.x + windowOffset.x) + windowSize.x;
        float leftBorderPos = (transform.position.x + windowOffset.x) - windowSize.x;
        float topBorderPos = (transform.position.y + windowOffset.y) + windowSize.y;
        float bottomBorderPos = (transform.position.y + windowOffset.y) - windowSize.y;

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

        Debug.Log("grounded status : " + BasicMovement.IS_GROUNDED);

        // ToDo: Fix smooth damp not moving to exactly the desired location
        transform.position = Vector3.SmoothDamp(transform.position, 
            new Vector3(transform.position.x, (float)groundPos - (platformSnapOffset + windowOffset.y), transform.position.z), 
            ref refPlatfromSnapVelocity, timeToSnapToPlatform);
    }

    void GetGroundPosition(){
        // ToDo: use the collider bounds to get the platform position
        // Bounds colliderBounds = Player.Instance.GetComponent<BoxCollider2D>().bounds;
        
        if(BasicMovement.IS_GROUNDED)
            if(groundPos == null)
                groundPos = Player.Instance.transform.position.y - 0.5f;
        
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
            new Vector3(transform.position.x, ((Vector2)Grapple.ANCHOR_POSITION).y - (anchorSnapOffset + windowOffset.y), transform.position.z), 
            ref refAnchorSnapVelocity, timeToSnapToPlatform);
    }
    #endregion

    #region TOOLS
    void DrawWindow(){
        if(!drawWindow)
            return;
        
        // top
        Debug.DrawLine(new Vector2(transform.position.x + windowOffset.x - windowSize.x, transform.position.y + windowOffset.y + windowSize.y),
            new Vector2(transform.position.x + windowOffset.x + windowSize.x, transform.position.y + windowOffset.y + windowSize.y), Color.yellow);
        // bottom
        Debug.DrawLine(new Vector2(transform.position.x + windowOffset.x - windowSize.x, transform.position.y + windowOffset.y - windowSize.y),
            new Vector2(transform.position.x + windowOffset.x + windowSize.x, transform.position.y + windowOffset.y - windowSize.y), Color.yellow);

        // left
        Debug.DrawLine(new Vector2(transform.position.x + windowOffset.x - windowSize.x, transform.position.y + windowOffset.y - windowSize.y),
            new Vector2(transform.position.x + windowOffset.x - windowSize.x, transform.position.y + windowOffset.y + windowSize.y), Color.yellow);
        // right
        Debug.DrawLine(new Vector2(transform.position.x + windowOffset.x + windowSize.x, transform.position.y + windowOffset.y - windowSize.y),
            new Vector2(transform.position.x + windowOffset.x + windowSize.x, transform.position.y + windowOffset.y + windowSize.y), Color.yellow);
    }

    void DrawSnapLine(bool active, float offset, Color color){
        if(!active)
            return;
        Debug.DrawLine(new Vector2(transform.position.x + windowOffset.x - windowSize.x - 1f, transform.position.y + offset + windowOffset.y), 
                       new Vector2(transform.position.x + windowOffset.x + windowSize.x + 1f, transform.position.y + offset + windowOffset.y), 
                       color);
    }
    #endregion
}