using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Health and death handling for enemies (and creatures).
/// Damage is taken through trigger collisions: any collider whose tag equals targetTag
/// (e.g. the player's sword) deals a fixed 10 damage.
/// Requires an Animator on the same GameObject (parameters: Damage trigger, Death bool).
/// </summary>
public class EnemyHealth : MonoBehaviour
{
    public float MaxHealth = 100f;
    public float currentHealth;
    private Animator animator;
    private EnmeyCombate enemyCombate;
    // The enemy's weapon object; disabled on death so a dead enemy can't hurt anyone
    public GameObject weapon;
    public bool isDied;

    // Tag of the colliders that can damage this enemy (e.g. the player's weapon tag)
    public String targetTag;

    // UI slider shown above the enemy
    public Slider healthBar;
    public void Start()
    {
        currentHealth = MaxHealth;
        animator = GetComponent<Animator>();
        enemyCombate = GetComponent<EnmeyCombate>();

        healthBar.maxValue = MaxHealth;
        healthBar.value = currentHealth;
    }


    // Called by Unity when another trigger collider overlaps this enemy
    void OnTriggerEnter(Collider collider)
    {
        // Ignore hits if health is already below zero
        if (currentHealth < 0)  return;
        // Only react to the configured damage source
        if (collider.tag != targetTag)  return;

        currentHealth -= 10f;
        healthBar.value = currentHealth;
        // Play the hit reaction only while alive
        if(!isDied)
            animator.SetTrigger("Damage");
        // enemyCombate.increaseNextAttackTime();
        if (currentHealth <= 0)
            Death();
    }

    /// <summary>
    /// Plays the death animation and turns the enemy "off": it is moved to the Default layer and
    /// untagged so nothing targets it any more, its AI is disabled, and weapon/health bar are hidden.
    /// </summary>
    private void Death()
    {
        isDied  = true;
        animator.SetBool("Death", true);

        gameObject.layer = LayerMask.NameToLayer("Default");
        gameObject.tag = "Untagged";

        // Disable whichever AI this object uses (regular enemy state machine OR arena creature)
        if ( GetComponent<EnemyStateMachine>() != null)
            GetComponent<EnemyStateMachine>().enabled = false;

        else if (GetComponent<CreaturesAi>() != null)
            GetComponent<CreaturesAi>().enabled = false;

        weapon.SetActive(false);
        healthBar.gameObject.SetActive(false);
    }
}
