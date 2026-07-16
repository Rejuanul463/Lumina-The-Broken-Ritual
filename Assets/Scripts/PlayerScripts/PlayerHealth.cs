using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float MaxHealth = 100f;
    public float currentHealth;
    private Animator animator;
    public GameObject weapon;
    public PlayerCombat combat;
    public void Start()
    {
        combat = GetComponent<PlayerCombat>();
        currentHealth = MaxHealth;
        animator = GetComponent<Animator>();
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
        Debug.Log(damage);
        currentHealth -= damage;
        animator.SetTrigger("Damage");
        if (currentHealth <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
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
