using UnityEngine;

/// <summary>
/// Enemy state: moving. It has two behaviours:
///  1. Chase  - if a player is within detection range, run/walk toward them and switch to
///              combat once inside attack distance.
///  2. Patrol - otherwise walk to the patrol point chosen by EnemyIdle, then go back to idle.
/// </summary>
public class EnemyMovement : EnemyStates
{
    [Header("Movement")]
    public float walkSpeed = 2f;
    public float runSpeed = 4f;

    // Fraction (0-1) of the detection range. Inside this fraction the enemy walks, beyond it runs.
    // Also used as the "Speed" animator value while walking.
    [Range(0f, 1f)]
    public float walkStart = 0.3f;

    // Guards ObjectTick so it only runs between Enter() and Exit()
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

            // Walk when close to the player, run when far away
            agent.speed = playerDistance <= detectionRange * walkStart
                ? walkSpeed
                : runSpeed;

            // Matching value for the walk/run blend tree (walkStart = walk, 1 = run)
            animator.SetFloat("Speed",
                playerDistance <= detectionRange * walkStart
                    ? walkStart
                    : 1f);

            // Stop at attack distance so the enemy doesn't walk into the player
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
            // Small stopping distance so the enemy actually reaches the patrol point
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
        // Clear the current path so the agent doesn't keep sliding toward the old destination
        agent.ResetPath();
        startTick = false;
    }
}
