using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : MonoBehaviour
{
    // Start is called before the first frame update
    Animation animation;
    CharacterController controller;
    public Vector3 velocity;

    void Awake()
    {
        //animation = GetComponent<Animation>();    
        //controller = GetComponent<CharacterController>();
    }

    void Start()
    {
        velocity.y = 0;
        //animation.Play("walkforward");
        //StartCoroutine(playAnimation());
        //animation.Play("revive");
    }

    IEnumerator playAnimation()
    {
        yield return new WaitForSeconds(3);
    }

    // Update is called once per frame
    void Update()
    {
        //if (controller.isGrounded && velocity.y < 0)
        //{
        //    velocity.y = 0f;
        //}

        //velocity.y -= 9.81f * Time.deltaTime;
        //controller.Move(velocity * Time.deltaTime);
    }
}
