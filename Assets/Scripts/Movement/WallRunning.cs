using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] LayerMask Walls;
    [SerializeField] LayerMask Ground;
    [SerializeField] float wallRunForce = 7f;
    [SerializeField] float maxWallRunTime = 3f;
    float wallRunTimer;

    [Header("Input")]
    float horizInput;
    float verticalInput;

    [Header("Detection")]
    [SerializeField] float wallCheckDistance = 0.4f;
    [SerializeField] float minJumpHeight = 0.5f;
    RaycastHit wallLeftHit;
    RaycastHit wallRightHit;
    bool wallLeft;
    bool wallRight;

    [Header("References")]
    PlayerMovement pm;
    Rigidbody rb;
    [SerializeField] Transform orientation;


    void Awake()
    {
        pm = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        CheckForWall();
        StateMachine();
    }
    void FixedUpdate()
    {
        if(pm.wallRunning)
        {
            WallRunningMovement();
            Debug.Log("wall running");
        }
    }
    void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position + transform.up, orientation.right,out wallRightHit, wallCheckDistance, 
        Walls, QueryTriggerInteraction.Ignore);
        wallLeft = Physics.Raycast(transform.position + transform.up, -orientation.right,out wallLeftHit, wallCheckDistance, 
        Walls, QueryTriggerInteraction.Ignore);
        Debug.DrawRay(transform.position + transform.up, orientation.right);
    }
    bool AboveGround()
    {
        if(pm.GiveGrounded()) return false;
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, Ground);
    }
    void StateMachine()
    {
        GetInput();

        Debug.Log(wallLeft + " " + wallRight);

        //WallRunning
        if((wallLeft || wallRight) && verticalInput > 0 && AboveGround())
        {
            if(!pm.wallRunning)  StartWallRun();
        }

        else 
        {
            if(pm.wallRunning) StopWallRun();
        }
    }

    private void GetInput()
    {
        horizInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    void StartWallRun()
    {
        pm.wallRunning = true;
    }
    void StopWallRun()
    {
        pm.wallRunning = false;
    }

    void WallRunningMovement()
    {
        rb.useGravity = false;
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        Vector3 wallNormal = wallRight ? wallRightHit.normal : wallLeftHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
            wallForward = -wallForward;

        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);
    }
}
