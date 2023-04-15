using UnityEngine;

/*Note:     Here i recommend once the art for the game is done is to make the animation of the string to play once
 *          an idea for the animation is to make a long stick with leavs impulsed from the stick 
 */

public class GrappleRenderer : MonoBehaviour
{
    #region INSPECTOR VARIABLES
    [Tooltip("An offset for the start point of the rendered line (The start point is the game object this script is attached to)")]
    [SerializeField] Vector2 startPointOffset;
    [Tooltip("An offset for the end point of the rendered line")]
    [SerializeField] Vector2 endPointOffset;
    [SerializeField] LineRenderer lineRenderer;

    [Header("Animation")]
    [SerializeField] bool animated;
    [SerializeField] float animationFPS = 30f;
    [SerializeField] Texture[] frames;

    [Space]
    [Tooltip("Only set if animated is not checked")]
    [SerializeField] Texture nonAnimatedSprite;
    #endregion

    #region PRIVATE VARIABLES
    float fpsCounter = 0f; //Keeps track of the time of each frame
    int currentFrame = 0;
    #endregion

    #region EXECUTION
    void Awake() {
        HideLineRenderer();
    }

    void Update(){
        if(!Grapple.IS_GRAPPLING){
            if(!lineRenderer.enabled){
                HideLineRenderer();
                return;
            }
            lineRenderer.enabled = false;
            return;
        }
        
        if(!lineRenderer.enabled){
            lineRenderer.enabled = true;
        }

        SetRendererEndPoints();

        if(!animated){
            RenderLine();
            return;
        }

        RenderAnimation();
    }
    #endregion

    #region RENDERING
    void RenderLine(){
        lineRenderer.material.SetTexture("_MainTex", nonAnimatedSprite);
    }

    void RenderAnimation(){
        fpsCounter += Time.deltaTime;
        if(fpsCounter < 1f / animationFPS)
            return;

        if (currentFrame == frames.Length)
            currentFrame = 0;

        lineRenderer.material.SetTexture("_MainTex", frames[currentFrame]);

        fpsCounter = 0f;
        currentFrame++;
    }
    #endregion

    void SetRendererEndPoints(){
        lineRenderer.SetPosition(0, (Vector2)transform.position + startPointOffset);
        lineRenderer.SetPosition(1, Grapple.ANCHOR_POSITION + endPointOffset);
    }

    void HideLineRenderer(){
        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, Vector3.zero);
    }
}
