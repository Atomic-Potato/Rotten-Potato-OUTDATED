using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


[Serializable]
public class EnemyPathSection
{
    [Header("Random")]
    [SerializeField] bool isUsingRandom;
    [SerializeField] Random random;

    [Header("Linear")]
    [SerializeField] bool isUsingLinear;
    [SerializeField] Linear linear;
    
    [HideInInspector]
    public static readonly LinkedPoint END_OF_PATH = null;

    public EnemyPathSection NextPath;
    public EnemyPathSection PreviousPath;


    public LinkedPoint GetNextPoint()
    {
        LinkedPoint nextPoint = null;
        if (isUsingRandom && random != null)
        {
            nextPoint = random.GetNextPoint();
        }
        else if (isUsingLinear && linear != null)
        {
            nextPoint = linear.GetNextPoint();
        }
        

        return nextPoint;
    }

    public LinkedPoint GetPreviousPoint()
    {
        LinkedPoint previousPoint = null;
        if (isUsingRandom && random != null)
        {
            previousPoint = random.GetNextPoint();
        }
        else if (isUsingLinear && linear != null)
        {
            previousPoint = linear.GetNextPoint();
        }

        if (previousPoint == null)
        {
            return PreviousPath != null ? PreviousPath.GetPreviousPoint() : END_OF_PATH;
        }
        else
        {
            return END_OF_PATH;
        }
    }

    [Serializable]
    class Random : EnemyPathSectionBase
    {
        [Space]
        [Range(0f, 20f)]
        [SerializeField] float pointsRange;
        [SerializeField] Transform originPoint;

        [Space]
        [Header("Manual Points")]
        [SerializeField] bool isUsingManualPoints;
        [SerializeField] List<Transform> points = new List<Transform>();

        [Space]
        [Header("Random Points")]
        [SerializeField] bool isUsingRandomPoints;
        [SerializeField] int numberOfPoints;

        int _manualPointsUsed = 0;

        public override LinkedPoint GetNextPoint()
        {
            if (_currentPoint == null)
            {
                _currentPoint = GetFirstPoint();
                return _currentPoint;
            }

            if (_currentPoint.Next != null)
            {
                _currentPoint = _currentPoint.Next;
                return _currentPoint;
            }

            LinkedPoint nextPoint = null;
            if (isUsingManualPoints)
            {
                nextPoint = GetNextRandomPointInArray();
            }
            else if (isUsingRandomPoints)
            {
                nextPoint =  GenerateRandomPointWithinRange();
            }
            else
            {
                throw new Exception("No next method to get next point found in path");
            }

            if (nextPoint == null)
            {
                return null;
            }
            else
            {
                _currentPoint.Next = nextPoint;
                _currentPoint.Previous = _previousPoint;

                _previousPoint = _currentPoint;
                _currentPoint = nextPoint;

                return nextPoint;
            }
            
            #region Local Methods
            LinkedPoint GetFirstPoint()
            {
                return new LinkedPoint(originPoint.position, LinkedPoint.Types.Random);
            }

            LinkedPoint GetNextRandomPointInArray()
            {
                if (_manualPointsUsed == points.Count)
                {
                    return null;
                }

                int index = UnityEngine.Random.Range(0, points.Count - _manualPointsUsed);
                Debug.Log("Index: " + index + " Count: " + points.Count);
                LinkedPoint point = new LinkedPoint(points[index].position, LinkedPoint.Types.Random);
                SwitchPointsInArray(index, (points.Count - 1) - _manualPointsUsed);
                _manualPointsUsed++;
                return point; 
            }

            LinkedPoint GenerateRandomPointWithinRange()
            {
                if (numberOfPoints <= 0)
                {
                    return null;
                }
                numberOfPoints--;

                Vector2 unitVector = new Vector2(
                    UnityEngine.Random.Range(-1f, 1f),
                    UnityEngine.Random.Range(-1f, 1f)
                    );

                unitVector *= pointsRange;
                return new LinkedPoint((Vector2)originPoint.position + unitVector, LinkedPoint.Types.Random);
            }
            
            void SwitchPointsInArray(int index1, int index2)
            {
                Transform auxilary = points[index1];
                points[index1] = points[index2];
                points[index2] = auxilary;
            }
            #endregion
        }

        public override LinkedPoint GetPreviousPoint()
        {
            return _previousPoint;
        }
    }

    [Serializable]
    class Linear : EnemyPathSectionBase
    {
        [Tooltip("Enemy will move in the same order of the points")]
        [SerializeField] Transform[] points;

        int _currentIndex = -1;

        public override LinkedPoint GetNextPoint()
        {
            if (points.Length == 0)
            {
                throw new Exception("Linear section contains no points");
            }

            if (_currentIndex == points.Length)
            {
                return END_OF_PATH;
            }
            
            _currentIndex++;
            if (_currentPoint == null)
            {
                _currentPoint = new LinkedPoint(points[_currentIndex].position, LinkedPoint.Types.Linear);
                return _currentPoint;
            }

            if (_currentPoint.Next == null)
            {
                _currentPoint.Next = new LinkedPoint(points[_currentIndex].position, LinkedPoint.Types.Linear);
                _currentPoint.Previous = _previousPoint;

                _previousPoint = _currentPoint;
                return _currentPoint.Next;
            }

            return _currentPoint.Next;
        }

        public override LinkedPoint GetPreviousPoint()
        {
            if (points.Length == 0)
            {
                throw new Exception("Linear section contains no points");
            }

            if (_currentIndex == -1)
            {
                throw new Exception("Enemy still did not go throught the linear section");
            }

            _currentIndex--;
            if (_currentIndex == -1)
            {
                throw new Exception("Path cant continue past the current point");
            }

            if (_currentPoint == null)
            {
                throw new Exception("Previous point don't exist. (Enemy should go through the path before going back)");
            }
            
            _currentPoint = _currentPoint.Previous;
            return _currentPoint;
        }
    }
}
