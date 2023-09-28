using UnityEngine.Events;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] UnityEvent interactEvents;
    [SerializeField] UnityEvent stopInteractEvents;
    public Transform interactionPoint;
    public float interactionDistance = 2f;
    public string interactionText = "\"E\" To Interact.";
    public virtual void Use()
    {
        Interact();
    }
    void Awake()
    {
        if (interactionPoint == null)
        {
            interactionPoint = transform;
        }
    }

    public void Interact()
    {
        interactEvents.Invoke();
    }
    public void SetAsUnInteractable()
    {
        Destroy(this);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        if(interactionPoint == null) interactionPoint = transform;
        Gizmos.DrawSphere(interactionPoint.position, interactionDistance);
    }
}
