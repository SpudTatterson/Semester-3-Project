using UnityEngine;
using Cinemachine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 7f;

    [SerializeField] float groundDrag = 5f;

    [Header("Ground Check")]
    [SerializeField] float groundOffset = 2f;
    [SerializeField] float groundedRadius;
    [SerializeField] LayerMask whatIsGround;
    bool isGrounded;

    
    [Header("References")]
    [SerializeField] GameObject cam;
    
    float horizInput;
    float verticalInput;
    float mouseX;
    float mouseY;

    Vector3 moveDir;

    Rigidbody rb;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        isGrounded = GroundCheck();

        GetInput();

        rb.drag = isGrounded ? groundDrag : 0;
    }

    void FixedUpdate()
    {
        MovePlayer();
    }
    void MovePlayer()
    {
        Vector3 inputDir = new Vector3(horizInput, 0, verticalInput);

        Vector3 camForward = cam.transform.forward;
        Vector3 camRight = cam.transform.right;
        
        camForward.y = 0;
        camForward.Normalize();
        camRight.y = 0;
        camRight.Normalize();

        moveDir = inputDir.z * camForward + inputDir.x * camRight;
        rb.AddForce(moveDir.normalized * moveSpeed * 10f, ForceMode.Force);
    }
    void GetInput()
    {
        horizInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }
    bool GroundCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y + groundOffset,
        transform.position.z);
        return Physics.CheckSphere(spherePosition, groundedRadius, whatIsGround, QueryTriggerInteraction.Ignore);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y + groundOffset,
        transform.position.z);
        Gizmos.DrawSphere(spherePosition, groundedRadius);
    }
}
