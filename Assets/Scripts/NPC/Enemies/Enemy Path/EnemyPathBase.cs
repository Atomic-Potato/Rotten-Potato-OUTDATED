using System;

public abstract class EnemyPathSectionBase 
{
    public LinkedPoint _currentPoint;
    public LinkedPoint _previousPoint;


    public abstract int GetCurrentPointIndex();
    public abstract int GetSectionLength();
    public abstract LinkedPoint GetCurrentPoint();
    public abstract LinkedPoint GetNextPoint();
    public abstract LinkedPoint GetPreviousPoint();
    public abstract void Reset();
}