using UnityEngine;

/// <summary>
/// Responsible for moving all the clouds in the layer
/// </summary>
[System.Serializable]
public class CloudLayerManager : MonoBehaviour{
    #region INSPECTOR VARIABLES
    [Tooltip("Currently just helps in debugging")]
    [SerializeField] int layerNumber;
    [Tooltip("In which direction the cloud travels (-1: left | 0: no movement | 1: right)")]
    [Range(-1,1)]
    [SerializeField] int travelDirection;
    [Tooltip("The speed of the cloud in horizontal movement")]
    [Range(0f,100f)]
    [SerializeField] float speed;

    [Space]
    [SerializeField] Cloud[] cloudsInLayer;
    #endregion

    #region PRIVATE VARIABLES
    Cloud[] orderedClouds;
    System.Exception exCloudPositionNotFound;
    #endregion

    #region EXECUTION
    void Awake(){
        orderedClouds = new Cloud[cloudsInLayer.Length];
        exCloudPositionNotFound = new System.Exception("Cloud order position in layer " + layerNumber + "has no way to be handled");
    }

    void Start(){
        UpdateCloudsOrderInArray();
        SetLayerDirection();
    }
    #endregion

    #region MOVING THE LAYER
    /// <summary>
    /// Moves all the clouds in the layer accorinding 
    /// to the layer travel direction and speed
    /// </summary>
    public void MoveLayer(){
        if(travelDirection == CloudsManager.STILL)
            return;

        Cloud frontCloud = GetFrontCloud();
        MoveCloud(frontCloud);

        
        if(travelDirection == CloudsManager.RIGHT)
            ConnectCloudsFromLeftToRight(frontCloud);
        else
            ConnectCloudsFromRightToLeft(frontCloud);

        if(CloudOutOfBounds(frontCloud)){
            ShiftCloudsOrder();
            ConnectClouds(frontCloud, orderedClouds[1]); // the frontCloud is at index 0 in the array after shifting
        }
    }
    
    /// <summary>
    /// Gets the front cloud depending on the layer travel direction
    /// </summary>
    /// <returns></returns>
    Cloud GetFrontCloud(){
        if(travelDirection == CloudsManager.RIGHT)
            return orderedClouds[orderedClouds.Length-1];
        return orderedClouds[0];
    }

    /// <summary>
    /// Moves the cloud in the direction of the layer and its speed 
    /// </summary>
    void MoveCloud(Cloud cloud){
        cloud.transform.position = new Vector3(
            cloud.transform.position.x + Time.deltaTime * speed * travelDirection, 
            cloud.transform.position.y,
            cloud.transform.position.y
        );
    }

    /// <summary>
    /// Checks if the given cloud is still inside the boundaries
    /// specified in the CloudsManager
    /// </summary>
    bool CloudOutOfBounds(Cloud cloud){
        if(travelDirection == CloudsManager.RIGHT)
            return cloud.transform.position.x > CloudsManager.RIGHT_DIR_RIGHT_BOUNDARY_POINT.x
                || cloud.transform.position.x < CloudsManager.RIGHT_DIR_LEFT_BOUNDARY_POINT.x;
        else
            return cloud.transform.position.x > CloudsManager.LEFT_DIR_RIGHT_BOUNDARY_POINT.x
                || cloud.transform.position.x < CloudsManager.LEFT_DIR_LEFT_BOUNDARY_POINT.x;
    }
    #endregion

    #region SHIFTING CLOUDS
    /// <summary>
    ///  Shifts the clouds order by 1 for each cloud object
    /// </summary>
    void ShiftCloudsOrder(){
        foreach(Cloud cloud in cloudsInLayer){
            if(cloud.OrderPosition == travelDirection){
                cloud.OrderPosition = travelDirection * -1;
                continue;
            }
            cloud.OrderPosition += travelDirection;
        }
        UpdateCloudsOrderInArray();
    }


    /// <summary>
    /// Rearranges the ordered array of the clouds
    /// </summary>
    void UpdateCloudsOrderInArray(){
        foreach(Cloud cloud in cloudsInLayer){
            if(cloud.OrderPosition == CloudsManager.RIGHT)
                orderedClouds[2] = cloud;
            else if(cloud.OrderPosition == CloudsManager.MIDDLE)
                orderedClouds[1] = cloud;
            else if(cloud.OrderPosition == CloudsManager.LEFT)
                orderedClouds[0] = cloud;
            else
                throw  exCloudPositionNotFound;
        }
    }
    #endregion

    #region CONNECTING CLOUDS
    /// <summary>
    /// Places the a cloud and the b cloud next to eachother with no gaps
    /// depending on the travel direction. 
    /// If the direction is "right" : set a to left of b. 
    /// If the direction is "left" : set b to right of a.
    /// (Note: the important difference is which of the clouds position is one being set)
    /// </summary>
    void ConnectClouds(Cloud a, Cloud b){
        if(travelDirection == CloudsManager.RIGHT){
            a.transform.position = new Vector3(
                b.transform.position.x - b.Width,
                b.transform.position.y,
                b.transform.position.z
            );
        }
        else{
            b.transform.position = new Vector3(
                a.transform.position.x + a.Width,
                a.transform.position.y,
                a.transform.position.z
            );
        }
    }

    void ConnectCloudsFromLeftToRight(Cloud frontCloud){
        ConnectClouds(orderedClouds[0], orderedClouds[1]);
        ConnectClouds(orderedClouds[1], frontCloud);
    }

    void ConnectCloudsFromRightToLeft(Cloud frontCloud){
        ConnectClouds(orderedClouds[1], orderedClouds[2]);
        ConnectClouds(frontCloud, orderedClouds[1]);
    }
    #endregion

    #region SETTING UP
    /// <summary>
    /// Sets the starting position of each cloud depening on the travel direction
    /// </summary>
    void SetLayerDirection(){
        if(travelDirection == CloudsManager.RIGHT)
            PositionLayerToTheLeft();
        else if(travelDirection == CloudsManager.LEFT)
            PositionLayerToTheRight();
        else{
            foreach(Cloud cloud in orderedClouds)
                SetCloudHorizontalPos(cloud, 0f);
        }
    }

    /// <summary>
    /// Sets the cloud position on the x axis while keeping its y and z coordinates
    /// </summary>
    /// <param name="cloud"></param>
    /// <param name="pos"></param>
    void SetCloudHorizontalPos(Cloud cloud, float pos){
        cloud.transform.position = new Vector3(pos, cloud.transform.position.y, cloud.transform.position.z);
    }

    /// <summary>
    /// Sets all the clouds horizontal positions to the left of the camera in sequence
    /// (Thats is next to each other)
    /// </summary>
    void PositionLayerToTheLeft(){
        float horizontalPos = 0;
        for(int i = orderedClouds.Length-1; i >= 0; i--){
            SetCloudHorizontalPos(orderedClouds[i], horizontalPos);
            horizontalPos -= 20f;
        }
    }

    /// <summary>
    /// Sets all the clouds horizontal positions to the right of the camera in sequence
    /// (Thats is next to each other)
    /// </summary>
    void PositionLayerToTheRight(){
        float horizontalPos = 0;
        for(int i = 0; i < orderedClouds.Length; i++){
            SetCloudHorizontalPos(orderedClouds[i], horizontalPos);
            horizontalPos += 20f;
        }
    }
    #endregion
}