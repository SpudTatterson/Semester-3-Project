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
        if(!active) cam.Priority = 11;
        else cam.Priority = 1;
        active = !active;
    }
}
