using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(LineRenderer))]
public class LightBeam : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Light lightObject;
    [SerializeField] Transform initialBeamShootPoint;
    [SerializeField] Material mat;


    [Header("Settings")]
    [SerializeField] string reflectiveTag = "Reflective";
    [SerializeField] float lightBeamMaxDistance = 100f;
    [SerializeField] UnityEvent receiveBeamEvents;
    [SerializeField] bool shootBeam = true;
    [SerializeField] bool initialBeam = false;
    [SerializeField] float distance;
    [SerializeField] int beamsRequiredToActivate = 1;

    LineRenderer lr;
    bool receivedBeam = false;
    Vector3 direction;
    Vector3 beamLocation;
    Vector3 hitPoint;
    List<LightBeam> receivedBeams = new List<LightBeam>();
    Color emissionColor;
    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.enabled = initialBeam; // Disable the lineRenderer by default if the object is not a beam emitter 
        if(mat)
        {   
            emissionColor = new Color(0.7490196f,0.7490196f,0.7490196f);
            mat.SetColor("_EmissionColor", emissionColor);
        } 
    }

    // Update is called once per frame
    void Update()
    {
        if(initialBeam) UpdateBeamDirection();
        if(initialBeam || (receivedBeam && shootBeam)) ShootBeam();
        if(!receivedBeam && !initialBeam) lr.enabled = false;
        if(lightObject) UpdateLight();
        receivedBeam = false;       // Make sure that the beam is disabled if nothing is hit
        
        //Debug.DrawRay(beamLocation, direction * 10);      
    }
    void UpdateBeamDirection()
    {
        beamLocation = initialBeamShootPoint.position;
        direction = transform.forward;
        receivedBeam = true;
    }
    void UpdateLight()
    {
        lightObject.range = distance * 1.5f;
        lightObject.transform.position = beamLocation;
        if(direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, transform.up);
            lightObject.transform.rotation = targetRotation;
        }
        lightObject.enabled = receivedBeam;
    }
    void ReceiveBeam(RaycastHit hit, Vector3 InitialDirection)
    {
        if(mat) mat.SetColor("_EmissionColor", emissionColor * 7 * receivedBeams.Count);
        direction = CalculateNewBeamDirection(InitialDirection ,hit);
        beamLocation = hit.point;
        receivedBeam = true;
        lr.enabled = true;
        if(receivedBeams.Count >= beamsRequiredToActivate) receiveBeamEvents.Invoke();
        
    }
    public void ShootBeam()
    {
        RaycastHit hit;
        if(Physics.Raycast(beamLocation, direction, out hit, lightBeamMaxDistance))
        {
            if(hit.transform.tag == reflectiveTag) //check if the hit object can reflect the beam and trigger the second beam 
            {      
                LightBeam beam; 
                if(beam = hit.transform.GetComponentInParent<LightBeam>())
                {
                    if(!beam.receivedBeams.Contains(this))
                    {
                        beam.receivedBeams.Add(this);
                        
                    }
                    beam.ReceiveBeam(hit, direction);
                }        
            }
            SetBeamPositions();
            hitPoint = hit.point;
            distance = hit.distance;
        }
        else
        {
            Vector3 pointAlongRay = beamLocation + direction * lightBeamMaxDistance;
            hitPoint = pointAlongRay;
            distance = lightBeamMaxDistance;
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

