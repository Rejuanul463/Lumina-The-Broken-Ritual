using UnityEngine;

/// <summary>
/// Defines a circular boss/creature fight area around this object.
/// Every frame it checks whether the player is within ArenaRange and exposes the result
/// through isPlayerInside. CreaturesAi reads this flag to decide whether to chase the
/// player or return to the arena centre.
/// </summary>
public class BattleArena : MonoBehaviour
{
    // The player to track
    public Transform player;

    // Radius of the arena (measured from this object's position)
    public float ArenaRange;

    // True while the player is within ArenaRange
    public bool isPlayerInside;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            // Simple distance check (3D sphere, not just the ground plane)
            if (Vector3.Distance(player.position, transform.position) < ArenaRange)
            {
                isPlayerInside = true;
            }
            else
            {
                isPlayerInside = false;
            }
        }
    }

#if UNITY_EDITOR
    // Editor-only: draws the arena radius when the object is selected.
    // Red = player is inside, Yellow = player is outside.
    private void OnDrawGizmosSelected()
    {
        if (isPlayerInside)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, ArenaRange);
        }
        else
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, ArenaRange);
        }
    }
#endif
}
