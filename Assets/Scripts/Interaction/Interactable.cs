using UnityEngine.Events;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] UnityEvent interactEvents;
    [SerializeField] UnityEvent stopInteractEvents;

    public void Interact()
    {
        interactEvents.Invoke();
    }
    public void StopInteract()
    {
        stopInteractEvents.Invoke();
    }
}
