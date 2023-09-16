using UnityEngine;

public class PlayerInputManager : MonoBehaviour{
    private static PlayerInput playerInputActions;

    public static PlayerInput Maps => playerInputActions;

    public static Vector2 DirectionInput
    {
        get
        {
            Vector2 direction = new Vector2();
            direction.x = Maps.Player.XAxisLeftRight.ReadValue<float>();
            direction.y = Maps.Player.YAxisNegativeRoll.ReadValue<float>();
            return direction;
        }
    }

    void Awake() 
    {
        playerInputActions = new PlayerInput();    
    }

    void OnEnable() 
    {
        playerInputActions.Enable();    
    }

    void OnDisable() 
    {
        playerInputActions.Disable();    
    }

    
}