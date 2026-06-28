using UnityEngine;

enum PlayerState
{
    NormalState,
    CombatState
}
public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Animator animator;
    private PlayerInputHandler playerInput;
    
    private PlayerMovement playerMovement;
    private PlayerCombat combat;
    
    PlayerState currentState;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentState = PlayerState.NormalState;
        
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInputHandler>();
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        combat = GetComponent<PlayerCombat>();
    }

    void Update()
    {
        if(currentState == PlayerState.NormalState)
            playerMovement.MovementTick(ref controller, ref animator, ref playerInput);
        else if(currentState == PlayerState.CombatState)
            PlayerCombat.CombateTick(ref controller, ref animator, ref playerInput);
    }
}
