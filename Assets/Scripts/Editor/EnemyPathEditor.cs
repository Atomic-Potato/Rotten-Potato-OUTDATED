using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyPathCreator))]
public class EnemyPathEditor : Editor
{
    EnemyPathCreator creator;
    EnemyPath path;

    Event guiEvent;


    void OnEnable()
    {
        creator = (EnemyPathCreator)target;
        if (creator.path == null)
            creator.CreatePath();
        path = creator.path;
    }

    void OnSceneGUI() 
    {
        guiEvent = Event.current;

        DrawPath();

        if (IsReceivedAddPointInput())
        {
            AddPoint();
            guiEvent.Use();
        }

        if (IsReceiviedRemovePointInput())
        {
            RemovePoint();
            guiEvent.Use();
        }
    }


    void DrawPath()
    {

        for (int i = 0; i < path.points.Count; i++)
        {
            // Drawing handles
            Handles.color = creator.HandlesColor;
            Vector3 pointPosition = path.points[i].transform.position;
            Vector3 newPosition = Handles.FreeMoveHandle(pointPosition, Quaternion.identity, creator.HandlesSize, Vector2.zero, Handles.CylinderHandleCap);

            // Updating point position
            if (pointPosition != newPosition)
            {
                Undo.RecordObject(creator, "Move point");
                path.MovePoint(i, newPosition);
            }

            // Drawing the line between handles
            if (i > 0)
            {
                float distanceBetweenHandles = Vector2.Distance(path.points[i-1].transform.position, path.points[i].transform.position);
                Handles.color = distanceBetweenHandles <= creator.SafeDistance ? creator.SafeDistancePathColor : creator.NonSafeDistancePathColor;
                Vector3 previousPointPosition = path.points[i-1].transform.position;
                Handles.DrawLine(previousPointPosition, pointPosition);
            }
        }
    }

    void AddPoint()
    {
        Vector2 mousePosition = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;
        float minDistanceToLine = 0.05f;
        int closestHandleIndex = -1;

        for (int i=1; i < path.points.Count; i++)
        {
            float distance = HandleUtility.DistancePointLine(mousePosition, path.points[i-1].transform.position, path.points[i].transform.position);
            if (distance <= minDistanceToLine)
            {
                closestHandleIndex = i;
                break;
            }
        }

        Undo.RecordObject(creator, "Add point");
        path.AddPoint(mousePosition, closestHandleIndex);
    }

    void RemovePoint()
    {
        float minDistanceToHandle = creator.HandlesSize;
        int closestHandleIndex = -1;
        Vector2 mousePosition = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;
        float distance;

        for (int i=0; i < path.points.Count; i++)
        {
            distance = Vector2.Distance(mousePosition, path.points[i].transform.position);
            if (distance <= minDistanceToHandle)
            {
                minDistanceToHandle = distance;
                closestHandleIndex = i;
            }
        }

        if (closestHandleIndex != -1)
        {
            Undo.RecordObject(creator, "Remove point");
            path.RemovePoint(closestHandleIndex);
        }
    }

    bool IsReceivedAddPointInput()
    {
        return guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift;
    }

    bool IsReceiviedRemovePointInput()
    {
        return guiEvent.type == EventType.MouseDown && guiEvent.button == 1 && guiEvent.shift;
    }
}
