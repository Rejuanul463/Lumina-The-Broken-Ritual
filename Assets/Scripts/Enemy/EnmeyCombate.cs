using UnityEngine;

// NOTE: "Enmey"/"Combate" are typos of "Enemy"/"Combat". They are kept as-is because renaming
// the class/file would break the component references already set up in scenes and prefabs.
/// <summary>
/// Enemy state: fighting. The enemy stands still, turns toward the player and triggers attack
/// animations on a cooldown. It leaves this state when the player dies/leaves detection (-> Idle)
/// or moves out of attack range (-> Movement).
/// Animator parameters used: Attack (trigger), DualSlashProbability (float), Move (trigger).
/// </summary>
public class EnmeyCombate : EnemyStates
{
    [Header("Attack")]
    // Seconds between two attacks
    [SerializeField] private float attackCooldown = 1.2f;

    // Earliest Time.time at which the next attack may start
    private float nextAttackTime;
    // Guards ObjectTick so it only runs between Enter() and Exit()
    private bool startTick;

    public override void Enter()
    {
        // Stand still while attacking
        agent.isStopped = true;
        agent.ResetPath();

        // Allow attacking immediately
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
        // (horizontal only; y is zeroed so the enemy doesn't tilt up/down)
        Vector3 direction = playerTransform.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Smooth turn (8 = turn speed)
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * 8f);
        }

        // Attack
        if (Time.time >= nextAttackTime)
        {
            // Random 0-1 value the Animator uses to choose between the normal and the dual-slash attack
            float dualSlashProbability = Random.Range(0f, 1f);

            animator.SetFloat("DualSlashProbability", dualSlashProbability);
            animator.SetTrigger("Attack");

            nextAttackTime = Time.time + attackCooldown;
        }
    }

    public override void Exit()
    {
        startTick = false;
        // Return the animator to the movement state
        animator.SetTrigger("Move");
    }

    // Animation Event
    // Called from the attack animation at the moment the hit lands (currently only logs).
    public void AttackPerformed()
    {
        Debug.Log("Attack Performed");
    }

    // Animation Event
    // Called from the dual slash animation at the moment the hit lands (currently only logs).
    public void DualSlashPerformed()
    {
        Debug.Log("Dual Slash Performed");
    }

    // Animation Event
    // Delays the next attack by extraTime seconds (e.g. to make a long animation finish first).
    public void IncreaseNextAttackTime(float extraTime)
    {
        nextAttackTime += extraTime;
    }
}
