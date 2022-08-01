using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Note:     Here i recommend once the art for the game is done is to make the animation of the string to play once
 *          an idea for the animation is to make a long stick with leavs impulsed from the stick 
 */

public class GrapplingStringController : MonoBehaviour
{
    public LineRenderer lineRenderer;

    //Animation
    [Tooltip("These are just normal sprites")]
    [SerializeField] Texture[] grapplingStringFrames;
    [SerializeField] float animationFPS = 30f; //Animation FPS

    float fpsCounter = 0f; //Keeps track of the time of each frame
    int currentFrame = 0;

    // Update is called once per frame
    void Update()
    {
        fpsCounter += Time.deltaTime;
        if(fpsCounter >= 1f / animationFPS)
        {
            if (currentFrame == grapplingStringFrames.Length)
                currentFrame = 0;

            lineRenderer.material.SetTexture("_MainTex", grapplingStringFrames[currentFrame]);

            fpsCounter = 0f;
            currentFrame++;
        }


        //Debugging
        //Debug.Log("Array Length = " + grapplingStringFrames.Length);
    }

    public void GrapplingStringTarget(Transform origin, Transform target)
    {
        lineRenderer.SetPosition(0, origin.position);
        lineRenderer.SetPosition(1, target.position);
    }
}
