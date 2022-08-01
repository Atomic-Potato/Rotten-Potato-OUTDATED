using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundInfiniteScrolling : MonoBehaviour
{
    [SerializeField] float scrollSpeed = 5f;
    [SerializeField] Vector3 startposition;
    [SerializeField] Vector3 endPosition;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * scrollSpeed * Time.deltaTime);

        if (transform.localPosition.x >= endPosition.x)
        {
            transform.localPosition = startposition;
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 10f);
        }
    }
}
