using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MechMovement : PlayerController
{
    Animator animator;
    public GameObject crosshairPlacement;
    public Transform cam;

    public GameObject player;
    public float speed = 6f;
    public TMPro.TextMeshProUGUI ammoText;
    Gun mechGun;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public bool overriden;
    public Cinemachine.CinemachineVirtualCamera cameraVirtual;

    protected override void Start()
    {
        mechGun = GetComponent<Gun>();
        overriden = false;
        controller = GetComponent<CharacterController>();
        velocity.y = 0;
        animator = GetComponent<Animator>();
        cameraVirtual.gameObject.SetActive(false);
    }

    protected override void Update()
    {
        if(overriden)
        {
            if(Input.GetKeyDown(KeyCode.F))
            {
                overriden = false;
                player.SetActive(true);
                player.transform.position = transform.position;
            }
            ammoText.text = mechGun.AmmoText();
            cameraVirtual.gameObject.SetActive(true);
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
                    //targetAngle = cam.eulerAngles.y;
                    //angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
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
}
