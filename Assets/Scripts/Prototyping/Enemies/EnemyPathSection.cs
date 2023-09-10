using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class EnemyPathSection
{
    [SerializeField] bool isUsingRandom;
    [SerializeField] Random random;    
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


        if (nextPoint == null)
        {
            return NextPath != null ? NextPath.GetNextPoint() : END_OF_PATH;
        }
        else
        {
            return END_OF_PATH;
        }
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

        // The points in the orderd they were given using the GetNextPoint function
        List<LinkedPoint> constructedRandomPath = new List<LinkedPoint>();

        public override LinkedPoint GetNextPoint()
        {

            if (_currentPoint == null)
            {
                _currentPoint = GetFirstPoint();
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
                constructedRandomPath.Add(nextPoint);
                _previousPoint = _currentPoint;
                _currentPoint = nextPoint;
                return nextPoint;
            }
            
            #region Local Methods
            LinkedPoint GetFirstPoint()
            {
                return new LinkedPoint(originPoint.position);
            }

            LinkedPoint GetNextRandomPointInArray()
            {
                if (points.Count == 0)
                {
                    return null;
                }

                int index = UnityEngine.Random.Range(0, points.Count);
                points.RemoveAt(index);
                return new LinkedPoint(points[index].position); 
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
                return new LinkedPoint((Vector2)originPoint.position + unitVector);
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
        public override LinkedPoint GetNextPoint()
        {
            throw new NotImplementedException();
        }

        public override LinkedPoint GetPreviousPoint()
        {
            throw new NotImplementedException();
        }
    }
}
