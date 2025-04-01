using System;
using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float totalHealth = 100f;
    public float currentHealth;
    public float movementSpeed = 5f;
    public float rotationSpeed = 5f;
    public new string name;
    
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void GetHit()
    {
        animator.SetInteger("transition", 4);
        StartCoroutine(RecoveryFromHit());
        //Destroy(gameObject);
    }
    
    IEnumerator RecoveryFromHit()
    {
        yield return new WaitForSeconds(1.2f);
        animator.SetInteger("transition", 0);
    }
}
