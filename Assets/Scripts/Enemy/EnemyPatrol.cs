using UnityEngine;

public class FollowPlayer : EnemyStates
{
    public Transform player;
    public PlayerMovement playerMovement;
    public override void Enter()
    {
        animator.Play("Move");
        agent.stoppingDistance = 3f;
    }

    public override void ObjectTick()
    {
        transform.LookAt(player.position);
        agent.SetDestination(player.position);
    }
    

    public override void Exit()
    {
        // agent.stoppingDistance = 
    }
}
