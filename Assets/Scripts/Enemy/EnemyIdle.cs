using UnityEngine;

public class EnemyIdle : EnemyStates
{
    private bool startTick = false;
    public override void Enter()
    {
        agent.isStopped = true;
        agent.ResetPath();
        animator.SetTrigger("Idle");
        startTick = true;
    }

    public override void ObjectTick()
    {
        if (!startTick)
        {
            return;
        }
        if (playerDistance <= agent.stoppingDistance)
        {
            stateMachine.ChangeState(stateMachine.combatState);
            return;
        }
        
        if (playerDistance <= detectionRange )
        {
            stateMachine.ChangeState(stateMachine.moveState);
        }
    }
    

    public override void Exit()
    {
        startTick = false;
    }
}