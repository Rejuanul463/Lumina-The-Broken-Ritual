using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Single place where all player input is read and stored as simple public values
/// (moveDirection, jump, attack, sprint, parry, ...). Other scripts (PlayerMovement,
/// PlayerCombat, CameraFollow) only read these values and never touch input directly.
///
/// Supports two input sources:
///  - PC: Unity Input System actions assigned in the inspector.
///  - Android: on-screen joystick and UI buttons (enabled with the isAndroid flag for movement;
///    the button listeners are always registered in Start).
/// </summary>
public class PlayerInputHandler : MonoBehaviour
{
    // When true, movement comes from the on-screen joystick instead of the Input System
    public bool isAndroid;
    private static PlayerController playerController;

    // Event raised when the unlock button is pressed; OpenCage listens to it
    public static Action OpenCageGate;

    [Header("Input Actions")]
    public InputActionReference Move;
    // public InputActionReference Fire;
    public InputActionReference Jump;
    public InputActionReference Crouch;
    public InputActionReference Sprint;
    public  InputActionReference look;
    // Right mouse button = parry/block
    public InputActionReference rightClick;

    [Header("Input Values")]
    // Movement input in the XZ plane (x = left/right, z = forward/back), normalized
    public Vector3 moveDirection;
    // Camera look input (read by CameraFollow)
    public Vector2 lookDirection;
    // True while sprint is held
    public bool sprint;

    // One-frame triggers
    // (set to true when the button is pressed and reset to false at the end of the frame)
    public bool jump;
    public bool attack;
    public bool crouch;
    // True while the parry button is held
    public bool parry;

    // Mobile on-screen controls
    public Joystick joystick;
    public Button Attack;
    public HoldButton Block;
    public HoldButton Run;
    public Button hop;

    // Button shown near the cage to open it
    public Button unlock;
    private void Start()
    {
        playerController = GetComponent<PlayerController>();

        // Hook up the on-screen (mobile) buttons
        hop.onClick.AddListener(() => OnJumpAndroid());

        Attack.onClick.AddListener(()=> OnFireAndroid());

        Block.PointerDown += OnParryStartedAndroid;
        Block.PointerUp += OnParryEndedAndroid;

        Run.PointerDown += OnSprintStartedAndroid;
        Run.PointerUp += OnSprintCanceledAndroid;

        unlock.onClick.AddListener(() => UnlockCage());
    }

    // Notify any listening cage (OpenCage) that the player wants to open it
    private void UnlockCage()
    {
        OpenCageGate?.Invoke();
    }


    // Enable the input actions and subscribe to their events when this component is enabled
    private void OnEnable()
    {
        Move.action.Enable();
        // Fire.action.Enable();
        Jump.action.Enable();
        Crouch.action.Enable();
        Sprint.action.Enable();
        rightClick.action.Enable();

        // Fire.action.performed += OnFire;
        Jump.action.performed += OnJump;
        rightClick.action.started += OnParryStarted;
        rightClick.action.canceled += OnParryEnded;
        Sprint.action.started += OnSprintStarted;
        Sprint.action.canceled += OnSprintCanceled;

        Crouch.action.performed += OnCrouchStarted;
        // Crouch.action.canceled += OnCrouchCanceled;
    }

    // Always unsubscribe when disabled to avoid duplicate/leaked callbacks
    private void OnDisable()
    {
        rightClick.action.started -= OnParryStarted;
        rightClick.action.canceled -= OnParryEnded;

        // Fire.action.performed -= OnFire;
        Jump.action.performed -= OnJump;

        Sprint.action.started -= OnSprintStarted;
        Sprint.action.canceled -= OnSprintCanceled;

        Crouch.action.performed -= OnCrouchStarted;
        // Crouch.action.canceled -= OnCrouchCanceled;

        Move.action.Disable();
        // Fire.action.Disable();
        Jump.action.Disable();
        Crouch.action.Disable();
        Sprint.action.Disable();
    }

    private void Update()
    {
        // Continuous movement input
        Vector2 moveInput = Move.action.ReadValue<Vector2>();
        // On mobile use the joystick instead of the keyboard/gamepad value
        if(isAndroid) moveInput = new Vector2(joystick.Horizontal, joystick.Vertical);
        moveDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

        // lookDirection = look.action.ReadValue<Vector2>();

        // Reset one-frame triggers
        jump = false;
        attack = false;
    }

    private void LateUpdate()
    {
        // Reset triggers after everyone has had a chance to read them this frame.
        jump = false;
        attack = false;
        crouch = false;
    }

    // ---- PC input callbacks (Input System) ----
    private void OnJump(InputAction.CallbackContext context)
    {
        jump = true;
    }

    // Attack pressed: enter combat state and lock on to the nearest enemy.
    // (Currently unused on PC because the Fire action subscription is commented out.)
    private void OnFire(InputAction.CallbackContext context)
    {
        attack = true;
        playerController.currentState = PlayerState.CombatState;
        playerController.SelectEnemy();
    }

    private void OnSprintStarted(InputAction.CallbackContext context)
    {
        sprint = true;
    }

    private void OnSprintCanceled(InputAction.CallbackContext context)
    {
        sprint = false;
    }

    private void OnCrouchStarted(InputAction.CallbackContext context)
    {
        crouch = true;
    }

    // Right mouse button pressed: start parrying and switch to combat state
    private void OnParryStarted(InputAction.CallbackContext context)
    {
        parry = true;
        playerController.currentState = PlayerState.CombatState;
    }

    private void OnParryEnded(InputAction.CallbackContext context)
    {
        parry = false;
    }

    // ---- Mobile (on-screen button) callbacks: same behaviour as the PC versions above ----
    private void OnParryStartedAndroid()
    {
        parry = true;
        playerController.currentState = PlayerState.CombatState;
    }

    private void OnParryEndedAndroid()
    {
        parry = false;
    }

    private void OnSprintStartedAndroid()
    {
        sprint = true;
    }

    private void OnSprintCanceledAndroid()
    {
        sprint = false;
    }

    private void OnJumpAndroid()
    {
        jump = true;
    }

    // Attack button: enter combat state, lock on to nearest enemy, and register the attack press
    private void OnFireAndroid()
    {
        attack = true;
        playerController.currentState = PlayerState.CombatState;
        playerController.SelectEnemy();
    }
}
