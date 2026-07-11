using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyStates : MonoBehaviour
{
    protected Animator animator;
    protected NavMeshAgent agent;
    protected EnemyStateMachine stateMachine;

    protected Transform playerTransform;
    protected Transform[] targetPoints;

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
            playerDistance = Mathf.Infinity;
        }
    }

    public abstract void Enter();
    public abstract void ObjectTick();
    public abstract void Exit();
}