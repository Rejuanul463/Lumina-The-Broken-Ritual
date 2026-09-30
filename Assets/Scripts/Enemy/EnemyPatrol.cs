using UnityEngine;

// NOTE: this file is named EnemyPatrol.cs but the class inside is FollowPlayer.
// Unity requires a MonoBehaviour's class name to match its file name to attach it in the editor,
// so if you want to use this state, rename the class or the file to match.
/// <summary>
/// Simple extra enemy state that makes the enemy follow a given player.
/// It always faces the player and sets the NavMeshAgent destination to the player's position.
/// It is NOT referenced by EnemyStateMachine, so it is currently unused.
/// </summary>
public class FollowPlayer : EnemyStates
{
    public Transform player;
    public PlayerMovement playerMovement;

    public override void Enter()
    {
        // Jump straight into the "Move" animation state and stop 3 units from the player
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
