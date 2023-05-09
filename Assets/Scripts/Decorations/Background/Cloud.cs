using UnityEngine;

public class Cloud : MonoBehaviour{
    [SerializeField] SpriteRenderer spriteRenderer;

    int orderPosition;

    public int Order_Position{
        get{ 
            return orderPosition;
        }
        set{
            orderPosition = value;
        }
    }

    public float Width{
        get{
            return spriteRenderer.bounds.size.x;
        }
    }
}