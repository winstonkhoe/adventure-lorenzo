using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Vector3 velocity;
    public CharacterController controller;

    protected virtual void Start()
    {

        Cursor.lockState = CursorLockMode.Locked;
    }

    protected virtual void Update()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = 0f;
        }
        velocity.y -= 9.81f * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        
    }
}
