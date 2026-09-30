using System.Collections;
using UnityEngine;

/// <summary>
/// Player combat logic. Called every frame by PlayerController while the player is in
/// PlayerState.CombatState (via CombateTick).
/// Handles: parrying (block), the 3-step slash combo counter, and jump/gravity while in combat
/// (the actual sword damage is done by trigger colliders, not here).
/// The combo itself is played by the Animator using the "Slash" integer parameter.
/// </summary>
public class PlayerCombat : MonoBehaviour
{
    [Header("Components")]
    private PlayerController playerController;

    [Header("Movement Parameters")]
    // Set when a jump should be applied on the next grounded check
    private bool jumpRequested;
    // Current up/down speed (positive = going up). Gravity reduces it every frame.
    private float verticalVelocity;

    [Header("Jump Parameters")]
    [SerializeField] private bool isGrounded;
    // Layers that count as ground
    [SerializeField] private LayerMask groundMask;
    // Empty child object at the player's feet used for the ground check
    [SerializeField] private Transform feet;
    [SerializeField] private float gravity = 20f;
    [SerializeField] private float jumpHeight = 3f;


    [Header("Combat Parameters")] [SerializeField]
    // Current combo step (0 = not attacking, 1..2 = combo hits). Sent to the Animator as "Slash".
    private int slashState;
    // True while the parry/block button is held
    public bool isParrying;
    // Earliest Time.time at which a new combo may start (after the combo ends)
    private float NextAttackTime;
    // Extra delay added by increaseNextAttackTime() (called from animation events on hit)
    [SerializeField] private float hitCooldown = 0.2f;
    // Cooldown applied after the final combo hit
    [SerializeField] private float attackCooldown = 2f;

    private void Start()
    {
        // Time.timeScale = .4f;
        playerController = GetComponent<PlayerController>();
    }

    /// <summary>
    /// Runs once per frame while in combat state. Handles parry, gravity/jump and combo progression.
    /// Movement while attacking is handled separately by PlayerMovement (called by PlayerController).
    /// </summary>
    public void CombateTick(ref CharacterController controller, ref Animator animator, ref PlayerInputHandler inputHandler)
    {
        // Parry: hold to block; on release, stop blocking and reset the combo
        if (inputHandler.parry)
        {
            isParrying = true;
            animator.SetBool("Parrying", true);
        }
        else if(isParrying)
        {
            isParrying = false;
            animator.SetBool("Parrying", false);
            ResetAttack();
        }

        VerticalMovement(ref controller, ref animator, ref inputHandler);
        isGrounded = Physics.CheckSphere(feet.position, 0.1f, groundMask);
        if (isGrounded)
        {
            // if(inputHandler.sprint) animator.SetBool("Shifted", true);
            // else animator.SetBool("Shifted", false);
            // Attack pressed and cooldown finished:
            //  - advance the combo (0 -> 1 -> 2)
            //  - once the combo is at its last step, start the cooldown before it can be used again
            if (inputHandler.attack && Time.time > NextAttackTime)
            {
                if (slashState < 2) slashState += 1;
                else NextAttackTime = Time.time + attackCooldown;
            }
        }
        // Tell the Animator which combo step to play
        animator.SetInteger("Slash", slashState);
        animator.SetFloat("Speed", 0f);
    }

    /// <summary>
    /// Applies gravity and jumping in combat state (vertical motion only; horizontal motion comes
    /// from PlayerMovement). Same jump formula as PlayerMovement: v = sqrt(2 * g * h).
    /// </summary>
    private void VerticalMovement(ref CharacterController controller, ref Animator animator,
        ref PlayerInputHandler inputHandler)
    {
        Vector3 direction = inputHandler.moveDirection;
        isGrounded = Physics.CheckSphere(feet.position, 0.1f, groundMask);
        if (isGrounded)
        {
            // Keep the controller grounded
            if (verticalVelocity < 0f)
                verticalVelocity = -2f;

            if (inputHandler.jump && playerController.doMove)
            {
                jumpRequested = true;
            }
            // Apply jump AFTER movement direction is calculated
            if (jumpRequested)
            {
                animator.SetTrigger("Jump");
                verticalVelocity = Mathf.Sqrt(jumpHeight * 2f * gravity);
                jumpRequested = false;
            }
        }
        else
        {
            // In the air: gravity pulls the player down
            verticalVelocity -= gravity * Time.deltaTime;
        }
        animator.SetBool("isGround", isGrounded);
        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }

    /// <summary>
    /// Ends the combo and returns the player to NormalState.
    /// Also called from an animation event at the end of the attack animation.
    /// </summary>
    public void ResetAttack()
    {
        slashState = 0;
        playerController.currentState = PlayerState.NormalState;
    }

    // Animation Event: pushes the next allowed attack time back slightly (hitCooldown)
    public void increaseNextAttackTime()
    {
        NextAttackTime += hitCooldown;
    }
}
