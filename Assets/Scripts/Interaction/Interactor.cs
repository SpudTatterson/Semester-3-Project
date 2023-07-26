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
        if(Physics.Raycast(ray,out hit, maxInteractionDistance))
        {
            if(hit.collider.gameObject.layer != 6) return;
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
            managers.uiManager.interactText.gameObject.SetActive(false); 
            if(interacted)
            {
                lastInteractedObject.StopInteract(); 
                interacted = false;
            }
                  
        }
    }
}
