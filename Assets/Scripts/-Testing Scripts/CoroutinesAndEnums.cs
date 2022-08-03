using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutinesAndEnums : MonoBehaviour
{
    int counter = 0;
    int oldCounter = -1;

    Coroutine  functionCache = null;

    float time = 69f;

    void Update()
    {
        time += Time.deltaTime;

        Debug.Log(functionCache);
        if(functionCache == null)
            functionCache = StartCoroutine(Function());

        if(oldCounter != counter)
        {
            Debug.Log(counter);
            oldCounter = counter;
        }
    }

    IEnumerator Function()
    {
        counter = 0;
        while(counter < 100)
        {
            yield return null;
            counter++;
        }
    }






    /*
    void Start()
    {
        IEnumerator<bool> evens = Function();
        while(evens.MoveNext())
            Debug.Log(evens);

        GameObject player;
        player.GetComponent 
    }



    IEnumerator<bool> Function()
    {
        for(int i=0; i<5; i++)
        {
            if(i%2 == 0)
                yield return true;
            else
                yield return false;
        }
    }*/
}
