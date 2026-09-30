using UnityEngine;

/// <summary>
/// Third-person orbit camera that follows the player.
/// Mouse/look input rotates the camera around the target (yaw = left/right, pitch = up/down),
/// and the camera is placed behind the target at a fixed distance and height.
/// Both rotation and position are smoothed so the camera feels less rigid.
/// Runs in LateUpdate so it moves AFTER the player has moved for the frame (avoids jitter).
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    // Source of the look/mouse input (reads playerInputHandler.lookDirection)
    public PlayerInputHandler playerInputHandler;
    // The transform the camera orbits around (normally the player)
    public Transform target;
    // Cached from the target in Start (currently not used for logic)
    private PlayerMovement characterMovement;

    [Header("Distance & Height")]
    // How far behind the target the camera sits
    [SerializeField] private float distance = 5f;
    // Vertical offset above the target's pivot (roughly head/shoulder height)
    [SerializeField] private float height = 1.5f;

    [Header("Rotation")]
    [SerializeField] private float mouseSensitivity = 50f;
    // Pitch limits in degrees: minY looks up from below, maxY looks down from above
    [SerializeField] private float minY = -30f;
    [SerializeField] private float maxY = 60f;

    [Header("Smoothness")]
    // Lower value = snappier camera, higher value = floatier camera
    [SerializeField] private float positionSmoothTime = 0.05f;
    [SerializeField] private float rotationSmoothTime = 0.05f;

    [Header("Focus Settings")]
    // Extra downward/upward tilt used when the camera is in "focus" (lock-on) mode
    [SerializeField] private float focusPitchOffset = 30f;

    // Raw (un-smoothed) rotation angles driven by player input
    [SerializeField] private float yaw;
    [SerializeField] private float pitch;

    // Smoothed angles that are actually applied to the camera transform
    private float currentYaw;
    private float currentPitch;

    // Velocity holders required by the SmoothDamp functions (they are updated internally)
    private float yawVelocity;
    private float pitchVelocity;

    private Vector3 positionVelocity;

    // Was the camera in focus mode last frame? Used to avoid a snap when leaving focus mode.
    private bool wasFocused;

    void Start()
    {
        // playerInputHandler = target.GetComponent<PlayerInputHandler>();
        Vector3 angles = transform.eulerAngles;

        // yaw = angles.y;
        // pitch = angles.x;

        // Start the smoothed angles at the serialized yaw/pitch so there is no initial swing
        currentYaw = yaw;
        currentPitch = pitch;

        characterMovement = target.GetComponent<PlayerMovement>();

        // Hide and lock the mouse cursor to the game window
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // bool isFocused = false;

        // NOTE: focus (lock-on) camera mode is currently disabled - "false" is always passed.
        // The focus branches below are kept so the feature can be re-enabled later.
        HandleFocusState(false);
        HandleRotation(false);
        HandlePosition();

        wasFocused = false;
    }

    /// <summary>
    /// When leaving focus mode, copies the camera's current angles into yaw/pitch
    /// so free-look continues from where the camera is (prevents a sudden snap).
    /// </summary>
    private void HandleFocusState(bool isFocused)
    {
        // Sync rotation when exiting focus (prevents snap)
        if (wasFocused && !isFocused)
        {
            Vector3 angles = transform.eulerAngles;

            yaw = angles.y;
            pitch = angles.x;

            currentYaw = yaw;
            currentPitch = pitch;
        }
    }

    /// <summary>
    /// Computes the desired camera angles (from input, or from the target when focused),
    /// smooths toward them, and applies the result to the camera rotation.
    /// </summary>
    private void HandleRotation(bool isFocused)
    {
        float targetYaw;
        float targetPitch;

        if (isFocused)
        {
            // Focus mode: match the target's facing direction, tilted by focusPitchOffset
            Vector3 targetEuler = target.eulerAngles;

            targetYaw = targetEuler.y;
            targetPitch = targetEuler.x + focusPitchOffset;
        }
        else
        {
            // Free-look mode: accumulate look input into yaw/pitch
            Vector2 lookDirection = playerInputHandler.lookDirection;

            yaw += lookDirection.x * mouseSensitivity * 10f * Time.deltaTime;;
            // Pitch is subtracted so moving the mouse up looks up
            pitch -= lookDirection.y * mouseSensitivity * 10f * Time.deltaTime;
            // Prevent the camera from flipping over the top/bottom
            pitch = Mathf.Clamp(pitch, minY, maxY);

            targetYaw = yaw;
            targetPitch = pitch;
        }

        // Proper angle smoothing (fixes 360 spin issue)
        // SmoothDampAngle takes the shortest path around the circle
        currentYaw = Mathf.SmoothDampAngle(
            currentYaw,
            targetYaw,
            ref yawVelocity,
            rotationSmoothTime
        );

        currentPitch = Mathf.SmoothDampAngle(
            currentPitch,
            targetPitch,
            ref pitchVelocity,
            rotationSmoothTime
        );

        // Roll (z) is always 0 so the camera never tilts sideways
        transform.eulerAngles = new Vector3(currentPitch, currentYaw, 0f);
    }

    /// <summary>
    /// Places the camera "distance" units behind the target (opposite of the camera's forward),
    /// raised by "height", and smoothly moves toward that position.
    /// </summary>
    private void HandlePosition()
    {
        Vector3 targetPosition =
            target.position +
            Vector3.up * height -
            transform.forward * distance;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref positionVelocity,
            positionSmoothTime
        );
    }
}
