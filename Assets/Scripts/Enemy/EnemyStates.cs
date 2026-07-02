using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyStates : MonoBehaviour
{
    protected Animator animator;
    protected Vector3 targetPosition;
    protected Transform playerTransform;
    protected NavMeshAgent agent;
    protected EnemyStateMachine stateMachine;
    protected Transform[] targetPoints;
    protected float detectionRange;

    protected float playerDistance;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        stateMachine = GetComponent<EnemyStateMachine>();
        detectionRange = stateMachine.detectionDistance;
        agent.stoppingDistance = stateMachine.attackDistance;
        if (stateMachine != null)
        {
            playerTransform = stateMachine.Player;
            targetPoints = stateMachine.targetPoints;
        }
    }

    private void Update()
    {
        playerDistance = Vector3.Distance(transform.position, playerTransform.position);
    }

    public abstract void Enter();
    public abstract void ObjectTick();
    public abstract void Exit();
}