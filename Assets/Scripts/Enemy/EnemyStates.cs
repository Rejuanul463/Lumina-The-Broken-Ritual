using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Base class for every enemy AI state (Idle, Movement, Combat, ...).
/// It caches the components all states need and provides shared helper data.
/// Each state must implement:
///   Enter()      - called once when the state becomes active
///   ObjectTick() - called every frame by EnemyStateMachine while the state is active
///   Exit()       - called once when leaving the state
/// To add a new state: derive from this class, add it to the enemy, and switch to it with
/// stateMachine.ChangeState(...).
/// Requires on the same GameObject: Animator, NavMeshAgent, EnemyStateMachine.
/// </summary>
public abstract class EnemyStates : MonoBehaviour
{
    protected Animator animator;
    protected NavMeshAgent agent;
    protected EnemyStateMachine stateMachine;

    // Current target (player) and patrol points, refreshed by UpdateStateData()
    protected Transform playerTransform;
    protected Transform[] targetPoints;

    // Distance to the target (Infinity if there is none) and the detection radius
    protected float playerDistance;
    protected float detectionRange;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        stateMachine = GetComponent<EnemyStateMachine>();

        detectionRange = stateMachine.detectionDistance;
        agent.stoppingDistance = stateMachine.attackDistance;

        targetPoints = stateMachine.targetPoints;
    }

    /// <summary>
    /// Refresh shared AI information.
    /// Call this at the beginning of every ObjectTick().
    /// </summary>
    protected void UpdateStateData()
    {
        playerTransform = stateMachine.Player;
        targetPoints = stateMachine.targetPoints;

        if (playerTransform != null)
        {
            playerDistance = Vector3.Distance(
                transform.position,
                playerTransform.position);

            // stateMachine.currentTarget = playerTransform.position;
        }
        else
        {
            // No target -> treat as infinitely far away
            playerDistance = Mathf.Infinity;
        }
    }

    public abstract void Enter();
    public abstract void ObjectTick();
    public abstract void Exit();
}
