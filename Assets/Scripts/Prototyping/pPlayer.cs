using UnityEngine;

public class pPlayer : MonoBehaviour
{
    static GameObject _instance; 
    public static GameObject Instance { get { return _instance; } } 

    void Awake() 
    {
        if (_instance != null)
        {
            Destroy(_instance);
        }
        else
        {
            _instance = gameObject;    
        }
    }
}
