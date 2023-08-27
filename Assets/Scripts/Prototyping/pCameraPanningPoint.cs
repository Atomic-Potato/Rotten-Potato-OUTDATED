using UnityEngine;

public class pCameraPanningPoint : MonoBehaviour {

    [Tooltip("Marks this point as edge of the scene so the camera wont go past it")]
    [SerializeField] bool isSceneEdge = true;

    [Space]
    [SerializeField] pCameraPanningPoint nextPoint;
    [Space]
    [SerializeField] pCameraPanningPoint previousPoint;
    
    
    public float x{ get{ return transform.position.x; } }
    public float y{ get{ return transform.position.y; } }

    public pCameraPanningPoint getNextPoint{
        get{
            return nextPoint;
        }
    }

    public pCameraPanningPoint getPreviousPoint{
        get{
            return previousPoint;
        }
    }

    public bool IsSceneEdgePoint{ get { return isSceneEdge; } }
}