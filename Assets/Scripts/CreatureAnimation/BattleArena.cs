using UnityEngine;

public class BattleArena : MonoBehaviour
{
    public Transform player;

    public float ArenaRange;

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
