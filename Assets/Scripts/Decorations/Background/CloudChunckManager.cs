using UnityEngine;

/// <summary>
/// Responsible for setting up the clouds in the chunk
/// </summary>
[System.Serializable]
public class CloudChunckManager : MonoBehaviour{
    [Tooltip("The starting position of the clouds in this chunk (-1:left | 0:Middle | 1:Right)")]
    [Range(-1,1)]
    [SerializeField] int orderPosition;
    [SerializeField] Cloud[] clouds;


    void Awake() {
        SetCloudsOrder(orderPosition);
        SetChunkPosition(orderPosition);
    }

    void SetCloudsOrder(int position){
        foreach(Cloud cloud in clouds){
            cloud.Order_Position = position;
        }
    }

    void SetChunkPosition(int position){
        Vector3 physicalPosition;
        switch (position){
            case 1:
                physicalPosition = new Vector3(0f, 0f, -10f); 
                break;
            case 0:
                physicalPosition = new Vector3(-20f, 0f, -10f); 
                break;
            case -1:
                physicalPosition = new Vector3(-40f, 0f, -10f); 
                break;
            default:
                physicalPosition = Vector3.zero;
                break;
        }
        SetAllCloudsPosition(physicalPosition);
    }

    void SetAllCloudsPosition(Vector3 position){
        foreach(Cloud cloud in clouds){
            cloud.transform.position = position;
        }
    }
}