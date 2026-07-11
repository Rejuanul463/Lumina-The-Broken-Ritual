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
        if (collider.tag != "EnemySword")
        {
            return;
        }
        currentHealth -= 10f;
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
