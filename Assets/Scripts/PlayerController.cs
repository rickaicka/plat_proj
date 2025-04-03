using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    
    private float speed = 5;
    public  float rotation;
    private float gravity = 300;
    public float colliderRadius;
    public float playerDamage = 25f;
    private float rotSpeed = 60;
    public float totalHealth = 100f;
    public float currentHealth;
    
    private Boolean isReady;
    private List<Transform> enemiesList = new List<Transform>();
    private Vector3 valueMove;
    private Vector3 moveDirection;
    CharacterController controller;
    Animator animator;
    Rigidbody rb;
    //Drawn Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward , colliderRadius);
    }
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        currentHealth = totalHealth;
    }
    void Update()
    {
        if (!animator.GetBool("isDead"))
        {
            Move();
            GetJumpInput();
            GetMouseInput();
        }
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
    void GetMouseInput()
    {
        if (Input.GetMouseButton(0))
        {
            if(animator.GetBool("isJumpUp"))
            {
                StartCoroutine(StopJump());
            }

            if (animator.GetBool("isRunningBack") || animator.GetBool("isRunning"))
            {
                StartCoroutine(StopWalk());
                StartCoroutine(Attack());
            }
            else
            {
                StartCoroutine(Attack());
            }
        }
    }
    void GetEnemiesRange()
    {
        enemiesList.Clear();
        foreach (Collider collider in Physics.OverlapSphere(transform.position + transform.forward * colliderRadius, colliderRadius))
        {
            if (collider.gameObject.CompareTag("Enemy"))
            {
                enemiesList.Add(collider.transform);
            }
        }
    }
    public void PlayerGetHit(float damage)
    {
        currentHealth -= damage;
        if (currentHealth > 0)
        {
            animator.SetInteger("transition", 6);
            StartCoroutine(RecoveryFromHit());
        }
        else
        {
            Die();
        }
    }
    public void Die()
    {
        if (currentHealth <= 0)
        {
            animator.SetInteger("transition", 7);
            animator.SetBool("isDead", true);
            StartCoroutine(DiedCharacter());
        }
    }

    public bool CheckPlayerIsDead()
    {
        return animator.GetBool("isDead");
    }
    IEnumerator JumpUp()
    {
        speed = 2;
        var dividedSpeed = ((speed / 2) / 2);
        
        yield return new WaitForSeconds(.05f);
        animator.SetInteger("transition", 2);
        animator.SetBool("isJumpUp", true);
        moveDirection = (Vector3.up * 6) + (Vector3.forward * dividedSpeed);
        moveDirection = transform.TransformDirection(moveDirection);
        controller.Move(moveDirection * Time.deltaTime);
        rotation += Input.GetAxis("Horizontal") * rotSpeed * Time.deltaTime;
        transform.eulerAngles = new Vector3(0, rotation, 0);
        StartCoroutine(StopJump());
    }
    IEnumerator Attack()
    {
        if (!isReady)
        {
            isReady = true;
            animator.SetBool("isAttacking", true);
            animator.SetInteger("transition", 4);
            yield return new WaitForSeconds(.5f);

            GetEnemiesRange();
            
            foreach (Transform enemies in enemiesList)
            {
                EnemyController enemyC = enemies.GetComponent<EnemyController>();
                if (enemyC != null)
                {
                    enemyC.EnemyGetHit(playerDamage);
                }
            }
            
            yield return new WaitForSeconds(1f);
            animator.SetBool("isAttacking", false);
            animator.SetBool("isAttackingIdle", true);
            animator.SetInteger("transition", 5);
            isReady = false;
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
        speed = 5;
    }
    IEnumerator RecoveryFromHit()
    {
        yield return new WaitForSeconds(1.7f);
        animator.SetInteger("transition", 5);
    }
    IEnumerator DiedCharacter()
    {
        yield return new WaitForSeconds(2.5f);
        //Destroy(gameObject);
        Debug.Log("Morreu");
    }
}
