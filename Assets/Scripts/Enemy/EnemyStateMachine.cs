using UnityEngine;


public class EnemyStateMachine : MonoBehaviour
{
    [Header("References")]
    public Transform Player;
    public Transform[] targetPoints;

    [Header("Detection")]
    public float detectionDistance = 10f;
    public float attackDistance = 2f;

    [SerializeField] private EnemyStates currentState;

    [HideInInspector] public EnemyIdle idleState;
    [HideInInspector] public EnemyMovement moveState;
    [HideInInspector] public EnmeyCombate combatState;

    private void Awake()
    {
        idleState = GetComponent<EnemyIdle>();
        moveState = GetComponent<EnemyMovement>();
        combatState = GetComponent<EnmeyCombate>();
    }

    private void Start()
    {
        ChangeState(idleState);
    }

    private void Update()
    {
        currentState?.ObjectTick();
    }
    

    public void ChangeState(EnemyStates newState)
    {
        if (newState == null)
            return;

        if (currentState != null)
            currentState.Exit();

        currentState = newState;

        currentState.Enter();
    }

    public float DistanceToPlayer()
    {
        if (Player == null)
            return Mathf.Infinity;

        return Vector3.Distance(transform.position, Player.position);
    }
}
