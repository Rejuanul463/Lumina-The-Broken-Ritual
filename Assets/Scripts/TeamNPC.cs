using UnityEngine;
using UnityEngine.AI;

public class TeamNPC : MonoBehaviour
{
    public Transform player;
    private EnemyStateMachine statemachine;
    private bool stateMachineActivated;
    public bool isFree = false;
    public Animator anim;
    private NavMeshAgent agent;
    private float detectionDistance = 10f;
    [SerializeField]
    private LayerMask playerLayer;

    [SerializeField] private string playerTag;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        statemachine = GetComponent<EnemyStateMachine>();
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = 3f;
        
        statemachine.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (stateMachineActivated) return;
        if (!isFree)
        {
            gameObject.layer = LayerMask.NameToLayer("Default");
            gameObject.tag = "Untagged";
            anim.SetFloat("Speed", 0);
        }
        else
        {
            FollowPlayer();
        }
    }

    void FollowPlayer()
    {
        gameObject.layer = playerLayer;
        gameObject.tag = playerTag;
        
        agent.SetDestination(player.transform.position);
        if(agent.remainingDistance > agent.stoppingDistance) anim.SetFloat("Speed", 0f);
        else anim.SetFloat("Speed", 1);
        
        FindClosestPlayer();
    }
    
    
    private void FindClosestPlayer()
    {
        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            detectionDistance,
            playerLayer);

        if (colliders.Length > 0)
        {
            stateMachineActivated = true;
            statemachine.enabled = true;
        }
    }
}
