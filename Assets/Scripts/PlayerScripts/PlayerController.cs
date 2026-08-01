using UnityEngine;

public enum PlayerState
{
    NormalState,
    CombatState
}
public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    public Animator animator;
    private PlayerInputHandler playerInput;
    
    public float detectionDistance = 5f;
    public LayerMask enemyLayer;
    public Transform Target;
    public bool isFocused = false;
    
    private PlayerMovement playerMovement;
    private PlayerCombat combat;
    
    public PlayerState currentState;
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
        bool isMoving = false;
        if (playerInput.moveDirection.magnitude > 0f) isMoving = true;
        else isMoving = false;
        animator.SetBool("isMovingCombate", isMoving);
        
        if (currentState == PlayerState.NormalState)
        {
            combat.ResetAttack();
            animator.SetInteger("Slash", 0);
            playerMovement.MovementTick(ref controller, ref animator, ref playerInput);
        }
        else if (currentState == PlayerState.CombatState)
        {
            combat.CombateTick(ref controller, ref animator, ref playerInput);
            if (isMoving)
                playerMovement.MovementTick(ref controller, ref animator, ref playerInput);
        }

        if (Target != null)
        {
            if (isFocused)
            {
                transform.LookAt(new Vector3(Target.position.x, transform.position.y, Target.position.z));
            }
            if (Vector3.Distance(Target.position, transform.position) > detectionDistance)
            {
                Target = null;
                isFocused = false;
                animator.SetBool("Lock", false);
            }
        }
    }
    
    public void enableDoMove() { doMove = true;}

    public void disableDoMove() { doMove = false;}

    public void SelectEnemy()
    {
        if (Target == null)
        {
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

        if (Target != null)
        {
            isFocused = true;
            animator.SetBool("Lock", true);
        }
    }
}
