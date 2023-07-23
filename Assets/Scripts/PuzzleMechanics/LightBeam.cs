using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LightBeam : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] LayerMask reflectiveLayer;
    [SerializeField] float lightBeamMaxDistance = 100f;
    [SerializeField] UnityEvent ReceiveBeamEvents;
    [SerializeField] bool shootBeam = true;
    [SerializeField] bool initialBeam = false;

    LineRenderer lr;
    bool receivedBeam = false;
    Vector3 direction;
    Vector3 beamLocation;
    Vector3 hitPoint;
    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        if(initialBeam)
        {
            beamLocation = transform.position;
            direction = transform.forward;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(initialBeam || (receivedBeam && shootBeam)) ShootBeam();
        Debug.DrawRay(beamLocation, direction * 10);
    }

    void ReceiveBeam(RaycastHit hit, Vector3 InitialDirection)
    {
        direction = CalculateNewBeamDirection(InitialDirection ,hit);
        beamLocation = hit.point;
        receivedBeam = true;
        ReceiveBeamEvents.Invoke();
    }
    public void ShootBeam()
    {
        RaycastHit hit;
        if(Physics.Raycast(beamLocation, direction, out hit, lightBeamMaxDistance, reflectiveLayer))
        {
            hitPoint = hit.point;
            LightBeam beam;
            if(beam = hit.transform.GetComponent<LightBeam>())
            {
                beam.ReceiveBeam(hit, direction);
            }
            SetBeamPositions();
        }
        else if(Physics.Raycast(beamLocation, direction, out hit, lightBeamMaxDistance))
        {
            hitPoint = hit.point;
            SetBeamPositions();
        }
    }
    Vector3 CalculateNewBeamDirection(Vector3 inDirection, RaycastHit hit)
    {
        Vector3 newBeamDirection = Vector3.Reflect(inDirection, hit.normal);
        return newBeamDirection;
    }
    void SetBeamPositions()
    {
        lr.SetPosition(0, beamLocation);
        lr.SetPosition(1, hitPoint);
    }
}

