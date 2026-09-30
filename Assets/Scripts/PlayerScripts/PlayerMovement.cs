using System.Collections;
using UnityEngine;


/// <summary>
/// Handles the player's locomotion: walking, sprinting, crouching, jumping, gravity and turning.
/// It has no Update of its own; PlayerController calls MovementTick() every frame.
/// Movement is camera-relative. There are two movement styles:
///  - Unfocused (default): the character turns to face the direction it is moving.
///  - Focused (locked on to an enemy): the character keeps facing the target and strafes.
/// Also drives the movement Animator parameters: Speed, xDir, zDir, direction, isGround, isCrouching, Jump.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    private PlayerController playerController;

    [Header("Movement Parameters")]
    // World-space direction the player is currently moving in (before smoothing)
    private Vector3 moveDir;
    // Velocity holder required by SmoothDamp
    private Vector3 speedVelocity;
    // Smoothed horizontal velocity that is actually applied
    private Vector3 horizontalMovement = Vector3.zero;
    private float currentSpeed;
    // Current target speed (switches between walk/run/crouch)
    public float speed;
    private bool jumpRequested;
    [SerializeField] private float walkSpeed = 2;
    [SerializeField] private float runSpeed = 7;
    // Current up/down velocity (positive = going up)
    private float verticalVelocity;

    [Header("Rotation Parameters")]
    // Camera used to make movement camera-relative (assigned from Camera.main in Start)
    [SerializeField] public Transform cam;
    // How quickly the character turns toward its movement direction
    [SerializeField] private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    [Header("Jump Parameters")]
    [SerializeField] private bool isGrounded;
    // Layers that count as ground
    [SerializeField] private LayerMask groundMask;
    // Empty child object at the player's feet used for the ground check
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

    /// <summary>
    /// Main per-frame movement update, called from PlayerController.
    /// Steps: crouch toggle -> pick speed -> ground check -> work out move direction
    /// -> jump/gravity -> smooth the velocity -> update animator -> move the CharacterController.
    /// </summary>
    public void MovementTick(ref CharacterController controller, ref Animator animator, ref PlayerInputHandler inputHandler)
    {
        // Crouch button toggles crouching (only when movement isn't blocked by an animation)
        if (inputHandler.crouch && playerController.doMove)
        {
            isCrouching = !isCrouching;
            animator.SetBool("isCrouching", isCrouching);
        }

        // Choose speed: crouch speed, otherwise walk/run depending on sprint
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

            // Can't jump while crouching or when movement is blocked by an animation
            if (inputHandler.jump && !isCrouching && playerController.doMove)
            {
                jumpRequested = true;
            }

            if (direction.magnitude > 0.1f)
            {
                // Locked on to an enemy -> strafe; otherwise turn toward the movement direction
                if (playerController.isFocused)
                {
                    FocusedMovement(ref direction, ref animator);
                }
                else
                {
                    UnfocusedMovement(ref animator, ref direction);
                }
            }
            else
            {
                // No input -> stop
                moveDir = Vector3.zero;
            }
            // Apply jump AFTER movement direction is calculated
            // Jump velocity from height: v = sqrt(2 * gravity * height)
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

        // Smooth acceleration/deceleration of the horizontal velocity
        Vector3 smoothedMovement = SmoothMovement(moveDir.normalized);

        // "Speed" = 0..1 value for the locomotion blend tree (1 = full run speed)
        animator.SetFloat(
            "Speed",
            new Vector3(smoothedMovement.x, 0f, smoothedMovement.z).magnitude / runSpeed
        );

        // animator.SetFloat("xDir",  direction.x * animMultiplier);
        // animator.SetFloat("zDir", direction.z * animMultiplier);
        // xDir/zDir = movement relative to the character (used for strafe animations when locked on)
        Vector3 localMove = transform.InverseTransformDirection(moveDir);

        animator.SetFloat("xDir", localMove.x * animMultiplier);
        animator.SetFloat("zDir", localMove.z * animMultiplier);
        // Apply vertical movement
        smoothedMovement.y = verticalVelocity;

        // Actually move the character (skipped while an animation blocks movement)
        if(playerController.doMove) controller.Move(smoothedMovement * Time.deltaTime);
    }

    /// <summary>
    /// Normal (free) movement: rotates the character toward the input direction relative to the
    /// camera, and sets moveDir to that direction. Also sets the "direction" animator value
    /// (-0.1 / 0 / 0.1) depending on whether the character is turning left, right or not at all.
    /// </summary>
    private void UnfocusedMovement(ref Animator animator, ref Vector3 direction)
    {
        // Input angle + camera yaw = angle the character should face in world space
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(
            transform.eulerAngles.y,
            targetAngle,
            ref turnSmoothVelocity,
            turnSmoothTime);

        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        // Remaining angle to turn (used to pick the turning animation)
        float delta = Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle);

        if (Mathf.Abs(delta) < 1f)
            animator.SetFloat("direction", 0f);
        else if (delta > 0f)
            animator.SetFloat("direction", 0.1f);
        else
            animator.SetFloat("direction", -0.1f);

        // Move toward the target angle
        moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
    }

    /// <summary>
    /// Lock-on movement: the character does not rotate here (PlayerController makes it face the
    /// target); input is converted to camera-relative directions so the player can strafe around.
    /// </summary>
    private void FocusedMovement(ref Vector3 direction, ref Animator animator)
    {
        // Flatten the camera vectors
        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        // Camera-relative movement
        moveDir = camForward * direction.z +
                  camRight * direction.x;

        if (moveDir.sqrMagnitude > 1f)
        {
            moveDir.Normalize();
        }
    }

    // Scales the xDir/zDir animator values (1 when running or locked on, 0.5 when walking)
    private float animMultiplier = 1f;

    /// <summary>
    /// Chooses speed and smoothing values.
    ///  - Sprinting: run speed, cancels lock-on, slower turning and softer acceleration.
    ///  - Not sprinting: walk speed; after a delay the smoothing values return to their defaults.
    ///  - Locked on ("only for wizkid"): moves at half of run speed.
    /// </summary>
    private void playerSpeedHandler(bool sprint)
    {
        if (sprint)
        {
            // Sprinting always breaks the lock-on
            playerController.isFocused = false;
            // NOTE: because isFocused was just set to false, the first branch below never runs.
            if (playerController.isFocused)
            {
                speed = runSpeed * 0.4f;
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
            animMultiplier = .5f;
            speed = walkSpeed;
            StartCoroutine(ChangeAcceleration(decelerationTime));
        }

        // Only for wizkid
        if (playerController.isFocused)
        {
            speed = runSpeed * 0.5f;
            animMultiplier = 1f;
        }
    }

    // After "time" seconds, restores the default (snappy) turn/acceleration values.
    // Delaying it lets the character slide to a stop after sprinting.
    IEnumerator ChangeAcceleration(float time)
    {
        yield return new WaitForSeconds(time);
        accelerationTime = 0.05f;
        decelerationTime = 0.02f;
        turnSmoothTime = 0.1f;
    }

    // SmoothDamp times: how long to reach full speed / how long to come to a stop
    private float accelerationTime;
    private float decelerationTime;

    /// <summary>
    /// Smoothly changes the horizontal velocity toward (direction * speed), or toward zero when
    /// there is no input. Uses accelerationTime when speeding up and decelerationTime when slowing down.
    /// </summary>
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



