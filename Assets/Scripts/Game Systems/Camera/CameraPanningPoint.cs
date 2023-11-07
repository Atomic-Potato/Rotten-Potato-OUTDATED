using UnityEngine;

public class CameraPanningPoint : MonoBehaviour {
    [Space]
    [SerializeField] CameraPanningPoint nextPoint;
    [Space]
    [SerializeField] CameraPanningPoint previousPoint;
    
    
    public float x{ get{ return transform.position.x; } }
    public float y{ get{ return transform.position.y; } }

    public CameraPanningPoint getNextPoint{
        get{
            return nextPoint;
        }
    }

    public CameraPanningPoint getPreviousPoint{
        get{
            return previousPoint;
        }
    }
}