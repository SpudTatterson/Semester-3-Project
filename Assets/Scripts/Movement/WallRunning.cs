using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] LayerMask Walls;
    [SerializeField] LayerMask Ground;
    [SerializeField] float wallRunForce = 7f;
    [SerializeField] float maxWallRunTime = 3f;
    [SerializeField] float wallJumpUpForce = 5f;
    [SerializeField] float wallJumpSideForce = 10f;

    [SerializeField] float wallRunTime;
    float wallRunTimer;

    [Header("Input")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    float horizInput;
    float verticalInput;

    [Header("Exiting")]
    bool exitingWall;
    [SerializeField] float exitWallTime = .5f;
    float exitWallTimer;

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
    Animator animator;
    [SerializeField] Transform orientation;

    Vector3 forceToApply;


    void Awake()
    {
        pm = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        CheckForWall();
        StateMachine();

        Debug.DrawRay(transform.position, forceToApply);
    }
    void FixedUpdate()
    {
        if(pm.wallRunning)
        {
            WallRunningMovement();
        }
    }
    void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position + transform.up, orientation.right,out wallRightHit, wallCheckDistance, 
        Walls, QueryTriggerInteraction.Ignore);
        wallLeft = Physics.Raycast(transform.position + transform.up, -orientation.right,out wallLeftHit, wallCheckDistance, 
        Walls, QueryTriggerInteraction.Ignore);
    }
    bool AboveGround()
    {
        if(pm.GiveGrounded()) return false;
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, Ground);
    }
    void StateMachine()
    {
        GetInput();

        //WallRunning
        if((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !exitingWall)
        {
            if(!pm.wallRunning)  StartWallRun();

            if(Input.GetKeyDown(jumpKey)) WallJump();
        }

        else if(exitingWall)
        {
            if(pm.wallRunning) StopWallRun();

            if(exitWallTimer > 0) exitWallTimer -= Time.deltaTime;
                
            if(exitWallTimer <= 0)  exitingWall = false;

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
        animator.SetBool("StartedWallRunning", true);
    }
    void StopWallRun()
    {
        pm.wallRunning = false;
        animator.SetBool("StartedWallRunning", false);
    }

    void WallRunningMovement()
    {
        rb.useGravity = false;
        rb.velocity = VectorUtility.FlattenVector(rb.velocity);

        Vector3 wallNormal = wallRight ? wallRightHit.normal : wallLeftHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
            wallForward = -wallForward;

        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);
    }

    void WallJump()
    {
        Debug.Log("wall jumped");
        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 wallNormal = wallRight ? wallRightHit.normal : wallLeftHit.normal;

        forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        Debug.DrawRay(transform.position, forceToApply);
        rb.velocity = VectorUtility.FlattenVector(rb.velocity);

        rb.AddForce(forceToApply, ForceMode.Force);
    }
}
