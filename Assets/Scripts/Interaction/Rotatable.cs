using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotatable : MonoBehaviour
{
    [SerializeField] Vector2 minMaxAngle = new Vector2(45, -45);
    [SerializeField] float rotationSpeed = 5f;
    [SerializeField] bool active = false;

    float currentAngle;
    // Start is called before the first frame update
    void Start()
    {
        currentAngle = transform.rotation.y;
    }

    // Update is called once per frame
    void Update()
    {
        if(active)
        {
            if(Input.GetKey(KeyCode.E))
            {
                RotateRight(); 
            }
            if(Input.GetKey(KeyCode.Q))
            {
                RotateLeft();
            }
        }
    }
     void RotateRight()
    {
        currentAngle += rotationSpeed * Time.deltaTime;
        currentAngle = Mathf.Clamp(currentAngle, minMaxAngle.y, minMaxAngle.x);
        transform.localRotation = Quaternion.Euler(0f, currentAngle, 0f);
    }

    void RotateLeft()
    {
        currentAngle -= rotationSpeed * Time.deltaTime;
        currentAngle = Mathf.Clamp(currentAngle, minMaxAngle.y, minMaxAngle.x);
        transform.localRotation = Quaternion.Euler(0f, currentAngle, 0f);
    }
}
