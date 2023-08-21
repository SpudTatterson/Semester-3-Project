using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    [SerializeField] LayerMask grappleable;
    [SerializeField] Transform projectileSpawnPoint;
    [SerializeField] float maxDistance = 100f;
    [SerializeField] float releaseDistance = 2f;
    [SerializeField] float aimAssistRadius = 2f;
    [SerializeField] float ThrustForce = 10f;
    [SerializeField] float playerDetectionRadius = 2f;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] Transform HandTarget;

    LineRenderer lr;
    Vector3 grapplePoint;
    bool isGrappling = false;
    Camera cam;
    //CharacterController characterController;
    Vector3 swingDirection;
    float swingDistance;
    PlayerMovement pm;
    SpringJoint joint;
    Rigidbody rb;
    Animator animator;
    ManagersManager managers;
    [SerializeField] Transform orientation;

    float horizInput;
    float verticalInput;

    void Start()
    {
        managers = FindObjectOfType<ManagersManager>();
        lr = GetComponent<LineRenderer>();
        cam = Camera.main;
        rb = GetComponentInParent<Rigidbody>();
        pm = GetComponentInParent<PlayerMovement>();
        animator = GetComponentInParent<Animator>();
        //characterController = GetComponentInParent<CharacterController>();
        //player = GetComponentInParent<ThirdPersonController>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (!isGrappling)
                StartGrapple();
            else
                StopGrapple();
        }

        if (isGrappling)
        {
            SetLineRendererPositions(grapplePoint);
            HandTarget.position = grapplePoint;
            //UpdateSwingPosition();
            //if (Vector3.Distance(transform.position, grapplePoint) <= releaseDistance)
                //StopGrapple();
        }
    }
    void FixedUpdate()
    {
        if(joint != null) SwingMovement();
    }

    Vector3 CheckForSwingPoint()
    {
        RaycastHit rayHit;
        RaycastHit sphereHit;
        Vector3 SwingPoint;

        Ray camRay = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(camRay, out rayHit, maxDistance, grappleable))
        {
            SwingPoint = rayHit.point;
            return SwingPoint;
        }
        if(Physics.SphereCast(camRay, aimAssistRadius,out sphereHit, maxDistance, grappleable))
        {
            SwingPoint = sphereHit.point;
            return SwingPoint;
        }
        // return blank if no potential swinging points are found
        return Vector3.zero;
    }

    void StartGrapple()
    {   
        grapplePoint = CheckForSwingPoint();
        if(grapplePoint != Vector3.zero)
        {
            HandTarget.position = grapplePoint;
            StartCoroutine(IKRigManager.SetRigWeight(managers.ikRig.rightHandRig, 1, 1f));
            animator.SetBool("StartedSwinging", true);
            lr.enabled = true;
            pm.swinging = true;

            joint = rb.gameObject.AddComponent<SpringJoint>();
            ConfigureJoint();

            isGrappling = true;
        }
    }
    float GetSpeedModifier()
    {
        float speed = rb.velocity.magnitude;
        float speedModifier = Mathf.InverseLerp(0, 14, speed);
        return speedModifier;
    }
    void ConfigureJoint()
    {
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = grapplePoint;

        float distanceFromPoint = Vector3.Distance(pm.transform.position, grapplePoint);

        // the distance grapple will try to keep from grapple point. 
        joint.maxDistance = distanceFromPoint * 0.8f;
        joint.minDistance = distanceFromPoint * 0.3f;

        joint.spring = 4.5f;
        joint.damper = 7f;
        joint.massScale = 4.5f;
    }

    void SwingMovement()
    {
        //if(!IsPlayerUnderGrapplePoint()) return;

        float speedModifier = GetSpeedModifier();
        Vector3 moveDir = pm.GiveMoveDir();

        rb.AddForce(moveDir * ThrustForce * 10 * Time.deltaTime);
        //rb.AddForce(orientation.forward * forwardThrustForce * 10 * Time.deltaTime, ForceMode.Acceleration);
        Debug.DrawRay(transform.position, orientation.forward);
    }

    bool IsPlayerUnderGrapplePoint()
    {
        Ray ray = new Ray(grapplePoint, Vector3.down);
        return Physics.SphereCast(ray, playerDetectionRadius, maxDistance, playerLayer);
    }
    // void StartGrapple()
    // {
    //     rb.useGravity = true;
    //     RaycastHit hit;
    //     Ray camRay = cam.ScreenPointToRay(Input.mousePosition);
    //     if (Physics.Raycast(camRay, out hit, maxDistance, grappleable))
    //     {
    //         lr.enabled = true;
    //         pm.swinging = true; 

    //         grapplePoint = hit.point;
            
    //         joint = rb.gameObject.AddComponent<SpringJoint>();
    //         joint.autoConfigureConnectedAnchor = false;
    //         joint.connectedAnchor = grapplePoint;

    //         float distanceFromPoint = Vector3.Distance(pm.transform.position, grapplePoint);

    //         Debug.Log(distanceFromPoint);

    //         // the distance grapple will try to keep from grapple point. 
    //         joint.maxDistance = distanceFromPoint * 0.8f;
    //         joint.minDistance = distanceFromPoint * 0.3f;

    //         // customize values as you like
    //         joint.spring = 4.5f;
    //         joint.damper = 7f;
    //         joint.massScale = 4.5f;

    //         // swingDirection = (grapplePoint - transform.position).normalized;
    //         // swingDistance = Vector3.Distance(transform.position, grapplePoint);
    //         isGrappling = true;
    //     }
        
    // }

    // void UpdateSwingPosition()
    // {
    //     if(characterController.isGrounded) return;
    //     Vector3 planeGrapplePoint = Vector3.ProjectOnPlane(grapplePoint, Vector3.up);
    //     Vector3 grapplePointPlayerY = new Vector3(planeGrapplePoint.x, transform.position.y, planeGrapplePoint.z);
    //     Debug.DrawLine(transform.position, grapplePointPlayerY);

    //     Vector3 swingForceVector = swingDirection * swingForce;
    //     characterController.Move(swingForceVector * Time.deltaTime);

    //     // Update position to maintain swingDistance
    //     Vector3 swingCurrentPos = transform.position - grapplePoint;
    //     swingCurrentPos = Vector3.ClampMagnitude(swingCurrentPos, swingDistance);
    //     Vector3 wantedPosition = grapplePoint + swingCurrentPos;
    //     Vector3 wantedDirection = wantedPosition + player.transform.forward * 10 - transform.position;
    //     characterController.Move(wantedDirection.normalized * Time.deltaTime);

    //     SetLineRendererPositions(grapplePoint);
    // }

    void SetLineRendererPositions(Vector3 pos)
    {
        lr.SetPosition(0, projectileSpawnPoint.position);
        lr.SetPosition(1, pos);
    }

    void StopGrapple()
    {
        StartCoroutine(IKRigManager.SetRigWeight(managers.ikRig.rightHandRig, 0, 1f));
        animator.SetBool("StartedSwinging", false);
        animator.SetTrigger("StopSwinging");
        pm.swinging = false;
        isGrappling = false;    
        Destroy(joint);
        lr.enabled = false;
    }
}
