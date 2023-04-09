using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField] Rigidbody2D rigidBody;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    Vector2 refVelocity;

    // Update is called once per frame
    void Update()
    {
        rigidBody.velocity = Vector2.SmoothDamp(rigidBody.velocity, transform.right * 10f, ref refVelocity, 3f);
    }
}
