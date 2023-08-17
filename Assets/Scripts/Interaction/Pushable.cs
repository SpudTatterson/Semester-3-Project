using UnityEngine;

public class Pushable : MonoBehaviour
{
    [SerializeField] float pushForce = 5f;
    [SerializeField] float maxAllowedAngle = 45f;  
    [SerializeField] float constraintDistance = 4f;

    Vector3 startingPosition;
    Vector3 constraintPosition;
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        startingPosition = transform.position;
    }

    void Update()
    {
        if (rb.velocity.magnitude > 0.01)
        {
            if (ReachedConstraintsCheck())
            {
                rb.velocity = Vector3.zero;
                //transform.position = constraintPosition;
            }
            ConstrainMovementToLocalXAxis();
        }

    }

    private void ConstrainMovementToLocalXAxis()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
        localVelocity.y = 0;
        localVelocity.z = 0;

        rb.velocity = transform.TransformDirection(localVelocity);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) 
        {
            Debug.Log(collision.contacts[0].normal);
        }
    }
    bool ReachedConstraintsCheck()
    {
        float distance = Vector3.Distance(transform.position, startingPosition);
        //if(distance > constraintDistance && constraintPosition == Vector3.zero) constraintPosition = transform.position;
        return (distance > constraintDistance);
    }

}
