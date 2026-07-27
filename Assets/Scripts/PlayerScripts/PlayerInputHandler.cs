using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerInputHandler : MonoBehaviour
{
    public bool isAndroid;
    private static PlayerController playerController;
    
    public static Action OpenCageGate;
   
    [Header("Input Actions")]
    public InputActionReference Move;
    // public InputActionReference Fire;
    public InputActionReference Jump;
    public InputActionReference Crouch;
    public InputActionReference Sprint;
    public  InputActionReference look;
    public InputActionReference rightClick;
    
    [Header("Input Values")]
    public Vector3 moveDirection;
    public Vector2 lookDirection;
    public bool sprint;

    // One-frame triggers
    public bool jump;
    public bool attack;
    public bool crouch;
    public bool parry;
    
    public Joystick joystick;
    public Button Attack;
    public HoldButton Block;
    public HoldButton Run;
    public Button hop;

    public Button unlock;
    private void Start()
    {
        playerController = GetComponent<PlayerController>();

        hop.onClick.AddListener(() => OnJumpAndroid());
        
        Attack.onClick.AddListener(()=> OnFireAndroid());

        Block.PointerDown += OnParryStartedAndroid;
        Block.PointerUp += OnParryEndedAndroid;

        Run.PointerDown += OnSprintStartedAndroid;
        Run.PointerUp += OnSprintCanceledAndroid;
        
        unlock.onClick.AddListener(() => UnlockCage());
    }

    private void UnlockCage()
    {
        OpenCageGate?.Invoke();
    }
    
    
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

    private void OnJump(InputAction.CallbackContext context)
    {
        jump = true;
    }

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

    private void OnParryStarted(InputAction.CallbackContext context)
    {
        parry = true;
        playerController.currentState = PlayerState.CombatState;
    }

    private void OnParryEnded(InputAction.CallbackContext context)
    {
        parry = false;
    }
    
    
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

    private void OnFireAndroid()
    {
        attack = true;
        playerController.currentState = PlayerState.CombatState;
        playerController.SelectEnemy();
    }
}