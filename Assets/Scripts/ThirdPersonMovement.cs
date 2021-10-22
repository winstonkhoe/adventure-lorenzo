using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : PlayerController
{
    Animator animator;
    Player player;
    public GameObject crosshairPlacement;
    public Transform cam;
    Camera mainCamera;
    public float speed = 6f;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    protected override void Start()
    {
        controller = GetComponent<CharacterController>();
        velocity.y = 0;
        animator = GetComponent<Animator>();
        player = GetComponent<Player>();
        mainCamera = Camera.main;
    }

    private void FixedUpdate()
    {
        float yawCamera = mainCamera.transform.rotation.eulerAngles.y;
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(Input.GetAxisRaw("Horizontal"), yawCamera, Input.GetAxisRaw("Vertical")), turnSmoothTime * Time.fixedDeltaTime);
        
        //var mouseX = Input.GetAxis("Mouse X");
        
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
            //float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            if (!player.onShootingMode)
            {
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
            }
            else
            {
                //angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                var mouseX = Input.GetAxis("Mouse X");
                //targetAngle = mouseX;
                //targetAngle = cam.eulerAngles.y;
                transform.Rotate(new Vector3(0, mouseX, 0));
            }
            //transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDirection.normalized * speed * Time.deltaTime);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }
}
