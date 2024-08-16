using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 2f;

    public float gravity = -9.81f;

    public Vector3 RaycastOffset = Vector3.zero;

    Vector3 velocity;

    // Update is called once per frame
    void Update()
    {
        if (controller.enabled == true)
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;
            controller.Move(move * Time.deltaTime * speed);

            if (!Physics.Raycast(transform.position + RaycastOffset, Vector3.down, 2f, -1))
            {
                velocity.y += gravity * Time.deltaTime;
            }
            else
            {
                velocity.y = -2f;
            }
            controller.Move(velocity * Time.deltaTime);
        }
    }
}
