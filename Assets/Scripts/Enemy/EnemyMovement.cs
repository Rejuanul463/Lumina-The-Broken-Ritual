using UnityEngine;

public class EnemyMovement : EnemyStates
{
    [Header("Movement")]
    public float walkSpeed = 2f;
    public float runSpeed = 4f;

    [Range(0f, 1f)]
    public float walkStart = 0.3f;

    private bool startTick;

    public override void Enter()
    {
        agent.isStopped = false;

        animator.ResetTrigger("Idle");
        animator.SetTrigger("Move");

        startTick = true;
    }

    public override void ObjectTick()
    {
        if (!startTick)
            return;
        
        UpdateStateData();

        // Chase the player
        if (playerTransform != null && playerDistance <= detectionRange)
        {
            stateMachine.currentTarget = playerTransform.position;

            agent.speed = playerDistance <= detectionRange * walkStart
                ? walkSpeed
                : runSpeed;

            animator.SetFloat("Speed",
                playerDistance <= detectionRange * walkStart
                    ? walkStart
                    : 1f);

            agent.stoppingDistance = stateMachine.attackDistance;
            agent.SetDestination(stateMachine.currentTarget);

            // Switch to combat
            if (!agent.pathPending &&
                playerDistance <= agent.stoppingDistance)
            {
                stateMachine.ChangeState(stateMachine.combatState);
                return;
            }
        }
        // Patrol
        else
        {
            agent.speed = walkSpeed;
            agent.stoppingDistance = 0.1f;

            animator.SetFloat("Speed", walkStart);

            // currentTarget should already contain the waypoint position
            agent.SetDestination(stateMachine.currentTarget);

            // Arrived at patrol point
            if (!agent.pathPending &&
                agent.remainingDistance <= agent.stoppingDistance)
            {
                stateMachine.ChangeState(stateMachine.idleState);
                Debug.Log(stateMachine.idleState);
            }
        }
    }

    public override void Exit()
    {
        agent.ResetPath();
        startTick = false;
    }
}