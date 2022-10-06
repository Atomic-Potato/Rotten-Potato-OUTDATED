using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DELETEME : MonoBehaviour
{
    public Rigidbody2D rb;
    public float velocity;
    Transform pt;
    private void Start() {
        pt = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate() 
    {
        Vector3 direction = (pt.position - transform.position).normalized;
        rb.velocity = new Vector2(-direction.x * velocity, direction.y * velocity * Time.deltaTime);
    }
}
