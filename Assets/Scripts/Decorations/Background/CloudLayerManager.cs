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
    }
    #endregion

    #region MOVING THE LAYER
    /// <summary>
    /// Moves all the clouds in the layer accorinding 
    /// to the layer direction and speed
    /// </summary>
    public void MoveLayer(){
        if(travelDirection == CloudsManager.STILL)
            return;

        Cloud frontCloud = orderedClouds[2];
        MoveCloud(frontCloud);
        
        ConnectClouds(orderedClouds[1], orderedClouds[2]);
        ConnectClouds(orderedClouds[0], orderedClouds[1]);

        if(CloudOutOfBounds(frontCloud)){
            ShiftCloudsOrder();
            ConnectClouds(frontCloud, orderedClouds[1]); // the frontCloud is at index 0 in the array after shifting
        }
    }
    
    /// <summary>
    /// Moves the cloud in the direction of the layer and its speed 
    /// </summary>
    void MoveCloud(Cloud cloud){
        Debug.Log("Moving " + gameObject.name);
        cloud.transform.position = new Vector3(
            cloud.transform.position.x + Time.deltaTime * speed * travelDirection, 
            cloud.transform.position.y,
            cloud.transform.position.y
        );
    }

    /// <summary>
    /// Places "left" to the left of "right" with no gaps
    /// </summary>
    void ConnectClouds(Cloud left, Cloud right){
        left.transform.position = new Vector3(
            right.transform.position.x - right.Width,
            right.transform.position.y,
            right.transform.position.z
        );
    }

    /// <summary>
    /// Checks if the given cloud is still inside the boundaries
    /// specified in the CloudsManager
    /// </summary>
    bool CloudOutOfBounds(Cloud cloud){
        return cloud.transform.position.x > CloudsManager.RIGHT_BOUNDARY_POINT.x
            || cloud.transform.position.x < CloudsManager.LEFT_BOUNDARY_POINT.x;
    
    }

    /// <summary>
    ///  Shifts the clouds order by 1 for each cloud object
    /// </summary>
    void ShiftCloudsOrder(){
        foreach(Cloud cloud in cloudsInLayer){
            if(cloud.Order_Position == CloudsManager.RIGHT){
                cloud.Order_Position = CloudsManager.LEFT;
                continue;
            }
            cloud.Order_Position += 1;
        }
        UpdateCloudsOrderInArray();
    }


    /// <summary>
    /// Rearranges the ordered array of the clouds
    /// </summary>
    void UpdateCloudsOrderInArray(){
        foreach(Cloud cloud in cloudsInLayer){
            if(cloud.Order_Position == CloudsManager.RIGHT)
                orderedClouds[2] = cloud;
            else if(cloud.Order_Position == CloudsManager.MIDDLE)
                orderedClouds[1] = cloud;
            else if(cloud.Order_Position == CloudsManager.LEFT)
                orderedClouds[0] = cloud;
            else
                throw  exCloudPositionNotFound;
        }
    }
    #endregion
}