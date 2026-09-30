using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// High-level states of the arena creature (boss).
/// idle        - decides what to do next (attack if close enough, otherwise move)
/// move        - chases the player (if inside the arena) or returns to the arena centre
/// attack      - normal attack; every 4th attack becomes a power attack
/// powerAttack - steps backwards, then plays the power attack animation
/// </summary>
public enum CreatureState
{
    idle,
    move,
    attack,
    powerAttack
}

/// <summary>
/// Simple state-machine AI for the creature fought inside a BattleArena.
/// Uses a NavMeshAgent for movement and an Animator for visuals.
/// Requires on the same GameObject: Animator, NavMeshAgent.
/// Animator parameters used: Speed (float), Attack / WalkBack / PowerAttack (triggers).
/// </summary>
public class CreaturesAi : MonoBehaviour
{
    private CreatureState state = CreatureState.idle;
    private Animator anim;
    private NavMeshAgent agent;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    // How close the creature gets to its destination; also used as its attack range
    [SerializeField] private float stoppingDistance;
    // Minimum seconds between two normal attacks
    [SerializeField] private float attackCoolDown;
    private float nextAttackTime;
    // True when the next power attack has not started yet (used to run the "walk back" setup once)
    private bool isInnitiatingPowerAttack = true;
    // Fraction used to decide walk vs run based on remaining distance
    private float walkState = 0.3f;
    // While false the NavMeshAgent is stopped (e.g. during attack animations)
    private bool isMovable = true;
    // Target point the creature steps back to before a power attack
    private Vector3 position;

    // Number of attacks performed so far; every 4th one is a power attack (see Attack())
    private int attackCount;

    // The arena that tells us where the player is and whether they are inside
    [SerializeField] private BattleArena battleArena;


    // Optional "go here" arrow indicator; hidden permanently once the player enters the arena
    [SerializeField] private GameObject Arrow;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        // Freeze / unfreeze the agent depending on whether we are allowed to move
        if (isMovable) agent.isStopped = false;
        else agent.isStopped = true;

        // Run the logic of the current state
        if (state == CreatureState.move)
        {
            move();
        }
        else if(state == CreatureState.idle){
            Idle();

        }
        else if (state == CreatureState.attack)
        {
            Attack();
        }
        else if (state == CreatureState.powerAttack)
        {
            PowerAttack();
        }
    }

    /// <summary>
    /// Chase the player when inside the arena, otherwise walk back to the arena centre.
    /// Switches to idle once the destination is reached.
    /// </summary>
    private void move()
    {
        if(battleArena.isPlayerInside)
        {
            // The player has entered the arena, so the guiding arrow is no longer needed
            if(Arrow != null)
            {
                Arrow.SetActive(false);
                Arrow = null;
            }
            // Run when far from the player, walk when close
            if (agent.remainingDistance * walkState * 2 > agent.stoppingDistance)
            {
                agent.speed = runSpeed;
            }
            else
            {
                agent.speed = walkSpeed;
            }
            agent.SetDestination(battleArena.player.transform.position);
        }
        else
        {
            // Player left the arena: go back to the middle of it
            agent.SetDestination(battleArena.transform.position);
        }

        // Drive the walk/run blend tree (0 = still, 1 = full run speed)
        anim.SetFloat("Speed", agent.velocity.magnitude / runSpeed);

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            state = CreatureState.idle;
        }
    }

    /// <summary>
    /// Decision state: attack if the player is inside the arena and within attack range,
    /// otherwise go back to moving.
    /// </summary>
    private void Idle()
    {
        anim.SetFloat("Speed", 0);
        if (battleArena.isPlayerInside && Vector3.Distance(transform.position, battleArena.player.transform.position) < agent.stoppingDistance)
        {
            state = CreatureState.attack;
        }
        else
        {
            state = CreatureState.move;
        }
    }

    /// <summary>
    /// Normal attack. Every 4th attack (attackCount % 4 == 0, including the very first)
    /// is redirected to a power attack. Otherwise triggers the attack animation when the
    /// cooldown has passed and keeps facing the player.
    /// </summary>
    private void Attack()
    {
        if (attackCount % 4 == 0)
        {
            PowerAttack();
            return;
        }
        if (Time.time > nextAttackTime)
        {
            // Stop moving until the animation calls AttackPerformed()
            isMovable = false;
            nextAttackTime = Time.time + attackCoolDown;
            anim.SetTrigger("Attack");
        }
        transform.LookAt(battleArena.player.transform);

        // Player moved out of range - chase again
        if (Vector3.Distance(transform.position, battleArena.player.transform.position) > agent.stoppingDistance)
        {
            state = CreatureState.move;
        }
    }

    //Need to fix
    /// <summary>
    /// Power attack sequence: face the player, step one unit backwards (WalkBack animation),
    /// then trigger the PowerAttack animation. Ends when the animation calls powerAttackComplete().
    /// </summary>
    private void PowerAttack()
    {
        transform.LookAt(battleArena.player.transform);
        isMovable = false;

        // One-time setup: pick the point one unit behind us and start the walk-back animation
        if (isInnitiatingPowerAttack)
        {
            isInnitiatingPowerAttack = false;
            position = transform.position - transform.forward;
            position.y = transform.position.y;
            anim.SetTrigger("WalkBack");

        }

        // Move manually (not via the NavMeshAgent) toward the step-back point
        transform.position =  Vector3.MoveTowards(transform.position, position, walkSpeed * Time.deltaTime);
        Debug.Log(position);
        // Reached the step-back point -> fire the power attack animation
        // (exact float comparison; this is one of the parts flagged "Need to fix")
        if (transform.position.x == position.x && transform.position.z == position.z)
        {
            anim.SetTrigger("PowerAttack");
            state = CreatureState.idle;
        }
    }

    // Animation Event: call at the end of the PowerAttack animation.
    // Re-enables movement, resets the power-attack setup flag and counts the attack.
    public void powerAttackComplete()
    {
        isInnitiatingPowerAttack  = true;
        isMovable = true;
        attackCount++;
    }

    // Animation Event: call at the end of the normal Attack animation.
    // Re-enables movement and counts the attack (drives the "every 4th is a power attack" rule).
    public void AttackPerformed()
    {
        isMovable = true;
        attackCount++;
    }
}
