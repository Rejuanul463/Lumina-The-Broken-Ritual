using UnityEngine;

public class EnmeyCombate : EnemyStates
{
    [SerializeField] private float attackCooldown = 2f;

    private float nextAttackTime;
    private bool startTick = false;
    private float dualSlashProbability;
    public override void Enter()
    {
        agent.isStopped = true;
        nextAttackTime = Time.time;
        startTick = true;
    }

    public override void ObjectTick()
    {
        if (!startTick)
        {
            return;
        }
        // Lost player completely
        if (playerDistance > detectionRange)
        {
            stateMachine.ChangeState(stateMachine.idleState);
            return;
        }

        // Player moved out of attack range
        if (playerDistance > agent.stoppingDistance)
        {
            stateMachine.ChangeState(stateMachine.moveState);
            return;
        }

        // Face the player
        Vector3 direction = playerTransform.position - transform.position;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * 8f);
        }

        // Attack
        if (Time.time >= nextAttackTime)
        {
            dualSlashProbability = Random.Range(0f, 100f) / 100f;
            animator.SetFloat("DualSlashProbability", dualSlashProbability);
            animator.SetTrigger("Attack");
            
            if(dualSlashProbability > 0.5) nextAttackTime = Time.time + 1.5f * attackCooldown;
            else nextAttackTime =  Time.time + attackCooldown;
        }
    }
    

    public override void Exit()
    {
        animator.SetTrigger("Move");
        startTick = false;
    }

    public void AttackPerformed()
    {
        Debug.Log("Attack");
    }

    public void dualSlashPerformed()
    {
        Debug.Log("DualSlash");
    }
}