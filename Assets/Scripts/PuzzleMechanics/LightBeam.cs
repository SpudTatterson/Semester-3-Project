using System.Net;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(LineRenderer))]
public class LightBeam : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] LayerMask reflectiveLayer;
    [SerializeField] string reflectiveTag = "Reflective";
    [SerializeField] float lightBeamMaxDistance = 100f;
    [SerializeField] UnityEvent receiveBeamEvents;
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
        lr.enabled = initialBeam; // Disable the lineRenderer by default if the object is not a beam emitter 
    }

    // Update is called once per frame
    void Update()
    {
        if(initialBeam) UpdateBeamDirection();
        if(initialBeam || (receivedBeam && shootBeam)) ShootBeam();
        if(!receivedBeam && !initialBeam) lr.enabled = false;
        receivedBeam = false;       // Make sure that the beam is disabled if nothing is hit
        
        //Debug.DrawRay(beamLocation, direction * 10);
    }
    void UpdateBeamDirection()
    {
        beamLocation = transform.position;
        direction = transform.forward;
    }
    void ReceiveBeam(RaycastHit hit, Vector3 InitialDirection)
    {
        direction = CalculateNewBeamDirection(InitialDirection ,hit);
        beamLocation = hit.point;
        receivedBeam = true;
        lr.enabled = true;
        receiveBeamEvents.Invoke();
    }
    public void ShootBeam()
    {
        RaycastHit hit;
        if(Physics.Raycast(beamLocation, direction, out hit, lightBeamMaxDistance))
        {
            if(hit.transform.tag == reflectiveTag) 
            {
                hitPoint = hit.point;
                //check if the hit object can reflect the beam and trigger the second beam
                LightBeam beam; 
                if(beam = hit.transform.GetComponent<LightBeam>())
                {
                    beam.ReceiveBeam(hit, direction);
                }
                SetBeamPositions();
            }
            else
            {
                hitPoint = hit.point;
                SetBeamPositions();
            }
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

