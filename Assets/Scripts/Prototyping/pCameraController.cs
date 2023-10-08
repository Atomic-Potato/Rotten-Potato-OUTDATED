using UnityEngine;

public class pCameraController : MonoBehaviour 
{
    [SerializeField] CameraScenePanning cameraScenePanning;


    ICameraStrategy strategy;

    void Update() 
    {
        strategy?.ExecuteUpdate();
    }

    void FixedUpdate() 
    {
        strategy?.ExecuteFixedUpdate();
    }

    ICameraStrategy GetStrategy()
    {
        if (Time.timeScale == 1f)
        {
            return cameraScenePanning;
        }

        return null;
    }
}