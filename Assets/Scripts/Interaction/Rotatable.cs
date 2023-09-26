using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotatable : Interactable
{
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
        if (secondCameraTransform == null) secondCameraTransform = transform;
        currentAngle = transform.localEulerAngles.y;
        targetAngle = currentAngle;
        calculatedMinMaxAngles.x = currentAngle + minMaxAngle.x;
        calculatedMinMaxAngles.y = currentAngle + minMaxAngle.y;

        if (startWithRandomRotation)
        {
            float rndRotation = Random.Range(calculatedMinMaxAngles.x, calculatedMinMaxAngles.y);
            transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, rndRotation,
            transform.localEulerAngles.z);
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
            transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, currentAngle, transform.localEulerAngles.z);
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
