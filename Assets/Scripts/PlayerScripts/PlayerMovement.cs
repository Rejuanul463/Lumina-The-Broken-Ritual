using System.Collections;
using NUnit.Framework.Constraints;
using Unity.Android.Gradle.Manifest;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    [Header("Components")] 
    private PlayerController playerController;

    [Header("Movement Parameters")]
    private Vector3 moveDir;
    private Vector3 speedVelocity;
    private Vector3 horizontalMovement = Vector3.zero;
    private float currentSpeed;
    private float speed;
    private bool jumpRequested;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    private float verticalVelocity;
    
    [Header("Rotation Parameters")]
    [SerializeField] public Transform cam;
    [SerializeField] private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    
    [Header("Jump Parameters")]
    [SerializeField] private bool isGrounded;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform feet;
    [SerializeField] private float gravity = 20f;
    [SerializeField] private float jumpHeight = 3f;
    
    [Header("Crouch Parameters")]
    private bool isCrouching;
    [SerializeField] private float crouchSpeed = 2f;
    
    private void Start()
    {
        // Time.timeScale = .4f;
        cam = Camera.main.transform;
        playerController = GetComponent<PlayerController>();
        speed = walkSpeed;
    }
    
    public void MovementTick(ref CharacterController controller, ref Animator animator, ref PlayerInputHandler inputHandler)
    {
        if (inputHandler.crouch && playerController.doMove)
        {
            isCrouching = !isCrouching;
            animator.SetBool("isCrouching", isCrouching);
        }
        
        if (isCrouching && !playerController.isFocused)
        {
            speed = crouchSpeed;
        }
        else
        {
            playerSpeedHandler(inputHandler.sprint);
        }
        
        Vector3 direction = inputHandler.moveDirection;

        isGrounded = Physics.CheckSphere(feet.position, 0.1f, groundMask);

        if (isGrounded)
        {
            // Keep the controller grounded
            if (verticalVelocity < 0f)
                verticalVelocity = -2f;

            if (inputHandler.jump && !isCrouching && playerController.doMove)
            {
                jumpRequested = true;
            }

            if (direction.magnitude > 0.1f)
            {
                if (playerController.isFocused)
                {
                    FocusedMovement(ref direction);
                }
                else
                {
                    UnfocusedMovement(ref animator, ref direction);
                }
            }
            else
            {
                moveDir = Vector3.zero;
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
            // Apply gravity
            verticalVelocity -= gravity * Time.deltaTime;
        }

        animator.SetBool("isGround", isGrounded);

        Vector3 smoothedMovement = SmoothMovement(moveDir.normalized);

        animator.SetFloat(
            "Speed",
            new Vector3(smoothedMovement.x, 0f, smoothedMovement.z).magnitude / runSpeed
        );
        
        animator.SetFloat("xDir",  direction.x * animMultiplier);
        animator.SetFloat("zDir", direction.z * animMultiplier);

        // Apply vertical movement
        smoothedMovement.y = verticalVelocity;

        if(playerController.doMove) controller.Move(smoothedMovement * Time.deltaTime);
    }

    private void UnfocusedMovement(ref Animator animator, ref Vector3 direction)
    {
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(
            transform.eulerAngles.y,
            targetAngle,
            ref turnSmoothVelocity,
            turnSmoothTime);

        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        float delta = Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle);

        if (Mathf.Abs(delta) < 1f)
            animator.SetFloat("direction", 0f);
        else if (delta > 0f)
            animator.SetFloat("direction", 0.1f);
        else
            animator.SetFloat("direction", -0.1f);

        moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
    }
    
    private void FocusedMovement(ref Vector3 direction)
    {
        moveDir = transform.right * direction.x +
                  transform.forward * direction.z;
        
        if (moveDir.sqrMagnitude > 1f)
            moveDir.Normalize();
    }
    private float animMultiplier = 1f;
    private void playerSpeedHandler(bool sprint)
    {
        if (sprint)
        {
            if (playerController.isFocused)
            {
                speed = runSpeed * 0.6f;
                animMultiplier = 1f;
            }
            else
            {
                speed = runSpeed;
                turnSmoothTime = 0.2f;
                accelerationTime = 0.2f;
                decelerationTime = 0.5f;
            }
        }
        else
        {
            animMultiplier = 0.5f;
            speed = walkSpeed;
            StartCoroutine(ChangeAcceleration(decelerationTime));
        }
    }

    IEnumerator ChangeAcceleration(float time)
    {
        yield return new WaitForSeconds(time);
        accelerationTime = 0.05f;
        decelerationTime = 0.02f;
        turnSmoothTime = 0.1f;
    }

    private float accelerationTime;
    private float decelerationTime;
    private Vector3 SmoothMovement(Vector3 moveDirection)
    {
        Vector3 targetVelocity = moveDirection.sqrMagnitude > 0f
            ?  moveDirection * speed
            : Vector3.zero;
        
        horizontalMovement = Vector3.SmoothDamp(
            horizontalMovement,
            targetVelocity,
            ref speedVelocity,
            targetVelocity.sqrMagnitude > horizontalMovement.sqrMagnitude ? accelerationTime : decelerationTime
        );
        return horizontalMovement;
    }
}



