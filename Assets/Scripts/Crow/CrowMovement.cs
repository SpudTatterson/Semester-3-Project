using System.Collections;
using UnityEngine;

public class CrowMovement : MonoBehaviour
{
    [Header("Destinations")]
    [SerializeField] CrowDestination[] crowDestinations;


    int currentLocation = 0;
    bool startedFlying = false;


    [Header("References")]
    Transform player;
    Animator animator;
    

    void Awake()
    {
        animator = GetComponent<Animator>();
        player = FindObjectOfType<PlayerMovement>().transform;
        transform.position = crowDestinations[0].Destination;
    }
    IEnumerator GoToNextLocation()
{
    if (currentLocation + 1 >= crowDestinations.Length)
    {
        // Got to crow this is the win condition so trigger win thing here i guess
        yield break;
    }
    startedFlying = true;
    animator.SetBool("Flying", true);

    Vector3 startPosition = transform.position;
    Vector3 targetPosition = crowDestinations[currentLocation + 1].Destination;
    float journeyLength = Vector3.Distance(startPosition, targetPosition);
    float journeyTime = crowDestinations[currentLocation + 1 ].time;

    float startTime = Time.time;

    while (Time.time < startTime + journeyTime)
    {
        float distanceCovered = (Time.time - startTime) * journeyLength / journeyTime;
        float fractionOfJourney = distanceCovered / journeyLength;
        
        transform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);

        Debug.Log("flying");
        yield return null;
    }
    Debug.Log("finished flying");
    transform.position = targetPosition;
    currentLocation++;
    animator.SetBool("Flying", false);
    startedFlying = false;
}


    void Update() 
    {
        if((Vector3.Distance(player.position, transform.position) < crowDestinations[currentLocation].distance) && !startedFlying)
        {
            StartCoroutine(GoToNextLocation());
        }    
    }
}
