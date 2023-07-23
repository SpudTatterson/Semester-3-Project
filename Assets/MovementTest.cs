using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTest : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] float walkSpeed = 6;

    float horizontalAxis;
    float verticalAxis;

    // Start is called before the first frame update
    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }
    // Update is called once per frame
    void Update()
    {
        GetInputs();
        Move();   
    }

    void GetInputs()
    {
        horizontalAxis = Input.GetAxisRaw("Horizontal");
        verticalAxis = Input.GetAxisRaw("Vertical");
        
    }

    void Move()
    {
        Vector3 moveDir = new Vector3(horizontalAxis, 0, verticalAxis);

        controller.Move(moveDir * walkSpeed * Time.deltaTime);
    }
}
