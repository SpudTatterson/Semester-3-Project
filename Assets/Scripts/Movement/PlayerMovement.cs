using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using Cinemachine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 7f;
    [SerializeField] float groundDrag = 5f;

    [Header("Jump Settings")]
    [SerializeField] float jumpForce = 10f;
    [SerializeField] float jumpCoolDown = 0.2f;
    [SerializeField] float airMultiplier = 0.5f;

    [Header("Misc Settings")]
    [SerializeField] float rotationSpeed = 7f;
    bool canJump = true;

    [Header("Ground Check")]
    [SerializeField] float groundOffset = 2f;
    [SerializeField] float groundedRadius;
    [SerializeField] LayerMask whatIsGround;
    bool isGrounded;

    [Header("Key Bindings")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;

    [Header("References")]
    [SerializeField] GameObject cam;
    
    float horizInput;
    float verticalInput;

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
        SpeedControl();
        RotatePlayer();

        rb.drag = isGrounded ? groundDrag : 0;
    }

    void FixedUpdate()
    {
        MovePlayer();
    }
    void GetInput()
    {
        horizInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        
        if(Input.GetKey(jumpKey) && canJump && isGrounded)
        {
            canJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCoolDown);
        }
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

        if(isGrounded)
            rb.AddForce(moveDir.normalized * moveSpeed * 10f, ForceMode.Force);

        else if(!isGrounded)
            rb.AddForce(moveDir.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }
    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    void ResetJump()
    {
        canJump = true;
    }
    void SpeedControl()
    {
        Vector3 flatVel = rb.velocity;
        flatVel.y = 0;

        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
    bool GroundCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y + groundOffset,
        transform.position.z);
        return Physics.CheckSphere(spherePosition, groundedRadius, whatIsGround, QueryTriggerInteraction.Ignore);
    }
    void RotatePlayer()
    {
        if(moveDir != Vector3.zero)
            transform.forward = Vector3.Slerp(transform.forward, moveDir.normalized, Time.deltaTime * rotationSpeed);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y + groundOffset,
        transform.position.z);
        Gizmos.DrawSphere(spherePosition, groundedRadius);
    }
}
