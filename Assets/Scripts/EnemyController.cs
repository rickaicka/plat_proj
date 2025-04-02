using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float totalHealth = 100f;
    public float currentHealth;
    public float movementSpeed = 5f;
    public float rotationSpeed = 5f;
    public new string name;
    public float lookRadius = 10f;
    public Transform target;

    private bool isReady;
    private Animator animator;
    private CapsuleCollider capsuleCollider;
    private NavMeshAgent agent;
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
    private void Start()
    {
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        agent = GetComponent<NavMeshAgent>();
        currentHealth = totalHealth;
    }

    private void Update()
    {
        if(!animator.GetBool("isDead"))
        {
            CheckDistance();
        }
    }

    public void CheckDistance()
    {
        
        float distance = Vector3.Distance(target.position, transform.position);
        if (distance <= lookRadius)
        {
            agent.isStopped = false;
            if(!animator.GetBool("isAttacking"))
            {
                MoveToPlayer();
            }
            
            if (distance <= agent.stoppingDistance)
            {
                StartCoroutine(Attack());
                FaceTarget();
            }
        }
        else
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", false);
            if(animator.GetBool("isDead"))
            {
                animator.SetInteger("transition", 5);
            }
            else
            {
                animator.SetInteger("transition", 0);
            }
            agent.isStopped = true;
        }
    }


    public void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    public void MoveToPlayer()
    {
        agent.SetDestination(target.position);
        animator.SetInteger("transition", 1);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isWalking", true);
    }
    public void GetHit(float damage)
    {
        currentHealth -= damage;
        if (currentHealth > 0)
        {
            animator.SetInteger("transition", 4);
            StartCoroutine(RecoveryFromHit());
        }
        else
        {
            Die();
        }
    }

    void Die()
    {
        if (currentHealth <= 0)
        {
            animator.SetInteger("transition", 5);
            animator.SetBool("isDead", true);
            StartCoroutine(DiedCharacter());
        }
    }
    
    IEnumerator RecoveryFromHit()
    {
        yield return new WaitForSeconds(1.2f);
        animator.SetInteger("transition", 0);
    }

    IEnumerator DiedCharacter()
    {
        capsuleCollider.enabled = false;
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
    
    IEnumerator Attack()
    {
        if (!isReady)
        {
            isReady = true;
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", true);
            animator.SetInteger("transition", 2);
            yield return new WaitForSeconds(2f);
            animator.SetBool("isAttacking", false);
            isReady = false;
        }
    }
    
}
