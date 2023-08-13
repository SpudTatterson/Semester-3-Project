using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBeam : MonoBehaviour
{
    [SerializeField] LayerMask reflectiveLayer;
    [SerializeField] float lightBeamMaxDistance = 100f;
    Vector3 beamDirection;
    LineRenderer lr;
    public List<Vector3> BeamPoints;
    
    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        beamDirection = transform.forward;
        BeamPoints.Add(transform.position);
        lr.SetPosition(0, transform.position);
        ShootBeam(transform.position ,beamDirection);
    }

    // Update is called once per frame
    void Update()
    {
        
        lr.positionCount = BeamPoints.Count;
        for (int i = 0; i < BeamPoints.Count; i++)
        {       
            lr.SetPosition(i, BeamPoints[i]);
        }
    }
    void ShootBeam(Vector3 position ,Vector3 dir)
    {
        Debug.Log("yes");
        
        RaycastHit hit;
        Debug.Log(Physics.Raycast(position, dir, out hit, lightBeamMaxDistance, reflectiveLayer));
        if(Physics.Raycast(position, dir, out hit, lightBeamMaxDistance, reflectiveLayer))
        {
            Vector3 newDir =  CalculateNewBeamDirection(hit);
            Debug.DrawRay(hit.point, newDir * 10);
            if(!BeamPoints.Contains(hit.point)) BeamPoints.Add(hit.point);   
            ShootBeam(hit.point, newDir);
        }
        else
        {

        }
    }
    Vector3 CalculateNewBeamDirection(RaycastHit hit)
    {
        Vector3 newBeamDirection = Vector3.Reflect(beamDirection, hit.normal);
        return newBeamDirection;
    }
}
