using UnityEngine;

[System.Serializable]
public class CloudsManager : MonoBehaviour{
    [SerializeField] CloudChunckManager[] cloudChuncks;
    [SerializeField] CloudLayerManager[] cloudsLayers;
    
    #region CONSTANTS
    /// <summary>
    /// if the cloud direction is right, 
    /// this is where the cloud retrun to after reaching the right boundary point
    /// </summary>
    /// <value></value>
    public static Vector3 RIGHT_DIR_LEFT_BOUNDARY_POINT{
        get{
            return new Vector3(-40, 0f, 0f);
        }
    }
    /// <summary>
    /// if the cloud direction is right, 
    /// this is where the clouds stop and return to the left boundary point
    /// </summary>
    /// <value></value>
    public static Vector3 RIGHT_DIR_RIGHT_BOUNDARY_POINT{
        get{
            return new Vector3(20, 0f, 0f);
        }
    }

    /// <summary>
    /// if the cloud direction is left, 
    /// this is where the clouds stop and return to the right boundary point
    /// </summary>
    /// <value></value>
    public static Vector3 LEFT_DIR_LEFT_BOUNDARY_POINT{
        get{
            return new Vector3(-20, 0f, 0f);
        }
    }
    /// <summary>
    /// if the cloud direction is left, 
    /// this is where the cloud retrun to after reaching the left boundary point
    /// </summary>
    /// <value></value>
    public static Vector3 LEFT_DIR_RIGHT_BOUNDARY_POINT{
        get{
            return new Vector3(40, 0f, 0f);
        }
    }


    public static int LEFT{
        get{ return -1; }
    }
    public static int MIDDLE{
        get{ return 0; }
    }
    public static int STILL{
        get{ return 0; }
    }
    public static int RIGHT{
        get{ return 1; }
    }
    #endregion

    void FixedUpdate(){
        foreach(CloudLayerManager cloudLayer in cloudsLayers)
            cloudLayer.MoveLayer();
    }
}