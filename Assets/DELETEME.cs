using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DELETEME : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;


    // Update is called once per frame
    void Update()
    {
    }

    public void Move(InputAction.CallbackContext context)
    {
        Debug.Log("Move: " + context.phase  + " " + context.ReadValue<float>());
    }

    public void Jump(InputAction.CallbackContext context)
    {
        Debug.Log("Jump: " + context.phase);
    }

    public void Dash(InputAction.CallbackContext context)
    {
        Debug.Log("Dash: " + context.phase  + " " + context.ReadValue<float>());
    }

    public void Grapple(InputAction.CallbackContext context)
    {
        Debug.Log("Grapple: " + context.phase  + " " + context.ReadValue<float>());
    }
}
