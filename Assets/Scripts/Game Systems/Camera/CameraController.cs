using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float camSwitchingSpeed;
    [SerializeField] Camera cameraComponent;
    [SerializeField] GameObject[] mouseRefrences; //"top 0" bottom 1 "right 2" left 3 "middletop 4" middlebottom 5 "middleright 6" middleleft 7 
    [SerializeField] GameObject[] camPositions;//top 0 "bottom 1" right 2 "left 3" middle 4
    

    bool onY; //checks if the camera is peeking on the Y
    bool onX; //checks if the camera is peeking on the X

    Vector3 newPosition = Vector3.zero;
    Vector3 refVelocity = Vector3.zero;


    void FixedUpdate() 
    {
        Peek();
    }

    void Peek()
    {
        /// Basically we have empty objects where if the mouse passes over their transforms
        /// the camera moves torwards that empty object
        
        //Get the mouse position in the world 
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if(mousePosition.y >= mouseRefrences[0].transform.position.y)//if at top of the screen
        {
            //Debug.Log("top");
            onY = true;
            newPosition.y = camPositions[0].transform.position.y;
        }
        else if(mousePosition.y <= mouseRefrences[1].transform.position.y)// if at the bottom of the screen
        {
            //Debug.Log("bottom");
            onY = true;
            newPosition.y = camPositions[1].transform.position.y;
        }
        else // if in the deadzone between the middle and the top dots
        {
            //if the mouse already reached the top, then stay at the top, same for bottom
            if(mousePosition.y >= mouseRefrences[4].transform.position.y && onY)
                newPosition.y = camPositions[0].transform.position.y;
            else if(mousePosition.y <= mouseRefrences[4].transform.position.y && onY)
                newPosition.y = camPositions[1].transform.position.y;
            else//if not stay in the middle
            {
                onY = false;
                newPosition.y = camPositions[4].transform.position.y;
            }
                
        }
        
        //if in the middle on y
        if(mousePosition.y <= mouseRefrences[4].transform.position.y && mousePosition.y >= mouseRefrences[5].transform.position.y)
        {
            onY = false;
            newPosition.y = camPositions[4].transform.position.y;
        }
        
        if(mousePosition.x >= mouseRefrences[2].transform.position.x)//if to the right
        {
            //Debug.Log("right");
            onX = true;
            newPosition.x = camPositions[2].transform.position.x;
        }
        else if(mousePosition.x <= mouseRefrences[3].transform.position.x)// if to the left
        {
            //Debug.Log("left");
            onX = true;
            newPosition.x = camPositions[3].transform.position.x;
        }
        else // if in the deadzone between the middle and the top dots
        {
            //if the mouse already reached the right, then stay at the right, same for left
            if(mousePosition.x >= mouseRefrences[6].transform.position.x && onX)
                newPosition.x = camPositions[2].transform.position.x;
            else if(mousePosition.x <= mouseRefrences[7].transform.position.x && onX)
                newPosition.x = camPositions[3].transform.position.x;
            else//if not stay in the middle
            {
                onX = false;
                newPosition.x = camPositions[4].transform.position.x;
            }
                
        }

        //if in the middle on x
        if(mousePosition.x <= mouseRefrences[6].transform.position.y && mousePosition.y >= mouseRefrences[7].transform.position.y)
        {
            onX = false;
            newPosition.x = camPositions[4].transform.position.x;
        }


        SwitchCamPosition(newPosition, camSwitchingSpeed);
    }

    void SwitchCamPosition(Vector3 newPosition, float speed)
    {

        newPosition.z = transform.position.z; // i dont want to mess with the camera z and i dont wanna deal with it every time i add a new position
        //Debug.Log("Current:" + transform.position + "Heading" + newPosition);
        transform.position = Vector3.Lerp(transform.position, newPosition, speed);
    }

    public void CameraShake(float strength)
    {
        transform.localPosition += Random.insideUnitSphere * strength;
    }

    public void CameraZoom(float zoomAmount, float timeToZoom, float defaultZoom)
    {

        if(!(cameraComponent.orthographicSize < (defaultZoom + zoomAmount + 0.1f) && cameraComponent.orthographicSize > (defaultZoom + zoomAmount - 0.1f)))
        {
            cameraComponent.orthographicSize = Mathf.SmoothDamp(cameraComponent.orthographicSize, defaultZoom + zoomAmount, ref refVelocity.x,timeToZoom);
        }
    }
}
