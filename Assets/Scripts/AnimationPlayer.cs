using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPlayer : MonoBehaviour
{
    Animation anim;
    void Start()
    {
        anim = GetComponent<Animation>();
    }
    public void PlayAnimation()
    {
        anim.Play();
    }
}
