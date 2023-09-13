using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class EnemyPathSection
{
    [Space]
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
    
    // Equivalent to Awake
    public void Initialize()
    {
        if (isUsingRandom && random != null)
        {
            random.Initialize();
        }
    }

    public LinkedPoint GetCurrentPoint()
    {
        if (isUsingRandom && random != null)
        {
            return random.GetCurrentPoint();
        }
        else if (isUsingLinear && linear != null)
        {
            return linear.GetCurrentPoint();
        }
        else
        {
            return null;
        }
    }

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
            previousPoint = random.GetPreviousPoint();
        }
        else if (isUsingLinear && linear != null)
        {
            previousPoint = linear.GetPreviousPoint();
        }
        return previousPoint;
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

        [Space]
        [Header("Other")]
        [SerializeField] Transform enemyTransform;
        [SerializeField] Collider2D collider;

        public bool IsUsingManualPoints => isUsingManualPoints;
        public bool IsUsingRandomPoints => isUsingRandomPoints;

        int _manualPointsUsed = 0;
        float _colliderMaxLength = 0f;

        // Equivalent to Awake
        public void Initialize() 
        {
            if (isUsingRandomPoints)
            {
                _colliderMaxLength = GetColliderMaximumLength();
                if (_colliderMaxLength > pointsRange)
                {
                    throw new Exception(
                        "Points range cannot be saller the collider max length.\n" +
                        "Decrease the collider size or increase the points range."
                        );
                }
            }
        }

        public override LinkedPoint GetCurrentPoint()
        {
            return _currentPoint;
        }

        public override LinkedPoint GetNextPoint()
        {
            if (_currentPoint == null)
            {
                _currentPoint = GetFirstPoint();
                return _currentPoint;
            }
            else if (_currentPoint.Next != null)
            {
                _previousPoint = _currentPoint;
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
                _previousPoint = _currentPoint;
                _currentPoint = END_OF_PATH;
            }
            else
            {
                _previousPoint = _currentPoint;
                _currentPoint = nextPoint;
                
                _previousPoint.Next = _currentPoint;
                _currentPoint.Previous = _previousPoint;
            }

            return _currentPoint;
            
            #region Local Methods
            LinkedPoint GetNextRandomPointInArray()
            {
                if (_manualPointsUsed == points.Count)
                {
                    return null;
                }

                int index = UnityEngine.Random.Range(0, points.Count - _manualPointsUsed);
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

                Vector2 playerDirectionSigns = GetPlayerDirectionSigns();
                Vector2 newPosition;
                float distanceFromPlayer;

                do{
                    Vector2 unitVector = new Vector2(
                        UnityEngine.Random.Range(0f, -1f * playerDirectionSigns.x),
                        UnityEngine.Random.Range(0f, -1f * playerDirectionSigns.y)
                        ).normalized;
                    newPosition = (Vector2)originPoint.position + unitVector * pointsRange;
                    distanceFromPlayer = Vector2.Distance(newPosition, pPlayer.Instance.transform.position);
                }while (distanceFromPlayer < _colliderMaxLength);

                return new LinkedPoint(newPosition, LinkedPoint.Types.Random);
            }
            
            void SwitchPointsInArray(int index1, int index2)
            {
                Transform auxilary = points[index1];
                points[index1] = points[index2];
                points[index2] = auxilary;
            }
            
            Vector2 GetPlayerDirectionSigns()
            {
                Vector2 signs = (enemyTransform.position - pPlayer.Instance.transform.position).normalized;
                signs /= new Vector2(Mathf.Abs(signs.x), Mathf.Abs(signs.y));
                return signs;
            }
            #endregion
        }

        public override LinkedPoint GetPreviousPoint()
        {
            if (_currentPoint == END_OF_PATH)
            {
                _currentPoint = _previousPoint;
                _previousPoint = _previousPoint?.Previous;
            }
            else
            {
                _previousPoint = _previousPoint?.Previous;
                _currentPoint = _currentPoint?.Previous;
            }

            return _currentPoint;
        }
    
        float GetColliderMaximumLength()
            {
                if (collider is BoxCollider2D)
                {
                    float x = ((BoxCollider2D)collider).size.x;
                    float y = ((BoxCollider2D)collider).size.y;
                    return Mathf.Sqrt(x*x + y*y);
                }
                else if(collider is CircleCollider2D)
                {
                    return ((CircleCollider2D)collider).radius * 2f;
                }
                else if(collider is CapsuleCollider2D)
                {
                    float x = ((CapsuleCollider2D)collider).size.x;
                    float y = ((CapsuleCollider2D)collider).size.y;
                    return Mathf.Sqrt(x*x + y*y);
                }
                else
                {
                    throw new Exception("Incompatible collider type");
                }
            }
    
        LinkedPoint GetFirstPoint()
        {
            return new LinkedPoint(originPoint.position, LinkedPoint.Types.Random);
        }
    }

    [Serializable]
    class Linear : EnemyPathSectionBase
    {
        [Tooltip("Enemy will move in the same order of the points")]
        [SerializeField] Transform[] points;

        int _currentIndex = -1;

        public override LinkedPoint GetCurrentPoint()
        {
            return _currentPoint;
        }

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
            if (_currentIndex == points.Length)
            {
                if (_currentPoint == END_OF_PATH)
                {
                    return END_OF_PATH;
                }

                _previousPoint = _currentPoint;
                _currentPoint = null;
                return END_OF_PATH;
            }
            else if (_currentPoint == null)
            {
                _currentPoint = new LinkedPoint(points[_currentIndex].position, LinkedPoint.Types.Linear);
                return _currentPoint;
            }
            
            LinkedPoint nextPoint = _currentPoint.Next == null ? 
                new LinkedPoint(points[_currentIndex].position, LinkedPoint.Types.Linear) :
                _currentPoint.Next;

            _previousPoint = _currentPoint;
            _currentPoint = nextPoint;

            _previousPoint.Next = _currentPoint;
            _currentPoint.Previous = _previousPoint;

            return _currentPoint;
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
            if (_currentPoint == END_OF_PATH && _previousPoint != null)
            {
                _currentPoint = _previousPoint;
                _previousPoint = _previousPoint?.Previous;
            }            
            else
            {
                _previousPoint = _previousPoint?.Previous;
                _currentPoint = _currentPoint?.Previous;
            }
            return _currentPoint;
        }
    }
}
