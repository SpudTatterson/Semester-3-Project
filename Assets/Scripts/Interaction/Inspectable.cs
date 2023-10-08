using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class Inspectable : Interactable
{
    [SerializeField] CinemachineVirtualCamera cam;
    bool active = false;
    public override void Use(Interactor interactor)
    {
        if (!active) Inspect(interactor);
        else StopInspect(interactor);
        active = !active;
    }

    private void StopInspect(Interactor interactor)
    {
        interactor.TogglePlayerMovement();
        cam.Priority = 1;
    }

    private void Inspect(Interactor interactor)
    {
        interactor.TogglePlayerMovement();
        cam.Priority = 11;
    }
}
