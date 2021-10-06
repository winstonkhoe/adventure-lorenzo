using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : PlayerController
{
    Animator animator;
    public GameObject crosshairPlacement;
    public Transform cam;

    public float speed = 6f;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    protected override void Start()
    {
        controller = GetComponent<CharacterController>();
        velocity.y = 0;
        animator = GetComponent<Animator>();
    }

    protected override void Update()
    {
        if(animator.GetBool("isAiming"))
        {
            transform.forward = Vector3.Lerp(transform.forward, crosshairPlacement.transform.forward, Time.deltaTime * 3f);
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = 0f;
        }

        velocity.y -= 9.81f * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (direction.magnitude >= 0.1f)
        {
            animator.SetBool("isWalking", true);
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDirection.normalized * speed * Time.deltaTime);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }
}
