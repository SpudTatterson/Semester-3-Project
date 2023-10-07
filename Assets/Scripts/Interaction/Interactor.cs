using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float maxInteractionDistance = 4f;
    [SerializeField] float interactionRadius = 0.5f;
    [SerializeField] LayerMask interactMask;
    [SerializeField] KeyCode interactKey = KeyCode.F;
    [SerializeField] LayerMask playerMask;

    [Header("References")]
    Camera cam;
    [SerializeField] Camera secondCamera;
    ManagersManager managers;
    PlayerMovement pm;


    //private vars
    Interactable lastInteractedObject;
    bool interacted;
    const int interactableLayer = 6;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        managers = FindObjectOfType<ManagersManager>();
        pm = FindObjectOfType<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.SphereCast(ray, interactionRadius, out hit, maxInteractionDistance, interactMask))
        {
            if ((hit.collider.gameObject.layer != interactableLayer) /*|| (hit.distance > maxInteractionDistance)*/)
            {
                StopInteract();
                return;
            }
            lastInteractedObject = hit.collider.gameObject.GetComponentInParent<Interactable>();
            if (lastInteractedObject)
            {
                managers.UI.interactText.text = lastInteractedObject.interactionText;
                if (Physics.CheckSphere(lastInteractedObject.interactionPoint.position,
                 lastInteractedObject.interactionDistance, playerMask))
                {
                    if (interacted == false) managers.UI.interactText.gameObject.SetActive(true);
                    if (Input.GetKeyDown(interactKey))
                    {
                        lastInteractedObject.Use(this);
                        interacted = true;
                        managers.UI.interactText.gameObject.SetActive(false);
                    }
                }
            }

        }
        else
        {
            StopInteract();
        }
    }

    private void StopInteract()
    {
        managers.UI.interactText.gameObject.SetActive(false);
        if (interacted)
        {
            interacted = false;
        }
    }
    public void ToggleCameras()
    {
        cam.gameObject.SetActive(!cam.isActiveAndEnabled);
        secondCamera.gameObject.SetActive(!secondCamera.isActiveAndEnabled);
    }
    public void TogglePlayerMovement()
    {
        pm.enabled = !pm.enabled;
    }
    public void SetSecondCamPosition(Vector3 position, Quaternion rotation, Transform parent)
    {
        secondCamera.transform.position = position;
        secondCamera.transform.rotation = rotation;
        secondCamera.transform.parent = parent;
    }
}
