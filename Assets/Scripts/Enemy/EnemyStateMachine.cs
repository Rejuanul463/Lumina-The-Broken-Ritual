using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    [Header("References")]
    public Transform Player;
    public Transform[] targetPoints;

    public Vector3 currentTarget;

    [Header("Detection")]
    public float detectionDistance = 10f;
    public float attackDistance = 2f;

    [SerializeField] private EnemyStates currentState;

    [HideInInspector] public EnemyIdle idleState;
    [HideInInspector] public EnemyMovement moveState;
    [HideInInspector] public EnmeyCombate combatState;

    [SerializeField]  private LayerMask playerLayer;

    private void Awake()
    {
        idleState = GetComponent<EnemyIdle>();
        moveState = GetComponent<EnemyMovement>();
        combatState = GetComponent<EnmeyCombate>();

        // playerLayer = LayerMask.GetMask("Player");
    }

    private void Start()
    {
        ChangeState(idleState);
    }

    private void Update()
    {
        FindClosestPlayer();
        currentState?.ObjectTick();
    }

    private void FindClosestPlayer()
    {
        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            detectionDistance,
            playerLayer);

        Player = null;

        float closestDistance = Mathf.Infinity;

        foreach (Collider col in colliders)
        {
            float distance = Vector3.Distance(transform.position, col.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                Player = col.transform;
            }
        }

        // Always chase the player if one is found
        if (Player != null)
        {
            currentTarget = Player.position;
        }
    }

    public void ChangeState(EnemyStates newState)
    {
        if (newState == null)
            return;

        if (currentState == newState)
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

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
#endif
}