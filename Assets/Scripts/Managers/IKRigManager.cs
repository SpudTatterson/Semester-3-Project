
using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;
public class IKRigManager : MonoBehaviour
{
    public  MultiAimConstraint headAimRig;
    public TwoBoneIKConstraint leftHandRig;
    public TwoBoneIKConstraint rightHandRig;
    public TwoBoneIKConstraint leftLegRig;
    public TwoBoneIKConstraint rightLegRig;
    public TwistChainConstraint spineRig;
    public MultiParentConstraint hipRig;


    void Start()
    {
        headAimRig.weight = 0;
        leftHandRig.weight = 0;
        rightHandRig.weight = 0;
        leftLegRig.weight = 0;
        rightLegRig.weight = 0;
        spineRig.weight = 0;
        hipRig.weight = 0;
    }
    public static IEnumerator SetRigWeight(TwoBoneIKConstraint rig, float targetWeight, float lerpTime)
    {
        float startTime = Time.time;
        float currentWeight = rig.weight;
        Debug.Log("test");
        while (Time.time - startTime < lerpTime)
        {
            float normalizedTime = (Time.time - startTime) / lerpTime;
            rig.weight = Mathf.Lerp(currentWeight, targetWeight, normalizedTime);
            yield return null;
        }

        // Ensure the weight is set to the target value at the end.
        rig.weight = targetWeight;
    }
    public static IEnumerator SetRigWeight(MultiAimConstraint rig, float targetWeight, float lerpTime)
    {
        float startTime = Time.time;
        float currentWeight = rig.weight;

        while (Time.time - startTime < lerpTime)
        {
            float normalizedTime = (Time.time - startTime) / lerpTime;
            rig.weight = Mathf.Lerp(currentWeight, targetWeight, normalizedTime);
            yield return null;
        }

        // Ensure the weight is set to the target value at the end.
        rig.weight = targetWeight;
    }
    public static IEnumerator SetRigWeight(TwistChainConstraint rig, float targetWeight, float lerpTime)
    {
        float startTime = Time.time;
        float currentWeight = rig.weight;

        while (Time.time - startTime < lerpTime)
        {
            float normalizedTime = (Time.time - startTime) / lerpTime;
            rig.weight = Mathf.Lerp(currentWeight, targetWeight, normalizedTime);
            yield return null;
        }

        // Ensure the weight is set to the target value at the end.
        rig.weight = targetWeight;
    }
    public static IEnumerator SetRigWeight(MultiParentConstraint rig, float targetWeight, float lerpTime)
    {
        float startTime = Time.time;
        float currentWeight = rig.weight;

        while (Time.time - startTime < lerpTime)
        {
            float normalizedTime = (Time.time - startTime) / lerpTime;
            rig.weight = Mathf.Lerp(currentWeight, targetWeight, normalizedTime);
            yield return null;
        }

        // Ensure the weight is set to the target value at the end.
        rig.weight = targetWeight;
    }
}
