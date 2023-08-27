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
    }

    void SetCloudsOrder(int position){
        foreach(Cloud cloud in clouds){
            cloud.OrderPosition = position;
        }
    }
}