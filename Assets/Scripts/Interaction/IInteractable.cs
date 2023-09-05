using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    
    [SerializeField] Transform interactionPoint{get; set;}
    [SerializeField] float interactionDistance {get; set;}
    [SerializeField] string interactionText {get; set;}
    public void Use();
}
