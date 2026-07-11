using UnityEngine;

public class EnemyIdle : EnemyStates
{
    [SerializeField] private float idleTime = 2f;

    [SerializeField] private float idleTimer;
    private bool startTick;

    public override void Enter()
    {
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
        idleTimer = idleTime;
        startTick = false;
    }
}