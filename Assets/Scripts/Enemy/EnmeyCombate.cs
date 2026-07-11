using UnityEngine;

public class EnmeyCombate : EnemyStates
{
    [Header("Attack")]
    [SerializeField] private float attackCooldown = 1.2f;

    private float nextAttackTime;
    private bool startTick;

    public override void Enter()
    {
        agent.isStopped = true;
        agent.ResetPath();

        nextAttackTime = Time.time;
        startTick = true;
    }

    public override void ObjectTick()
    {
        if (!startTick)
            return;

        UpdateStateData();

        // Lost the player
        if (playerTransform == null)
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
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
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
            float dualSlashProbability = Random.Range(0f, 1f);
            
            animator.SetFloat("DualSlashProbability", dualSlashProbability);
            animator.SetTrigger("Attack");
            
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    public override void Exit()
    {
        startTick = false;
        animator.SetTrigger("Move");
    }

    // Animation Event
    public void AttackPerformed()
    {
        Debug.Log("Attack Performed");
    }

    // Animation Event
    public void DualSlashPerformed()
    {
        Debug.Log("Dual Slash Performed");
    }

    // Animation Event
    public void IncreaseNextAttackTime(float extraTime)
    {
        nextAttackTime += extraTime;
    }
}