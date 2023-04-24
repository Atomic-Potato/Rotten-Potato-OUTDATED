using UnityEngine;

[RequireComponent(typeof(CameraBasicFollow), typeof(CameraDash))]
public class CameraController : MonoBehaviour{
    
    [Header("Camera Strategies")]
    [SerializeField] CameraBasicFollow cameraBasicFollow;
    [SerializeField] CameraDash cameraDash;

    CameraStrategy strategy;
    
    void Awake(){
        strategy = cameraBasicFollow;
    }

    void Start(){
        strategy.Execute();
    }

    void LateUpdate(){
        strategy = GetStrategy();
        strategy.Execute();        
    }

    CameraStrategy GetStrategy(){
        if(Dash.IS_DASHING || Dash.IS_HOLDING)
            return cameraDash;
        return cameraBasicFollow;
    }
}
