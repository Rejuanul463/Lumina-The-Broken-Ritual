using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum CreatureState
{
    idle,
    move,
    attack,
    powerAttack
}
public class CreaturesAi : MonoBehaviour
{
    private CreatureState state = CreatureState.idle;
    private Animator anim;
    private NavMeshAgent agent;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float stoppingDistance;
    [SerializeField] private float attackCoolDown;
    private float nextAttackTime;
    private bool isInnitiatingPowerAttack = true;
    private float walkState = 0.3f;
    
    [SerializeField] private BattleArena battleArena;
    
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
        if (state == CreatureState.move)
        {
            move();
        }else if(state == CreatureState.idle){
            Idle();
            
        }else if (state == CreatureState.attack)
        {
            Attack();
        }else if (state == CreatureState.powerAttack)
        {
            PowerAttack();
        }
    }

    private void move()
    {
        if(battleArena.isPlayerInside)
        {
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
            agent.SetDestination(battleArena.transform.position);
        }

        anim.SetFloat("Speed", agent.velocity.magnitude / runSpeed);
        
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            state = CreatureState.idle;
        }
    }

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

    private void Attack()
    {
        if (Time.time > nextAttackTime)
        {
            nextAttackTime = Time.time + attackCoolDown;
            anim.SetTrigger("Attack");
        }
    }

    
    //Need to fix
    private void PowerAttack()
    {
        if (isInnitiatingPowerAttack)
        {
            isInnitiatingPowerAttack = false;
            Vector3 postion = transform.position;
            postion.z -= stoppingDistance + 2f;
            anim.SetTrigger("WalkBack");
            agent.SetDestination(postion);
        }

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            anim.SetTrigger("PowerAttack");
        }
    }

    public void powerAttackComplete()
    {
        isInnitiatingPowerAttack  = true;
    }
}
