using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// A companion NPC that starts locked in a cage (see OpenCage).
/// Life cycle:
///  1. Caged (isFree == false): stands still, its EnemyStateMachine is disabled, and it is
///     untagged/unlayered so enemies ignore it.
///  2. Freed (isFree == true, set by OpenCage): walks out of the cage to the player; once it gets
///     close, the companion info UI is shown once (UiShowed).
///  3. Following: follows the player. It takes the player's layer and tag so enemies target it too.
///     When an enemy is nearby, the EnemyStateMachine is enabled so the NPC fights on its own;
///     when no enemy is near it is disabled again and the NPC just follows the player.
/// Requires on the same GameObject: EnemyStateMachine, Animator, NavMeshAgent.
/// </summary>
public class TeamNPC : MonoBehaviour
{
    // The player to follow
    public Transform player;
    private EnemyStateMachine statemachine;
    // True while the combat AI (state machine) is running instead of the follow behaviour
    private bool stateMachineActivated;
    // Set to true by OpenCage when the cage is opened
    public bool isFree = false;
    public Animator anim;
    private NavMeshAgent agent;
    // Radius in which enemies make the NPC switch to its combat AI
    private float detectionDistance = 10f;
    // Layer to use while following the player (must be a single layer; see FollowPlayer)
    [SerializeField]
    private LayerMask playerLayer;

    // Layers of the enemies the NPC should fight
    [SerializeField] private LayerMask enemyLayer;
    // Tag to apply to the NPC once freed (e.g. "Player") so enemies treat it as a target
    [SerializeField] private string playerTag;

    // The cage this NPC belongs to (used to show the companion UI)
    public OpenCage cage;
    // Ensures the companion UI is only shown once
    private bool isUIShowed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        statemachine = GetComponent<EnemyStateMachine>();
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = 1f;

        // Combat AI stays off until an enemy is close
        statemachine.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFree) agent.stoppingDistance = 1.5f;

        // Freed but UI not yet shown: wait until the NPC has walked to its destination, then show it.
        // Nothing else (follow / fight) happens until that has happened.
        if(!isUIShowed && isFree)
        {
            if(agent.remainingDistance <= agent.stoppingDistance)
                UiShowed();

            return;
        }

        // Check every frame whether the combat AI should be on or off
        if(isFree) FindClosestEnemy();

        // While the combat AI is active it controls the NPC, so skip follow logic
        if (stateMachineActivated) return;

        if (!isFree)
        {
            // Still caged: make it invisible to enemies and stand idle
            gameObject.layer = LayerMask.NameToLayer("Default");
            gameObject.tag = "Untagged";
            anim.SetFloat("Speed", 0);
        }
        else
        {
            FollowPlayer();
        }
    }

    /// <summary>
    /// Follows the player: adopts the player's layer/tag (so enemies target the NPC), walks toward
    /// the player and plays the run animation while it is farther than the stopping distance.
    /// </summary>
    void FollowPlayer()
    {
        agent.speed = 3.8f;
        // LayerMask holds a bit mask, so log2 converts it back to a layer index
        gameObject.layer = Mathf.RoundToInt(Mathf.Log(playerLayer.value, 2));
        gameObject.tag = playerTag;
        agent.SetDestination(player.transform.position);
        if(agent.remainingDistance > agent.stoppingDistance) anim.SetFloat("Speed", 1f);
        else anim.SetFloat("Speed", 0);
    }


    /// <summary>
    /// Enables the combat AI (EnemyStateMachine) when any enemy is within detectionDistance,
    /// and disables it again when there are none.
    /// </summary>
    private void FindClosestEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            detectionDistance,
            enemyLayer);

        if (colliders.Length > 0)
        {
            stateMachineActivated = true;
            statemachine.enabled = true;
        }
        else
        {
            stateMachineActivated = false;
            statemachine.enabled = false;
        }
    }

    // Shows the companion info UI (through the cage) exactly once and sets a closer stopping distance
    public void UiShowed()
    {
        isUIShowed = true;
        agent.stoppingDistance = 2f;
        cage.ShowUI();
    }
}
