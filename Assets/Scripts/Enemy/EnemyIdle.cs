using UnityEngine;

/// <summary>
/// Enemy state: standing still (Idle).
/// On entry it picks a random patrol point as the next destination.
/// It leaves this state (to EnemyMovement) when a player is detected or when idleTime has elapsed.
/// </summary>
public class EnemyIdle : EnemyStates
{
    // How many seconds the enemy waits before moving on to the next patrol point
    [SerializeField] private float idleTime = 2f;

    // Countdown timer (visible in the inspector for debugging)
    [SerializeField] private float idleTimer;
    // Guards ObjectTick so it only runs between Enter() and Exit()
    private bool startTick;

    public override void Enter()
    {
        // Stop the NavMeshAgent and clear any previous path
        agent.isStopped = true;
        agent.ResetPath();

        animator.SetTrigger("Idle");
        animator.SetFloat("Speed", 0f);
        idleTimer = idleTime;
        startTick = true;

        // Choose a random patrol point
        if (targetPoints != null && targetPoints.Length > 0)
        {
            stateMachine.currentTarget =
                targetPoints[Random.Range(0, targetPoints.Length)].position;
        }
    }

    public override void ObjectTick()
    {
        if (!startTick)
            return;

        UpdateStateData();

        // Found a player
        if (playerTransform != null)
        {
            stateMachine.ChangeState(stateMachine.moveState);
            return;
        }

        idleTimer -= Time.deltaTime;

        // Finished waiting -> patrol
        if (idleTimer <= 0f)
        {
            stateMachine.ChangeState(stateMachine.moveState);
        }
    }

    public override void Exit()
    {
        // Reset so the next visit to Idle starts fresh
        idleTimer = idleTime;
        startTick = false;
    }
}
