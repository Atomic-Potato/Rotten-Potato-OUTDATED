using UnityEngine;

public class LinkedPoint
{
    public Vector2 position;

    public LinkedPoint Next;
    public LinkedPoint Previous;

    public LinkedPoint(Vector2 position)
    {
        this.position = position;
    }
}