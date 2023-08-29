using System.Collections;
using UnityEngine;

public class CrowMovement : MonoBehaviour
{
    //[SerializeField] Vector3[] locations;
    //[SerializeField] float[] timeToNextLocation;
    [SerializeField] CrowDestination[] crowDestinations;
    public int currentLocation = 0;
    float timer;
    Transform player;
    bool startedFlying = false;

    void Awake()
    {
        player = FindObjectOfType<PlayerMovement>().transform;
        transform.position = crowDestinations[0].Destination;
    }
    IEnumerator GoToNextLocation()
{
    if (currentLocation + 1 >= crowDestinations.Length)
    {
        // Crow has reached its final destination, you can handle this case
        yield break;
    }
    startedFlying = true;
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
