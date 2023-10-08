using UnityEngine;

public class CameraController : MonoBehaviour{
    
    [Header("Camera Strategies")]
    [SerializeField] CameraScenePanning strategyCameraScenePanning;
    [SerializeField] ZoomEffect strategyZoomEffect;

    ICameraStrategy strategy;
    
    void Awake(){
        strategy = strategyCameraScenePanning;
    }

    void Update(){
        strategy = GetStrategy();
        strategy.ExecuteUpdate();        
    }

    void FixedUpdate() 
    {
        strategy.ExecuteFixedUpdate();
    }

    ICameraStrategy GetStrategy(){
        if (Time.timeScale < 1f || strategyZoomEffect.IsZooming)
        {
            return strategyZoomEffect;
        }


        return strategyCameraScenePanning;
    }
}
