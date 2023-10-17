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
    GrapplingEffect grapplingEffect;

    [Header("Private vars")]
    Vector3 grapplePoint;
    bool isGrappling = false;
    bool interacting = false;
    bool pulling = false;
    bool pullingRight;
    Vector3 hitNormal;
    bool NotGrappleable;
    [Header("Constants")]
    const int INTERACTABLE_LAYER = 6;
    private const string NOT_GRAPPLEABLE_TAG = "NotGrappleable";
    private const string LEFT_SIDE_TAG = "LeftSide";
    private const string RIGHT_SIDE_TAG = "RightSide";
    private const float MAX_DISTANCE_MULTIPLIER = 0.8f;
    private const float MIN_DISTANCE_MULTIPLIER = 0.5f;

    void Awake()
    {
        managers = FindObjectOfType<ManagersManager>();
        lr = GetComponent<LineRenderer>();
        cam = Camera.main;
        rb = GetComponentInParent<Rigidbody>();
        pm = GetComponentInParent<PlayerMovement>();
        animator = GetComponentInParent<Animator>();
        grapplingEffect = GetComponent<GrapplingEffect>();
    }

    void Update()
    {
        if (pm.enabled == false) return;
        if (Input.GetButtonDown("Fire1"))
        {
            if (!isGrappling)
                ShootGrapple();
            else
                StopGrapple();
        }

        if (isGrappling)
        {
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
            if (pullable == null) return; // if object is not pullable return
            Vector3 directionToPlayer = VectorUtility.GetDirection(grapplePoint, VectorUtility.FlattenVector(projectileSpawnPoint.position, grapplePoint.y));
            float angleToPlayer = Vector3.Angle(hitNormal, directionToPlayer);
            if (pulling)
            {
                if (angleToPlayer > 90) // if player pulling from wrong side disconnect grapple
                {
                    StopGrapple();
                    return;
                }
                if (pullingRight) pullable.MoveRight();
                else pullable.MoveLeft();
            }
            if (!pulling)
                pullable.Stop(); // stop the object if not actively pulling
        }
    }

    Vector3 CheckForSwingPoint()
    {
        RaycastHit rayHit;
        RaycastHit sphereHit;
        Vector3 swingPoint;

        Ray camRay = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(camRay, out rayHit, maxDistance, grappleable)) // check if hit directly
        {
            swingPoint = rayHit.point;
            CheckIfInteractable(rayHit);
            return swingPoint;
        }
        if (Physics.SphereCast(camRay, aimAssistRadius, out sphereHit, maxDistance, grappleable)) // check if there is a nearby object to swing from
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
        if (rayHit.collider.tag == NOT_GRAPPLEABLE_TAG) // if object is interactable but not grappleable return
        {
            NotGrappleable = true;
            return;
        }
        if (rayHit.collider.gameObject.layer == INTERACTABLE_LAYER)
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

        void CheckHitSide(RaycastHit rayHit) //checks what side of the cart was hit
        {
            if (rayHit.collider.tag == LEFT_SIDE_TAG)
            {
                pullingRight = false;
            }
            else if (rayHit.collider.tag == RIGHT_SIDE_TAG)
            {
                pullingRight = true;
            }
            else
            {
                interactableObject = null;
                interacting = false;
            }
        }
    }

    void ShootGrapple() // starts the swing animation rope animation and sets all the variables
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
            isGrappling = true;
        }
    }

    public void StartSwing() // actually starts the swing gets called when grappling hook hits the surface 
    {
        if (interacting == false)
        {
            joint = rb.gameObject.AddComponent<SpringJoint>();
            ConfigureJoint();
        }
        void ConfigureJoint()
        {
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            SetMaxMinDistance();

            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;
        }
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

    void SwingMovement() // Controls movement while in the air adds ability to pull yourself up
    {
        Vector3 moveDir = pm.GiveMoveDir();

        if (pulling && !interacting)
        {
            Vector3 directionToPoint = VectorUtility.GetDirection(projectileSpawnPoint.position, grapplePoint);

            rb.AddForce(directionToPoint * ThrustForce * 10 * Time.deltaTime);
            SetMaxMinDistance();
        }

        rb.AddForce(moveDir * ThrustForce * 10 * Time.deltaTime);
    }

    private void SetMaxMinDistance()
    {
        float distanceToPoint = Vector3.Distance(projectileSpawnPoint.position, grapplePoint);

        joint.maxDistance = distanceToPoint * MAX_DISTANCE_MULTIPLIER;
        joint.minDistance = distanceToPoint * MIN_DISTANCE_MULTIPLIER;
    }
    #region Public Getters
    public bool IsGrappling()
    {
        return isGrappling;
    }
    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }
    public Vector3 GetGunTip()
    {
        return projectileSpawnPoint.position;
    }
    public LayerMask GetGrappleableLayerMask()
    {
        return grappleable;
    }
    #endregion
}
