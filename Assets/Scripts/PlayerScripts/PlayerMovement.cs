using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    private PlayerController playerController;
    private Vector3 movement;
    Vector3 speedVelocity;
    private float verticalVelocity;
    private bool jumpRequested;
    private Vector3 horizontalMovement;

    [Header("Movement Parameters")] 
    private float currentSpeed;
    private float speed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float gravity;
    [SerializeField] private bool isGrounded;
    [SerializeField] private float feetRadius;
    
    
    [Header("Components")] 
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform feet;
    
    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        speed = walkSpeed;
    }

    public void InputUpdate(ref PlayerInputHandler inputHandler)
    {
        if (inputHandler.jump)
        {
            jumpRequested = true;
        }
    }
    
    public void MovementTick(ref CharacterController controller, ref Animator animator, ref PlayerInputHandler inputHandler)
    {
        playerSpeedHandler(inputHandler.sprint);

        isGrounded = Physics.CheckSphere(feet.position, feetRadius, groundMask);

        if (isGrounded)
        {
            // Horizontal movement
            float accelerationTime = .5f;
            float decelerationTime = .5f;

            Vector3 targetVelocity = inputHandler.moveDirection.sqrMagnitude > 0f
                ? inputHandler.moveDirection * speed
                : Vector3.zero;

            horizontalMovement = Vector3.SmoothDamp(
                horizontalMovement,
                targetVelocity,
                ref speedVelocity,
                targetVelocity.sqrMagnitude > horizontalMovement.sqrMagnitude
                    ? accelerationTime
                    : decelerationTime
            );
            
            animator.SetFloat("Speed", horizontalMovement.magnitude / runSpeed);
            
            if (verticalVelocity < 0)
                verticalVelocity = -2f;

            if (jumpRequested)
            {
                verticalVelocity = Mathf.Sqrt(jumpForce * -2f * gravity);
                jumpRequested = false;
            }
        }
        else
        {
            verticalVelocity += 2 * gravity * Time.fixedDeltaTime;
        }

        movement = horizontalMovement;
        movement.y = verticalVelocity;
        controller.Move(movement * Time.fixedDeltaTime);
    }
    
    private void playerSpeedHandler(bool sprint)
    {
        if (sprint)
        {
            speed = runSpeed;
        }
        else
        {
            speed = walkSpeed;
        }
    }
}
