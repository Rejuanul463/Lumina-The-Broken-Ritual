using System;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float MaxHealth = 100f;
    public float currentHealth;
    private Animator animator;

    public void Start()
    {
        currentHealth = MaxHealth;
        animator = GetComponent<Animator>();
    }
    
    
    public void TakeDamage(float damage)
    {
            currentHealth -= damage;
            animator.SetTrigger("Damage");
            if (currentHealth <= 0)
            {
                animator.SetTrigger("Death");
            }
    }
}
