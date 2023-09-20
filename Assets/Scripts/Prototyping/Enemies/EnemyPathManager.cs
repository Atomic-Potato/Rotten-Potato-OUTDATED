using System.Collections;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class EnemyPathManager : MonoBehaviour
{
    [SerializeField] EnemyPathSection[] path;
    [SerializeField] Transform ghostTransform;

    int _currentSectionIndex = -1;

    void Awake()
    {
        ConnectPathsSections();
        MoveToNextSection();
        MoveToNextPoint();
    }

    /// <summary>
    /// Returns the index of the point in the current section.
    /// -1 in case of failure
    /// </summary>
    public int GetCurrentPointIndex()
    {
        return path[_currentSectionIndex] != null ? path[_currentSectionIndex].GetCurrentPointIndex() : -1;
    }

    public int GetCurrentSectionLength()
    {
        return path[_currentSectionIndex] == null ? 0 : path[_currentSectionIndex].GetSectionLength();
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

        if (point.Previous != null)
            if (point.Previous.Previous != null)
                ghostTransform.position = point.Previous.Previous.position;

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
            return null;
        }

        return path[_currentSectionIndex];
    }

    /// <summary>
    /// Resets all indicies and points
    /// </summary>
    public void Reset()
    {
        _currentSectionIndex = -1;
        foreach (EnemyPathSection section in path)
        {
            section.Reset();
        }
        MoveToNextSection();
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
