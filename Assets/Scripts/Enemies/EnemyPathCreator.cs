using UnityEngine;

public class EnemyPathCreator : MonoBehaviour 
{
    [SerializeField, Range(0, 100f), Tooltip("Distance that is reachable by the player")]
    float safeDistance = 3f;
    [SerializeField, Range(0, 100f)]
    float handlesSize = .1f;
    [SerializeField] 
    Color handlesColor;
    [SerializeField]
    Color safeDistancePathColor;
    [SerializeField]
    Color nonSafeDistancePathColor;
    [SerializeField]
    Transform pathPointsParent;

    public EnemyPath path;
    public float SafeDistance => safeDistance;
    public float HandlesSize => handlesSize;
    public Color HandlesColor => handlesColor;
    public Color SafeDistancePathColor => safeDistancePathColor;
    public Color NonSafeDistancePathColor => nonSafeDistancePathColor;

    public void CreatePath()
    {
        Transform parent = pathPointsParent == null ? transform : pathPointsParent;
        path = new EnemyPath(parent);
    }

    void OnEnable()
    {
        handlesColor = Color.green;
        safeDistancePathColor = Color.white;    
    }
}