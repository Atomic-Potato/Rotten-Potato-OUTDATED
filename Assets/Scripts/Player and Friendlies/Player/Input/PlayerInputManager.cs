using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour{
    private static PlayerInput playerInputActions;

    public static PlayerInput Maps => playerInputActions;

    public static bool IsUsingGamePad => Gamepad.all.Count > 0;

    public static Vector2 Direction
    {
        get
        {
            
            Vector2 direction = Maps.Player.Move.ReadValue<Vector2>();

            if (direction.x > 0f)
            {
                direction.x = 1f;
            }
            else if (direction.x < 0f)
            {
                direction.x = -1f;
            }

            if (direction.y > 0f)
            {
                direction.y = 1f;
            }
            else if (direction.y < 0f)
            {
                direction.y = -1f;
            }

            return direction;
        }
    }

    public static Vector2 DirectionRaw => Maps.Player.Move.ReadValue<Vector2>();

    public static Vector2 Aim => Maps.Player.Aim.ReadValue<Vector2>().normalized;
    public static Vector2 AimRaw => Maps.Player.Aim.ReadValue<Vector2>();

    public static bool IsPerformedParry => Maps.Player.Parry.triggered;
    public static bool IsPerformedJump => Maps.Player.Jump.triggered;
    public static bool IsPerformedTheFunny => Maps.Player.TheFunny.triggered;
    public static bool IsPerformedDash => Maps.Player.Dash.triggered;
    public static bool IsPerformedInteract => Maps.Player.Interact.triggered;

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