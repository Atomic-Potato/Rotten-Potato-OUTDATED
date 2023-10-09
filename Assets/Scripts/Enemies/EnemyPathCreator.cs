using UnityEngine;

public class EnemyPathCreator : MonoBehaviour 
{
    [SerializeField, Range(0, 100f)]
    float handlesSize = .1f;
    [SerializeField] 
    Color handlesColor;
    [SerializeField]
    Color pathColor;
    [SerializeField]
    Transform pathPointsParent;

    public EnemyPath path;
    public float HandlesSize => handlesSize;
    public Color HandlesColor => handlesColor;
    public Color PathColor => pathColor;

    public void CreatePath()
    {
        Transform parent = pathPointsParent == null ? transform : pathPointsParent;
        path = new EnemyPath(parent);
    }

    void OnEnable()
    {
        handlesColor = Color.green;
        pathColor = Color.white;    
    }
}