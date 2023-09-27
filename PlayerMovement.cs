using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    private float speed, gravity = -3f, groundDist = 0.4f, x, z;
    private Vector3 move, velocity;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] Transform groundCheck;
    private bool isGrounded;
    [SerializeField] private Animator currentGun;
    [SerializeField] private AudioSource jumpSF;
    [SerializeField] private float defaultSpeed = 5f, jumpHeight = 24f;

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDist, groundMask);
        if (isGrounded && velocity.y < 0)
            velocity.y = -0.5f; 

        if(Input.GetButtonDown("Jump") && isGrounded) 
        {
            jumpSF.Play();
            velocity.y = Mathf.Sqrt(jumpHeight);
        }

        speed = defaultSpeed;
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C))
        {
            gravity = -100f;
            speed /= 2f;
            GetComponent<CharacterController>().height = 0.8f;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.C))
        {
            GetComponent<CharacterController>().height = 2.3f;
            gravity = -3f;
            speed *= 2f;
        }

        if (Input.GetKey(KeyCode.LeftShift))
            speed *= 2f;
        else if (Input.GetKeyUp(KeyCode.LeftShift))
            speed /= 2f;

        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");
        move = transform.right * x + transform.forward * z;
        characterController.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);


        if ((x!=0||z!=0) && isGrounded)
        {
            currentGun.SetInteger("beg", (Input.GetKey(KeyCode.LeftShift)) ? 2 : 1);
        }
        else currentGun.SetInteger("beg", 0);
    }
}
