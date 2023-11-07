using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class EnemyPath
{
    [SerializeField]
    public List<GameObject> points = new List<GameObject>();

    Transform parent;

    public EnemyPath(Transform parent)
    {
        this.parent = parent;
    }

    public void MovePoint(int i, Vector3 position)
    {
        points[i].transform.position = position;
    }

    public void AddPoint(Vector3 position, int i = -1)
    {
        GameObject point = new GameObject("EnemyPathPoint");
        point.transform.position = position;
        if (parent != null)
            point.transform.SetParent(parent);
        
        if (i == -1)
            points.Add(point);
        else
            points.Insert(i, point);
    }

    public void RemovePoint(int i)
    {
        GameObject point = points[i];
        points.RemoveAt(i);
        GameObject.DestroyImmediate(point); // ik it is unnecessary, but the compiler will be confused
                                            // since it does not derive from MonoBehaviour
    }
}