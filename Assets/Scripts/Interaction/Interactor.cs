using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float maxInteractionDistance = 4f;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] KeyCode interactKey = KeyCode.F;
    
    [Header("References")]
    Camera cam;
    ManagersManager managers;

    //private vars
    Interactable lastInteractedObject;
    bool interacted;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        managers = FindObjectOfType<ManagersManager>();
        
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit))
        {
            if(hit.collider.gameObject.layer != 6) 
            {
                StopInteract();
                return;
            }
            float distance = Vector3.Distance(transform.position, hit.point);
            if(distance > maxInteractionDistance) 
            {
                StopInteract();
                return;
            }
            managers.uiManager.interactText.gameObject.SetActive(true);
            if(Input.GetKeyDown(interactKey))
            {
                lastInteractedObject = hit.collider.gameObject.GetComponentInParent<Interactable>();
                lastInteractedObject.Interact();
                interacted = true;
            }
            
        }
        else
        {
            StopInteract();

        }
    }

    private void StopInteract()
    {
        managers.uiManager.interactText.gameObject.SetActive(false);
        if (interacted)
        {
            lastInteractedObject.StopInteract();
            interacted = false;
        }
    }
}
