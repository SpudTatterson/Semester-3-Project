using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsManager : MonoBehaviour
{
    [SerializeField] float bottomWorldBoundary = -62f;
    Transform player;
    Vector3 lastPosition;
    GrapplingGun grapplingGun;
    
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerMovement>().transform;
        grapplingGun = player.GetComponentInChildren<GrapplingGun>();
    }

    // Update is called once per frame
    void Update()
    {
        if(player.GetComponent<PlayerMovement>().GiveGrounded())
        {
            lastPosition = player.position;
        }
        if(player.position.y < bottomWorldBoundary)
        {
            ResetPlayerPosition();
        }
    }
    void ResetPlayerPosition()
    {
        player.GetComponent<Rigidbody>().velocity = new Vector3();
        player.position = lastPosition;
        grapplingGun.StopGrapple();
    }
}
