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
    GameObject interactableObject;
    bool interacting = false;
    SpringJoint joint;
    Rigidbody rb;
    Animator animator;
    ManagersManager managers;
    [SerializeField] Transform orientation;
    bool pulling = false;
    bool pullingRight;
    PullableObject pullable;
    Vector3 hitNormal;


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
            pulling = Input.GetKey(KeyCode.LeftShift);
            
            //UpdateSwingPosition();
            //if (Vector3.Distance(transform.position, grapplePoint) <= releaseDistance)
                //StopGrapple();
        }
    }
    void FixedUpdate()
    {
        if(joint != null) SwingMovement();
        if(interacting && isGrappling)
        {
            if(pullable == null) pullable = interactableObject.GetComponentInParent<PullableObject>();
            if(pullable == null) return;
            Vector3 directionToPlayer = VectorUtility.GetDirection(grapplePoint, VectorUtility.FlattenVector(projectileSpawnPoint.position, grapplePoint.y));
            float angleToPlayer = Vector3.Angle(hitNormal, directionToPlayer);
            if(pulling)
            {
                if(angleToPlayer > 90)
                {
                    StopGrapple();
                    return;
                } 
                if(pullingRight) pullable.MoveRight();
                else pullable.MoveLeft();
            }  
            if(!pulling)
                pullable.Stop();
        }
    }

    Vector3 CheckForSwingPoint()
    {
        RaycastHit rayHit;
        RaycastHit sphereHit;
        Vector3 swingPoint;

        Ray camRay = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(camRay, out rayHit, maxDistance, grappleable))
        {
            swingPoint = rayHit.point;
            if(rayHit.collider.gameObject.layer == 6)
            {
                hitNormal = rayHit.normal;
                interactableObject = rayHit.collider.gameObject;
                Debug.DrawRay(rayHit.point, rayHit.normal, Color.blue, 3f);
                
                
                if(rayHit.normal.x < 0 && rayHit.normal.z < 0)
                {
                    pullingRight = false;
                    Debug.Log("hit left side"); 
                }
                if(rayHit.normal.x > 0 && rayHit.normal.z > 0)
                {
                    Debug.Log("hit right side");
                    pullingRight = true;
                }
                interacting = true;
            } 
            else
            {
                interactableObject = null;
                interacting = false;
            }
            return swingPoint;
        }
        if(Physics.SphereCast(camRay, aimAssistRadius,out sphereHit, maxDistance, grappleable))
        {
            swingPoint = sphereHit.point;
            return swingPoint;
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

            if(!interacting)
            {
                joint = rb.gameObject.AddComponent<SpringJoint>();
                ConfigureJoint();
            }
            

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
        interactableObject = null;
        interacting = false;
        Destroy(joint);
        lr.enabled = false;
    }
}
