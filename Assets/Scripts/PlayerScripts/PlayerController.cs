using UnityEngine;

/// <summary>
/// The two modes the player can be in.
/// NormalState - free movement (walk/run/crouch/jump), handled by PlayerMovement.
/// CombatState - attacking / parrying, handled by PlayerCombat (movement is still allowed).
/// </summary>
public enum PlayerState
{
    NormalState,
    CombatState
}

/// <summary>
/// Central player script. It owns the current PlayerState and each frame decides which
/// component (PlayerMovement or PlayerCombat) runs. It also handles enemy lock-on (focus).
/// Requires on the same GameObject: CharacterController, Animator, PlayerInputHandler,
/// PlayerMovement, PlayerCombat.
/// </summary>
public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    public Animator animator;
    private PlayerInputHandler playerInput;

    // Lock-on settings: enemies within detectionDistance on enemyLayer can be targeted
    public float detectionDistance = 5f;
    public LayerMask enemyLayer;
    // The currently locked-on enemy (null when not locked on)
    public Transform Target;
    // True while locked on: the player always faces the target
    public bool isFocused = false;

    private PlayerMovement playerMovement;
    private PlayerCombat combat;

    public PlayerState currentState;
    // Set by animation events (enableDoMove / disableDoMove) to block movement/jumping
    // during certain animations, e.g. the middle of an attack.
    public bool doMove = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentState = PlayerState.NormalState;

        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInputHandler>();
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        combat = GetComponent<PlayerCombat>();
    }

    void Update()
    {
        // Tell the Animator whether the player is pushing the move input (used to layer
        // movement animations on top of attack/parry animations)
        bool isMoving = false;
        if (playerInput.moveDirection.magnitude > 0f) isMoving = true;
        else isMoving = false;
        animator.SetBool("isMovingCombate", isMoving);

        if (currentState == PlayerState.NormalState)
        {
            // Make sure no leftover combo state remains, then move normally
            combat.ResetAttack();
            animator.SetInteger("Slash", 0);
            playerMovement.MovementTick(ref controller, ref animator, ref playerInput);
        }
        else if (currentState == PlayerState.CombatState)
        {
            combat.CombateTick(ref controller, ref animator, ref playerInput);
            // The player may still walk while attacking/parrying
            if (isMoving)
                playerMovement.MovementTick(ref controller, ref animator, ref playerInput);
        }

        if (Target != null)
        {
            // While locked on, always face the target (horizontally only)
            if (isFocused)
            {
                transform.LookAt(new Vector3(Target.position.x, transform.position.y, Target.position.z));
            }
            // Target got too far away: release the lock-on
            if (Vector3.Distance(Target.position, transform.position) > detectionDistance)
            {
                Target = null;
                isFocused = false;
                animator.SetBool("Lock", false);
            }
        }
    }

    // Animation Events: allow / block movement and jumping at specific points of an animation
    public void enableDoMove() { doMove = true;}

    public void disableDoMove() { doMove = false;}

    /// <summary>
    /// Locks on to the closest enemy in range (if not already locked on).
    /// Called by PlayerInputHandler when the attack button is pressed.
    /// </summary>
    public void SelectEnemy()
    {
        if (Target == null)
        {
            // Find the closest enemy within detectionDistance
            Collider[] targets = Physics.OverlapSphere(transform.position, detectionDistance, enemyLayer );
            float minDistance = float.MaxValue;
            foreach (Collider target in targets)
            {
                float dist = Vector3.Distance(transform.position, target.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    Target = target.transform;
                }
            }
        }

        // Only enter focus mode if an enemy was actually found
        if (Target != null)
        {
            isFocused = true;
            animator.SetBool("Lock", true);
        }
    }
}
