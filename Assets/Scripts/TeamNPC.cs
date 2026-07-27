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

    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private string playerTag;

    public OpenCage cage;
    private bool isUIShowed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        statemachine = GetComponent<EnemyStateMachine>();
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = 1f;
        
        statemachine.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFree) agent.stoppingDistance = 1.5f;
        
        if(!isUIShowed && isFree)
        {
            if(agent.remainingDistance <= agent.stoppingDistance)
                UiShowed();
            
            return;
        }
        
        if(isFree) FindClosestEnemy();
        
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
        agent.speed = 3.8f;
        gameObject.layer = Mathf.RoundToInt(Mathf.Log(playerLayer.value, 2));
        gameObject.tag = playerTag;
        agent.SetDestination(player.transform.position);
        if(agent.remainingDistance > agent.stoppingDistance) anim.SetFloat("Speed", 1f);
        else anim.SetFloat("Speed", 0);
    }
    
    
    private void FindClosestEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            detectionDistance,
            enemyLayer);

        if (colliders.Length > 0)
        {
            stateMachineActivated = true;
            statemachine.enabled = true;
        }
        else
        {
            stateMachineActivated = false;
            statemachine.enabled = false;
        }
    }
    
    public void UiShowed()
    {
        isUIShowed = true;
        agent.stoppingDistance = 2f;
        cage.ShowUI();
    }
}
