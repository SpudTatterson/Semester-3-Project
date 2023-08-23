using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullableObject : MonoBehaviour
{
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void MoveLeft()
    {
        rb.AddForce(-transform.right * 10f);
    }
    public void MoveRight()
    {
        rb.AddForce(transform.right * 10f);
    }
    public void Stop()
    {
        rb.velocity = Vector3.zero;
    }
}
