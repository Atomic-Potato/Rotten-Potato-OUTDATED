using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour{
    private static PlayerInput playerInputActions;

    public static PlayerInput Maps{ get{return playerInputActions;} }

    void Awake() {
        playerInputActions = new PlayerInput();    
    }

    void OnEnable() {
        playerInputActions.Enable();    
    }

    void OnDisable() {
        playerInputActions.Disable();    
    }
}