using UnityEngine;

public class CameraCollider : MonoBehaviour{
    [SerializeField] Camera camera;
    [SerializeField] BoxCollider2D bordersCollider;

    private void Start() {
        UpdateBorders();
    }

    public void UpdateBorders(){
        float height = camera.orthographicSize * 2; // height of camera = 1  
                                                    // is always twice the height of a collider of size 1
        float ratio = (float)Screen.width/(float)Screen.height; // how much the width size is compared to the height
        float width = height * ratio; // the height never changes when the aspect ration is changed, only the width
        bordersCollider.size = new Vector2(width, height);
    }   
}