using System;

public abstract class EnemyPathSectionBase 
{
    public LinkedPoint _currentPoint;
    public LinkedPoint _previousPoint;

    public abstract LinkedPoint GetNextPoint();
    public abstract LinkedPoint GetPreviousPoint();
}