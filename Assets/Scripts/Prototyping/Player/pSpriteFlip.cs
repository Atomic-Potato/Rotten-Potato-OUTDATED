using UnityEngine;

public class pSpriteFlip : MonoBehaviour {
    [SerializeField] SpriteRenderer spriteRenderer;
    private void Update() 
    {
        if (IsGoingRight())
        {
            spriteRenderer.flipX = false;
        }
        else if (IsGoingLeft())
        {
            spriteRenderer.flipX = true;
        }
    }

    bool IsGoingRight()
    {
        return PlayerInputManager.Direction.x > 0f;
    }
    
    bool IsGoingLeft()
    {
        return PlayerInputManager.Direction.x < 0f;
    }
}