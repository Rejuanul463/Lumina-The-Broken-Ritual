using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Health and death handling for the player (and any ally that uses the same tags).
/// Damage comes from trigger collisions with enemy weapons, identified by tag:
///   "EnemyHeavyAttack" and "EnemySword" -> 20 damage. Anything else is ignored.
/// No damage is taken while parrying.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    public float MaxHealth = 100f;
    public float currentHealth;
    private Animator animator;
    // Weapon object; disabled on death
    public GameObject weapon;
    // Used to check whether the player is currently parrying
    public PlayerCombat combat;

    // UI slider showing the health
    public Slider healthBar;
    public void Start()
    {
        combat = GetComponent<PlayerCombat>();
        currentHealth = MaxHealth;
        animator = GetComponent<Animator>();
        healthBar.maxValue = MaxHealth;
        healthBar.value = currentHealth;
    }


    // Called by Unity when an enemy weapon trigger overlaps the player
    void OnTriggerEnter(Collider collider)
    {
        // Parry blocks all damage
        if (combat.isParrying) return;
        // Already dead / below zero: ignore further hits
        if (currentHealth < 0)
        {
            return;
        }

        // Damage depends on which enemy attack hit us
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
            // Not an enemy attack (e.g. walked into some other trigger)
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

    /// <summary>
    /// Plays the death animation and stops the character from being targeted or controlled.
    /// If this object has an EnemyStateMachine (i.e. it is an AI companion) its AI is disabled,
    /// otherwise the player controller is disabled.
    /// </summary>
    private void Death()
    {
        // Untag and move to Default layer so enemies stop targeting this character
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
