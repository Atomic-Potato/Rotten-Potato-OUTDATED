using UnityEngine;

public class LinkedPoint
{
    public Vector2 position;
    public Types type;

    public LinkedPoint Next;
    public LinkedPoint Previous;

    public enum Types
    {
        Random,
        Linear,
        End
    } 

    public LinkedPoint(Vector2 position, Types type)
    {
        this.position = position;
        this.type = type;
    }
}