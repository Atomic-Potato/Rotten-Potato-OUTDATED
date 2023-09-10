using UnityEngine;

public class EnemyPathManager : MonoBehaviour
{
    [SerializeField]
    EnemyPathSection[] path;

    void Awake()
    {
        ConnectPathsSections();
    }

    void ConnectPathsSections()
    {
        if (path.Length <= 1)
        {
            return;
        }

        if (path.Length > 1)
        {
            path[0].NextPath = path[1];
        }

        for (int i=1; i < path.Length; i++)
        {
            if (i == path.Length - 1)
            {
                path[i].PreviousPath = path[i-1];
                break;
            }

            path[i].NextPath = path[i+1];
            path[i].PreviousPath = path[i-1];
        }
    }
}
