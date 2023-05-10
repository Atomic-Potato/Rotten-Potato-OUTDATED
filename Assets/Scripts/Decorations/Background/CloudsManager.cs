using UnityEngine;

[System.Serializable]
public class CloudsManager : MonoBehaviour{
    [SerializeField] CloudChunckManager[] cloudChuncks;
    [SerializeField] CloudLayerManager[] cloudsLayers;
    
    #region CONSTANTS
    // where the cloud retrun to after reaching the endPoint
    public static Vector3 LEFT_BOUNDARY_POINT{
        get{
            return new Vector3(-40, 0f, 0f);
        }
    }
    // where the clouds stop and return to startPoint
    public static Vector3 RIGHT_BOUNDARY_POINT{
        get{
            return new Vector3(20, 0f, 0f);
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