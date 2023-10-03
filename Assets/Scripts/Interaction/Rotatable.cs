using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotatable : Interactable
{
    [SerializeField] Transform rotatableObject;
    [SerializeField] Vector2 minMaxAngle = new Vector2(45, -45);
    [SerializeField] float rotationSpeed = 5f;
    [SerializeField] bool active = false;
    [SerializeField] bool startWithRandomRotation = false;
    [SerializeField] Transform secondCameraTransform;
    float currentAngle;
    float targetAngle;
    Vector2 calculatedMinMaxAngles;

    // Start is called before the first frame update
    void Start()
    {
        if(!rotatableObject) rotatableObject = transform;
        if (secondCameraTransform == null) secondCameraTransform = rotatableObject;
        currentAngle = rotatableObject.localEulerAngles.y;
        targetAngle = currentAngle;
        calculatedMinMaxAngles.x = currentAngle + minMaxAngle.x;
        calculatedMinMaxAngles.y = currentAngle + minMaxAngle.y;

        if (startWithRandomRotation)
        {
            float rndRotation = Random.Range(calculatedMinMaxAngles.x, calculatedMinMaxAngles.y);
            rotatableObject.localRotation = Quaternion.Euler(rotatableObject.localEulerAngles.x, rndRotation,
            rotatableObject.localEulerAngles.z);
            currentAngle = rndRotation;
            targetAngle = rndRotation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            if (Input.GetKey(KeyCode.D))
            {
                targetAngle += rotationSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.A))
            {
                targetAngle -= rotationSpeed * Time.deltaTime;
            }

            // Clamp the targetAngle to the specified limits
            targetAngle = Mathf.Clamp(targetAngle, calculatedMinMaxAngles.y, calculatedMinMaxAngles.x);

            // Smoothly rotate towards the targetAngle
            currentAngle = Mathf.Lerp(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
            rotatableObject.localRotation = Quaternion.Euler(rotatableObject.localEulerAngles.x, currentAngle, rotatableObject.localEulerAngles.z);
        }
    }

    public void SetAsActiveRotatable(bool toggle)
    {
        active = toggle;
    }

    public override void Use()
    {
        Interactor interactor = FindObjectOfType<Interactor>();
        if(!active)
        {
            interactor.ToggleCameras();
            interactor.TogglePlayerMovement();
            interactor.SetSecondCamPosition(secondCameraTransform.position, secondCameraTransform.rotation, transform);
            active = true;
        }
        else
        {
            interactor.ToggleCameras();
            interactor.TogglePlayerMovement();
            active = false;
        }
    }

}
