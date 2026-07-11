using System;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float MaxHealth = 100f;
    public float currentHealth;
    private Animator animator;
    private EnmeyCombate enemyCombate;
    public GameObject weapon;
    
    public String targetTag;
    public void Start()
    {
        currentHealth = MaxHealth;
        animator = GetComponent<Animator>();
        enemyCombate = GetComponent<EnmeyCombate>();
    }
    
    
    void OnTriggerEnter(Collider collider)
    {
        if (currentHealth < 0)
        {
            return; 
        }
        if (collider.tag != targetTag)
        {
            return;
        }
        currentHealth -= 10f;
        animator.SetTrigger("Damage");
        // enemyCombate.increaseNextAttackTime();
        if (currentHealth <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        animator.SetBool("Death", true);
        GetComponent<EnemyStateMachine>().enabled = false;
        weapon.SetActive(false);
    }
}
