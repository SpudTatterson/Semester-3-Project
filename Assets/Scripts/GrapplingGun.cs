using UnityEngine;
using StarterAssets;

public class GrapplingGun : MonoBehaviour
{
    [SerializeField] LayerMask grappleable;
    [SerializeField] Transform projectileSpawnPoint;
    [SerializeField] float maxDistance = 100f;
    [SerializeField] float swingForce = 10f;
    [SerializeField] float releaseDistance = 2f;

    LineRenderer lr;
    Vector3 grapplePoint;
    bool isGrappling = false;
    Camera cam;
    CharacterController characterController;
    Vector3 swingDirection;
    float swingDistance;
    ThirdPersonController player;
    

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        cam = Camera.main;
        characterController = GetComponentInParent<CharacterController>();
        player = GetComponentInParent<ThirdPersonController>();
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
            UpdateSwingPosition();
            if (Vector3.Distance(transform.position, grapplePoint) <= releaseDistance)
                StopGrapple();
        }
    }

    void StartGrapple()
    {
       
        RaycastHit hit;
        Ray camRay = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(camRay, out hit, maxDistance, grappleable))
        {
            grapplePoint = hit.point;
            SetLineRendererPositions(grapplePoint);
            swingDirection = (grapplePoint - transform.position).normalized;
            swingDistance = Vector3.Distance(transform.position, grapplePoint);
        }
        isGrappling = true;
        player.useGravity = false;
    }

    void UpdateSwingPosition()
    {
        if(characterController.isGrounded) return;
        Vector3 planeGrapplePoint = Vector3.ProjectOnPlane(grapplePoint, Vector3.up);
        Vector3 grapplePointPlayerY = new Vector3(planeGrapplePoint.x, transform.position.y, planeGrapplePoint.z);
        Debug.DrawLine(transform.position, grapplePointPlayerY);

        Vector3 swingForceVector = swingDirection * swingForce;
        characterController.Move(swingForceVector * Time.deltaTime);

        // Update position to maintain swingDistance
        Vector3 swingCurrentPos = transform.position - grapplePoint;
        swingCurrentPos = Vector3.ClampMagnitude(swingCurrentPos, swingDistance);
        Vector3 wantedPosition = grapplePoint + swingCurrentPos;
        Vector3 wantedDirection = wantedPosition + player.transform.forward * 10 - transform.position;
        characterController.Move(wantedDirection.normalized * Time.deltaTime);

        SetLineRendererPositions(grapplePoint);
    }

    void SetLineRendererPositions(Vector3 pos)
    {
        lr.SetPosition(0, projectileSpawnPoint.position);
        lr.SetPosition(1, pos);
    }

    void StopGrapple()
    {
        isGrappling = false;
        player.useGravity = true;
    }
}
