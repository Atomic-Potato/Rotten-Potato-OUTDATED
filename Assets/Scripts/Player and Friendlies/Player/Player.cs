using UnityEngine;

public class Player : MonoBehaviour{
    [Tooltip("Where is exactly the center of the player object. Usually we place in here the player sprite")]
    [SerializeField] Transform center;
    
    static GameObject instance;
    static Vector3 position;

    public static GameObject Instance{
        get{
            return instance;
        }
    }

    public static Vector3 Position{
        get{
            return position;
        }
    }

    void Awake(){
        instance = gameObject;
    }

    void Update(){
        position = center.transform.position;
    } 
}