using UnityEngine;

public class pCameraPanningPoint : MonoBehaviour {
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
}