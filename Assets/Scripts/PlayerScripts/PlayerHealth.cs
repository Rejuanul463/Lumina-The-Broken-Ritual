using UnityEngine;
using UnityEngine.UI;
public class PlayerHealth : MonoBehaviour
{
    public float MaxHealth = 100f;
    public float currentHealth;
    private Animator animator;
    public GameObject weapon;
    public PlayerCombat combat;
    
    public Slider healthBar;
    public void Start()
    {
        combat = GetComponent<PlayerCombat>();
        currentHealth = MaxHealth;
        animator = GetComponent<Animator>();
        healthBar.maxValue = MaxHealth;
        healthBar.value = currentHealth;
    }
    
    
    void OnTriggerEnter(Collider collider)
    {
        if (combat.isParrying) return;
        if (currentHealth < 0)
        {
            return; 
        }

        float damage = 10;
        if (collider.tag == "EnemyHeavyAttack")
        {
            damage = 20;
        }else if (collider.tag == "EnemySword")
        {
            damage = 20;
        }
        else
        {
            return;
        }
        currentHealth -= damage;
        healthBar.value = currentHealth;
        animator.SetTrigger("Damage");
        if (currentHealth <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        gameObject.tag = "Untagged";
        gameObject.layer = LayerMask.NameToLayer("Default");
        animator.SetBool("Death", true);
        if (GetComponent<EnemyStateMachine>() != null)
        {
            GetComponent<EnemyStateMachine>().enabled = false;
            weapon.SetActive(false);
            return;
        }
        GetComponent<PlayerController>().enabled = false;
        weapon.SetActive(false);
    }
}
