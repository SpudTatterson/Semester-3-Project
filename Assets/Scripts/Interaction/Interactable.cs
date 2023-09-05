using UnityEngine.Events;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] UnityEvent interactEvents;
    [SerializeField] UnityEvent stopInteractEvents;
    [SerializeField] Transform interactionPoint;
    [SerializeField] float interactionDistance = 5f;
    [SerializeField] string interactionText =  "Press \"E\" To Interact.";
    public virtual void Use()
    {

    }
    void Start()
    {
        if(interactionPoint == null)
        {
            interactionPoint = transform;
        }
    }

    public void Interact()
    {
        interactEvents.Invoke();
    }
    public void StopInteract()
    {
        stopInteractEvents.Invoke();
    }
}
