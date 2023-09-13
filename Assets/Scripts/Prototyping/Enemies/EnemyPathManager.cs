using System.Collections;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class EnemyPathManager : MonoBehaviour
{
    [SerializeField] EnemyPathSection[] path;

    int _currentSectionIndex = -1;

    void Awake()
    {
        ConnectPathsSections();
        MoveToNextSection();
        MoveToNextPoint();
    }

    public LinkedPoint GetCurrentPoint()
    {
        return path[_currentSectionIndex].GetCurrentPoint();
    }
    
    public LinkedPoint MoveToNextPoint()
    {
        LinkedPoint point = path[_currentSectionIndex].GetNextPoint();

        if (point == null)
        {
            EnemyPathSection section =  MoveToNextSection();

            if (section == null)
            {
                return null;
            }
            return MoveToNextPoint();
        }

        transform.position = point.position;
        return point;
    }

    public LinkedPoint MoveToPreviousPoint()
    {
        LinkedPoint point = path[_currentSectionIndex].GetPreviousPoint();

        if (point == null)
        {
            EnemyPathSection section =  MoveToPreviousSection();

            if (section == null)
            {
                return null;
            }
            return MoveToPreviousPoint();
        }

        transform.position = point.position;
        return point;
    }

    EnemyPathSection MoveToNextSection()
    {
        _currentSectionIndex++;
        return _currentSectionIndex == path.Length ? null : path[_currentSectionIndex];
    }

    EnemyPathSection MoveToPreviousSection()
    {
        _currentSectionIndex--;
        if (_currentSectionIndex == -1)
        {
            _currentSectionIndex = 0;
        }

        return path[_currentSectionIndex];
    }

    void ConnectPathsSections()
    {
        if (path.Length <= 1)
        {
            path[0].Initialize();
            return;
        }

        if (path.Length > 1)
        {
            path[0].NextPath = path[1];
            path[0].Initialize();
        }

        for (int i=1; i < path.Length; i++)
        {
            path[i].Initialize();

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
