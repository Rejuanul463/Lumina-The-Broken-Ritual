using UnityEngine;

/// <summary>
/// The "brain" of an enemy (also used for the freed companion NPC, see TeamNPC).
/// It is a small state machine: it owns the shared data (nearest player, patrol points,
/// ranges) and forwards Update to the currently active state.
///
/// States (all separate components on the same GameObject, deriving from EnemyStates):
///   EnemyIdle     - wait, then pick a random patrol point
///   EnemyMovement - patrol or chase the player
///   EnmeyCombate  - attack the player
///
/// Flow: Idle -> Move -> (player in attack range) Combat -> (player out of range) Move -> ...
/// </summary>
public class EnemyStateMachine : MonoBehaviour
{
    [Header("References")]
    // Closest detected target this frame (null if nothing is in range). Set by FindClosestPlayer().
    public Transform Player;
    // Patrol waypoints the enemy walks between
    public Transform[] targetPoints;

    // Position the NavMeshAgent should currently walk to (patrol point or player position)
    public Vector3 currentTarget;

    [Header("Detection")]
    // Radius in which targets are detected
    public float detectionDistance = 10f;
    // Distance at which the enemy stops and starts attacking
    public float attackDistance = 2f;

    // The state that is currently running
    [SerializeField] private EnemyStates currentState;

    // Cached references to the state components (filled in Awake)
    [HideInInspector] public EnemyIdle idleState;
    [HideInInspector] public EnemyMovement moveState;
    [HideInInspector] public EnmeyCombate combatState;

    // Which layer(s) count as a target. Set in the inspector.
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
        // Every enemy begins in Idle
        ChangeState(idleState);
    }

    private void Update()
    {
        // Refresh target info first, then let the current state act on it
        FindClosestPlayer();
        currentState?.ObjectTick();
    }

    /// <summary>
    /// Scans for colliders on playerLayer within detectionDistance and stores the closest one
    /// in Player (or null if none). If found, currentTarget is set to its position.
    /// </summary>
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

    /// <summary>
    /// Switches to a new state: calls Exit() on the old one and Enter() on the new one.
    /// Does nothing if the new state is null or already active.
    /// </summary>
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

    /// <summary>Distance to the current target, or Infinity when there is none.</summary>
    public float DistanceToPlayer()
    {
        if (Player == null)
            return Mathf.Infinity;

        return Vector3.Distance(transform.position, Player.position);
    }

#if UNITY_EDITOR
    // Editor-only: red sphere = detection range, yellow sphere = attack range
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
#endif
}
