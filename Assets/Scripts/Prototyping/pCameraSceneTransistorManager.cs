using UnityEngine;

public class pCameraSceneTransistorManager : MonoBehaviour {
    [Space]
    [SerializeField] pCameraSceneTransistorPart rightPart;
    [SerializeField] pCameraPanningPoint rightSceneEdgePoint;

    [Space]
    [SerializeField] pCameraSceneTransistorPart leftPart;
    [SerializeField] pCameraPanningPoint leftSceneEdgePoint;

    [Space]
    [SerializeField] Transform player;
    [SerializeField] pCameraController cameraController;

    public void SwitchToLeftScene(){
        rightPart.gameObject.SetActive(false);
        leftPart.gameObject.SetActive(true);

        player.transform.position = leftSceneEdgePoint.transform.position;
        cameraController.firstPanningPoint = leftSceneEdgePoint;
        cameraController.Initialize();
    }

    public void SwitchToRightScene(){
        leftPart.gameObject.SetActive(false);
        rightPart.gameObject.SetActive(true);

        player.transform.position = rightSceneEdgePoint.transform.position;
        cameraController.firstPanningPoint = rightSceneEdgePoint;
        cameraController.Initialize();
    }
}