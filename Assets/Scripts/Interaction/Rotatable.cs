using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotatable : MonoBehaviour ,IInteractable
{
    [SerializeField] Vector2 minMaxAngle = new Vector2(45, -45);
    [SerializeField] float rotationSpeed = 5f;
    [SerializeField] bool active = false;
    [SerializeField] string interactionText = "Press \"E\" To Interact.";

    float currentAngle;
    float targetAngle; 
    Vector2 calculatedMinMaxAngles;

    

    // Start is called before the first frame update
    void Start()
    {
        currentAngle = transform.localEulerAngles.y;
        calculatedMinMaxAngles.x = currentAngle + minMaxAngle.x;
        calculatedMinMaxAngles.y = currentAngle + minMaxAngle.y;
        targetAngle = currentAngle; 
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            if (Input.GetKey(KeyCode.E))
            {
                targetAngle += rotationSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.Q))
            {
                targetAngle -= rotationSpeed * Time.deltaTime;
            }

            // Clamp the targetAngle to the specified limits
            targetAngle = Mathf.Clamp(targetAngle, calculatedMinMaxAngles.y, calculatedMinMaxAngles.x);

            // Smoothly rotate towards the targetAngle
            currentAngle = Mathf.Lerp(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, currentAngle, transform.localEulerAngles.z);
        }
    }

    public void SetAsActiveRotatable(bool toggle)
    {
        active = toggle;
    }
    
    public void Use()
    {

    }
}
