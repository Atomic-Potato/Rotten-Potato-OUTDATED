using UnityEngine;

[System.Serializable]
public class Cloud : MonoBehaviour{
    // Currently has no use but kept just in case
    // [Tooltip("Check this if the cloud is composed of multiple parts")]
    // [SerializeField] bool compositeCloud;
    
    // Doesnt make a difference if its an array or single sprite
    // currently only used to get the width of the sprite
    [SerializeField] SpriteRenderer[] spriteRenderers;

    int orderPosition;

    public int OrderPosition{
        get{ 
            return orderPosition;
        }
        set{
            if(value != CloudsManager.LEFT && value != CloudsManager.MIDDLE && value != CloudsManager.RIGHT)
                throw new System.Exception("The cloud order position " + value + ", is not an accaptable value");
            orderPosition = value;
        }
    }

    public float Width{
        get{
            return spriteRenderers[0].bounds.size.x;
        }
    }
}