using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CrowMovement : MonoBehaviour
{
    [Header("Destinations")]
    [SerializeField] CrowDestination[] crowDestinations;
    [Header("Crow Actions")]
    [SerializeField] int ringLocation;
    [SerializeField] UnityEvent ringPickUpEvents;   


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
        animator.SetBool("Eating", false);
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = crowDestinations[currentLocation + 1].Destination;
        transform.rotation = Quaternion.LookRotation(targetPosition);
        float journeyLength = Vector3.Distance(startPosition, targetPosition);
        float journeyTime = crowDestinations[currentLocation + 1].time;

        float startTime = Time.time;

        while (Time.time < startTime + journeyTime)
        {
            float distanceCovered = (Time.time - startTime) * journeyLength / journeyTime;
            float fractionOfJourney = distanceCovered / journeyLength;

            transform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);

            yield return null;
        }
        transform.position = targetPosition;
        currentLocation++;
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        animator.SetBool("Flying", false);
        startedFlying = false;
    }


    void Update()
    {
        if(currentLocation == ringLocation)
        {
            transform.rotation = Quaternion.Euler(0, 328.259f,0);
            ringPickUpEvents.Invoke();
        }
        if ((Vector3.Distance(player.position, transform.position) < crowDestinations[currentLocation].distance) && !startedFlying)
        {
            StartCoroutine(GoToNextLocation());
        }
    }
}
