using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logger : MonoBehaviour
{
    [SerializeField] bool disableAllLogs;

    public void Log(string message, Object caller)
    {
        if(!disableAllLogs)
            Debug.Log(caller.name + ": " + message);
    }
}
