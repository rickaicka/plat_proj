using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;

    private float rotSpeed = 60;

    public  float rotation;

    public float gravity;
    
    private Vector3 valueMove;

    private Vector3 moveDirection;
    
    CharacterController controller;
    
    Animator animator;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        GetJumpInput();
    }

    void Move()
    {
        if (controller.isGrounded)
        {

            if (!animator.GetBool("isJumpUp"))
            {
                if (Input.GetKey(KeyCode.W))
                {
                    animator.SetInteger("transition", 1);
                    animator.SetBool("isRunning", true);
                    moveDirection = Vector3.forward * speed;
                    moveDirection = transform.TransformDirection(moveDirection);
                }
            
                if (Input.GetKey(KeyCode.S))
                {
                    animator.SetInteger("transition", 3);
                    animator.SetBool("isRunningBack", true);
                    moveDirection = Vector3.back * speed;
                    moveDirection = transform.TransformDirection(moveDirection);
                }
            
                if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
                {
                    StartCoroutine(StopWalk());
                }
            }
        }
        
        rotation += Input.GetAxis("Horizontal") * rotSpeed * Time.deltaTime;
        transform.eulerAngles = new Vector3(0, rotation, 0);
        
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }

    void GetJumpInput()
    {
        if (controller.isGrounded)
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                StartCoroutine(JumpUp());
            }
            
        }
    }
    
    IEnumerator StopWalk()
    {
        yield return new WaitForSeconds(.2f);
        animator.SetInteger("transition", 0);
        animator.SetBool("isRunning", false);
        animator.SetBool("isRunningBack", false);
        moveDirection = Vector3.zero * speed;
        moveDirection = transform.TransformDirection(moveDirection);
    }

    IEnumerator StopJump()
    {
        yield return new WaitForSeconds(.85f);
        animator.SetInteger("transition", 0);
        animator.SetBool("isJumpUp", false);
        moveDirection = Vector3.zero * speed;
        moveDirection = transform.TransformDirection(moveDirection);
    }

    IEnumerator JumpUp()
    {
        yield return new WaitForSeconds(.05f);
        animator.SetInteger("transition", 2);
        animator.SetBool("isJumpUp", true);
        moveDirection.y = 30;
        moveDirection = Vector3.forward * (speed / 2);
        moveDirection = transform.TransformDirection(moveDirection);
        controller.Move(moveDirection * Time.deltaTime);
        rotation += Input.GetAxis("Horizontal") * rotSpeed * Time.deltaTime;
        transform.eulerAngles = new Vector3(0, rotation, 0);
        StartCoroutine(StopJump());
    }
}
