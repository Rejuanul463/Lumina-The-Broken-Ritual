using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerCombat : MonoBehaviour
{
    [Header("Components")] 
    private PlayerController playerController;

    [Header("Movement Parameters")]
    private bool jumpRequested;
    private float verticalVelocity;
    private float walkSpeed = 2f;
    
    [Header("Jump Parameters")]
    [SerializeField] private bool isGrounded;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform feet;
    [SerializeField] private float gravity = 20f;
    [SerializeField] private float jumpHeight = 3f;
    

    [Header("Combat Parameters")] [SerializeField]
    private int slashState;
    public bool isParrying;
    
    private void Start()
    {
        // Time.timeScale = .4f;
        playerController = GetComponent<PlayerController>();
    }
    
    public void CombateTick(ref CharacterController controller, ref Animator animator, ref PlayerInputHandler inputHandler)
    {
        if (inputHandler.parry)
        {
            isParrying = true;
            animator.SetBool("Parrying", true);
        }
        else if(isParrying)
        {
            isParrying = false;
            animator.SetBool("Parrying", false);
            ResetAttack();
        }
        VerticalMovement(ref controller, ref animator, ref inputHandler);
        isGrounded = Physics.CheckSphere(feet.position, 0.1f, groundMask);
        if (isGrounded)
        {
            if(inputHandler.sprint) animator.SetBool("Shifted", true);
            else animator.SetBool("Shifted", false);
            if (inputHandler.attack)
            {
                Debug.Log("Perform Slash");
                if(slashState < 2)  slashState += 1;
            }
        }
        animator.SetInteger("Slash", slashState);
        animator.SetFloat("Speed", 0f);
    }
    
    private void VerticalMovement(ref CharacterController controller, ref Animator animator,
        ref PlayerInputHandler inputHandler)
    {
        Vector3 direction = inputHandler.moveDirection;
        isGrounded = Physics.CheckSphere(feet.position, 0.1f, groundMask);
        if (isGrounded)
        {
            // Keep the controller grounded
            if (verticalVelocity < 0f)
                verticalVelocity = -2f;

            if (inputHandler.jump && playerController.doMove)
            {
                jumpRequested = true;
            }
            // Apply jump AFTER movement direction is calculated
            if (jumpRequested)
            {
                animator.SetTrigger("Jump");
                verticalVelocity = Mathf.Sqrt(jumpHeight * 2f * gravity);
                jumpRequested = false;
            }
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }
        animator.SetBool("isGround", isGrounded);
        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }


    private void HorizontalMovement(ref PlayerInputHandler inputHandler, ref Animator animator, ref CharacterController controller)
    {
        // animator.SetLayerWeight();
        Vector3 moveDirection = inputHandler.moveDirection.magnitude > 0.1f ? inputHandler.moveDirection.normalized + Camera.main.transform.forward : Vector3.zero;
        controller.Move(moveDirection * walkSpeed * Time.deltaTime);
        animator.SetFloat("xDir", moveDirection.x);
        animator.SetFloat("zDir", moveDirection.z);
    }
    public void ResetAttack()
    {
        slashState = 0;
        playerController.currentState = PlayerState.NormalState;
    }
}
