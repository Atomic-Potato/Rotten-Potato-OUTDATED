using UnityEngine;

public class pCameraSceneTransistorPart : MonoBehaviour {
    [SerializeField] int partPosition;
    [SerializeField] pCameraSceneTransistorManager manager;
    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player"){
            if(partPosition == 1)
                manager.SwitchToLeftScene();
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if(other.tag == "Player"){
            if(partPosition == -1)
                manager.SwitchToRightScene();
        }
    }

}