using UnityEngine;

public class pCameraController : MonoBehaviour {
    [SerializeField] Transform toFollow;
    [SerializeField] pCameraPanningPoint firstPanningPoint;

    pCameraPanningPoint nextPanningPoint;
    pCameraPanningPoint previousPanningPoint;

    public static int CurrentPanningType;
    public static int VerticalPanning{ get{ return 1; } }
    public static int HorizontalPanning{ get { return 2; } }

    public static Vector2 FlowDirection;
    

    private void Awake() {
        previousPanningPoint = firstPanningPoint;
        nextPanningPoint = firstPanningPoint.getNextPoint;

        transform.position = new Vector3 (previousPanningPoint.x, previousPanningPoint.y, transform.position.z);

        CurrentPanningType = GetPanningType();
        FlowDirection = GetFlowDirection(CurrentPanningType);
    }

    void FixedUpdate() {
        if(CurrentPanningType == VerticalPanning){
            FollowVertically();
            LockCameraBetweenVerticalPoints();
        }
        else if(CurrentPanningType == HorizontalPanning){
            FollowHorizontally();
            LockCameraBetweenHorizontalPoints();
        }
        else{
            transform.position = toFollow.position; // Fail safe so in case it would happen the type will be fixed at the next point
            throw new System.Exception("Unknown camera panning direction value: " + CurrentPanningType);
        }
    }

    void Update() {
        if(CurrentPanningType == VerticalPanning){
            LockCameraBetweenVerticalPoints();
        }
        else if(CurrentPanningType == HorizontalPanning){
            LockCameraBetweenHorizontalPoints();
        }
    }

    private void FollowVertically(){
        transform.position = new Vector3(transform.position.x, toFollow.transform.position.y, transform.position.z);
    }

    private void FollowHorizontally(){
        transform.position = new Vector3(toFollow.transform.position.x, transform.position.y, transform.position.z);
    }

    void LockCameraBetweenVerticalPoints(){
        if(FlowDirection == Vector2.up){
            if(nextPanningPoint.IsSceneEdgePoint &&  transform.position.y >= nextPanningPoint.y)
                transform.position = new Vector3 (nextPanningPoint.x, nextPanningPoint.y, transform.position.z);
            if(previousPanningPoint.IsSceneEdgePoint &&  transform.position.y <= previousPanningPoint.y)
                transform.position = new Vector3 (previousPanningPoint.x, previousPanningPoint.y, transform.position.z);
        }
        else{
            if(nextPanningPoint.IsSceneEdgePoint &&  transform.position.y <= nextPanningPoint.y)
                transform.position = new Vector3 (nextPanningPoint.x, nextPanningPoint.y, transform.position.z);
            if(previousPanningPoint.IsSceneEdgePoint &&  transform.position.y >= previousPanningPoint.y)
                transform.position = new Vector3 (previousPanningPoint.x, previousPanningPoint.y, transform.position.z);
        }
    }

    void LockCameraBetweenHorizontalPoints(){
        if(FlowDirection == Vector2.right){
            if(nextPanningPoint.IsSceneEdgePoint &&  transform.position.x >= nextPanningPoint.x)
                transform.position = new Vector3 (nextPanningPoint.x, nextPanningPoint.y, transform.position.z);
            if(previousPanningPoint.IsSceneEdgePoint &&  transform.position.x <= previousPanningPoint.x)
                transform.position = new Vector3 (previousPanningPoint.x, previousPanningPoint.y, transform.position.z);
        }
        else{
            if(nextPanningPoint.IsSceneEdgePoint &&  transform.position.x <= nextPanningPoint.x)
                transform.position = new Vector3 (nextPanningPoint.x, nextPanningPoint.y, transform.position.z);
            if(previousPanningPoint.IsSceneEdgePoint &&  transform.position.x >= previousPanningPoint.x)
                transform.position = new Vector3 (previousPanningPoint.x, previousPanningPoint.y, transform.position.z);
        }
    }

    /// <summary>
    /// Checks the difference in height and width between the panning points
    /// and returns the camera pannind direction based on which is greater
    /// </summary>
    /// <returns>Vertical if height is greater, otherwise Horizontal</returns>
    int GetPanningType(){
        float differenceInHeight = Mathf.Abs(previousPanningPoint.y - nextPanningPoint.y); 
        float differenceInWidth = Mathf.Abs(previousPanningPoint.x - nextPanningPoint.x);

        if(differenceInHeight > differenceInWidth)
            return VerticalPanning;
        else
            return HorizontalPanning; 
    }

    Vector2 GetFlowDirection(int panningType){
        if(panningType == HorizontalPanning){
            if(previousPanningPoint.x < nextPanningPoint.x)
                return Vector2.right;
            else
                return Vector2.left;
        }
        else{
             if(previousPanningPoint.y < nextPanningPoint.y)
                return Vector2.up;
            else
                return Vector2.down;   
        }
    }
}