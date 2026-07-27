using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float MaxHealth = 100f;
    public float currentHealth;
    private Animator animator;
    private EnmeyCombate enemyCombate;
    public GameObject weapon;
    
    public String targetTag;
    
    public Slider healthBar;
    public void Start()
    {
        currentHealth = MaxHealth;
        animator = GetComponent<Animator>();
        enemyCombate = GetComponent<EnmeyCombate>();
        
        healthBar.maxValue = MaxHealth;
        healthBar.value = currentHealth;
    }
    
    
    void OnTriggerEnter(Collider collider)
    {
        if (currentHealth < 0)  return; 
        if (collider.tag != targetTag)  return;
        
        currentHealth -= 10f;
        healthBar.value = currentHealth;
        animator.SetTrigger("Damage");
        // enemyCombate.increaseNextAttackTime();
        if (currentHealth <= 0) 
            Death();
    }

    private void Death()
    {
        animator.SetBool("Death", true);
        
        gameObject.layer = LayerMask.NameToLayer("Default");
        gameObject.tag = "Untagged";
        
        if ( GetComponent<EnemyStateMachine>() != null) 
            GetComponent<EnemyStateMachine>().enabled = false;
        
        else if (GetComponent<CreaturesAi>() != null) 
            GetComponent<CreaturesAi>().enabled = false;
        
        weapon.SetActive(false);
        healthBar.gameObject.SetActive(false);
    }
}
