using UnityEngine;

public class ProgressArea : MonoBehaviour 
{
    [SerializeField] ProgressBlock entranceBlock;
    [SerializeField] ProgressBlock exitBlock;
    void Update() 
    {
        CloseEntrance();
        OpenExit();
    }

    void CloseEntrance()
    {
        if (entranceBlock == null) 
            return;
        
        if (entranceBlock.TriggeredTag != null && entranceBlock.TriggeredTag == Tags.Tag_Player)
        {
            if (entranceBlock.IsOpen)
                entranceBlock.Close();
        }
    }

    void OpenExit()
    {
        if (exitBlock == null)
            return;
        
        Debug.Log(exitBlock.TriggeredTag + " " + exitBlock.TriggeredTag);

        if (exitBlock.TriggeredTag != null && exitBlock.TriggeredTag == Tags.Tag_MediumEnemy)
        {
            if (exitBlock.IsClosed)
                exitBlock.Open();
        }
    }

    public void Reset()
    {
        if (entranceBlock.IsClosed)
        {
            entranceBlock.Open();
        }

        if (exitBlock.IsOpen)
        {
            exitBlock.Close();
        }
    }
}