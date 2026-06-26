using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private PlayerController playerController;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
    }
    
    public static void CombateTick(ref CharacterController controller, ref Animator animator, ref PlayerInputHandler playerInput)
    {
        
    }
}
