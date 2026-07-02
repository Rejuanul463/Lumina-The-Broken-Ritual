using UnityEngine;

public class EnemyMovement : EnemyStates
{
    public float walkSpeed;
    public float runSpeed;
    
    public float walkStart = .3f;
    private bool startTick = false;
    public override void Enter()
    {
        agent.isStopped = false;
        animator.SetTrigger("Move");
        startTick = true;
    }

    public override void ObjectTick()
    {
        if (!startTick) return;
        // Lost player
        if (playerDistance > detectionRange)
        {
            stateMachine.ChangeState(stateMachine.idleState);
            return;
        }
        
        if (playerDistance < detectionRange * walkStart)
        {
            agent.speed = walkSpeed;
            animator.SetFloat("Speed", walkStart);
        }
        else
        {
            agent.speed = runSpeed;
            animator.SetFloat("Speed", 1f);
        }

        // Update destination every frame
        agent.SetDestination(playerTransform.position);

        // Close enough to attack
        if (playerDistance <= agent.stoppingDistance)
        {
            stateMachine.ChangeState(stateMachine.combatState);
        }
    }
    

    public override void Exit()
    {
        agent.ResetPath();
        startTick = false;
    }
}