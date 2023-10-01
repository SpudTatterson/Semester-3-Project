using UnityEngine;
using TMPro;

public class GrapplingGun : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float maxDistance = 100f;
    [SerializeField] float aimAssistRadius = 2f;
    [SerializeField] float ThrustForce = 10f;
    [SerializeField] float playerDetectionRadius = 2f;

    [Header("References")]
    [SerializeField] LayerMask grappleable;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] Transform projectileSpawnPoint;
    [SerializeField] Transform HandTarget;
    [SerializeField] TextMeshProUGUI pullText;
    [SerializeField] Transform orientation;
    LineRenderer lr;
    Camera cam;
    PlayerMovement pm;
    Rigidbody rb;
    Animator animator;
    ManagersManager managers;
    GameObject interactableObject;
    SpringJoint joint;
    PullableObject pullable;

    [Header("Private vars")]
    Vector3 grapplePoint;
    bool isGrappling = false;
    bool interacting = false;
    bool pulling = false;
    bool pullingRight;
    Vector3 hitNormal;
    bool NotGrappleable;

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
        if (pm.enabled == false) return;
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
        }
    }
    void FixedUpdate()
    {
        if (joint != null) SwingMovement();
        if (interacting && isGrappling)
        {
            if (pullable == null) pullable = interactableObject.GetComponentInParent<PullableObject>();
            if (pullable == null) return;
            Vector3 directionToPlayer = VectorUtility.GetDirection(grapplePoint, VectorUtility.FlattenVector(projectileSpawnPoint.position, grapplePoint.y));
            float angleToPlayer = Vector3.Angle(hitNormal, directionToPlayer);
            if (pulling)
            {
                if (angleToPlayer > 90)
                {
                    StopGrapple();
                    return;
                }
                if (pullingRight) pullable.MoveRight();
                else pullable.MoveLeft();
            }
            if (!pulling)
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
            CheckIfInteractable(rayHit);
            return swingPoint;
        }
        if (Physics.SphereCast(camRay, aimAssistRadius, out sphereHit, maxDistance, grappleable))
        {
            swingPoint = sphereHit.point;
            CheckIfInteractable(sphereHit);
            return swingPoint;
        }
        // return blank if no potential swinging points are found
        return Vector3.zero;
    }

    void CheckIfInteractable(RaycastHit rayHit)
    {
        if (rayHit.collider.tag == "NotGrappleable")
        {
            NotGrappleable = true;
            return;
        }
        if (rayHit.collider.gameObject.layer == 6)
        {
            hitNormal = rayHit.normal;
            interactableObject = rayHit.collider.gameObject;
            interacting = true;
            CheckHitSide(rayHit);
        }
        else
        {
            interactableObject = null;
            interacting = false;
        }

        void CheckHitSide(RaycastHit rayHit)
        {
            if (rayHit.normal.x < 0 && rayHit.normal.z < 0)
            {
                pullingRight = false;
                Debug.Log("hit left side");
            }
            if (rayHit.normal.x > 0 && rayHit.normal.z > 0)
            {
                Debug.Log("hit right side");
                pullingRight = true;
            }
        }
    }

    void StartGrapple()
    {
        grapplePoint = CheckForSwingPoint();
        if (NotGrappleable)
        {
            NotGrappleable = false;
            StopGrapple();
            return;
        }
        pullText.gameObject.SetActive(true);
        if (grapplePoint != Vector3.zero)
        {
            HandTarget.position = VectorUtility.GetDirection(transform.position, grapplePoint);
            //StartCoroutine(IKRigManager.SetRigWeight(managers.ikRig.rightHandRig, 1, 0.1f));
            animator.SetBool("StartedSwinging", true);
            lr.enabled = true;
            pm.swinging = true;

            if (interacting == false)
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
        joint.minDistance = distanceFromPoint * 0.5f;

        joint.spring = 4.5f;
        joint.damper = 7f;
        joint.massScale = 4.5f;
    }

    void SwingMovement()
    {
        //if(!IsPlayerUnderGrapplePoint()) return;

        Vector3 moveDir = pm.GiveMoveDir();

        if (pulling && !interacting)
        {
            Vector3 directionToPoint = VectorUtility.GetDirection(projectileSpawnPoint.position, grapplePoint);

            rb.AddForce(directionToPoint * ThrustForce * 10 * Time.deltaTime);

            float distanceToPoint = Vector3.Distance(projectileSpawnPoint.position, grapplePoint);

            joint.maxDistance = distanceToPoint * 0.8f;
            joint.minDistance = distanceToPoint * 0.5f;
        }

        rb.AddForce(moveDir * ThrustForce * 10 * Time.deltaTime);
        //rb.AddForce(orientation.forward * forwardThrustForce * 10 * Time.deltaTime, ForceMode.Acceleration);
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

    public void StopGrapple()
    {
        StartCoroutine(IKRigManager.SetRigWeight(managers.ikRig.rightHandRig, 0, 0.1f));
        pullText.gameObject.SetActive(false);
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
