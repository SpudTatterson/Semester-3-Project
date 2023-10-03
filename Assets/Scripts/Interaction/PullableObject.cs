using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PullableObject : MonoBehaviour
{
    [SerializeField] float pullStrength = 5f;
    Rigidbody rb;
    bool isMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
      if(!isMoving) Stop();   
    }
    public void MoveLeft()
    {
        isMoving = true;
        rb.AddForce(-transform.right * rb.mass * pullStrength);
    }
    public void MoveRight()
    {
        isMoving = true;
        rb.AddForce(transform.right * rb.mass * pullStrength);
    }
    public void Stop()
    {
        isMoving = false;
        rb.velocity = Vector3.zero;
    }
}
