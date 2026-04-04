using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset playerControls;


    [Header("Action Map Name Reference")]
    [SerializeField] private string actionMapName = "Player";


    [Header("Action Name References")]
    [SerializeField] private string movement = "Movement";
    [SerializeField] private string rotation = "Rotation";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string sprint = "Sprint";
    [SerializeField] private string crouch = "Crouch";
    [SerializeField] private string dash = "Dash";
    [SerializeField] private string grapplePull = "GrapplePull";
    [SerializeField] private string objectPickup = "PickUp";
    [SerializeField] private string objectThrow = "Throw";
    [SerializeField] private string resetPickups = "ResetPickups";

    private InputAction movementAction;
    private InputAction rotationAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction crouchAction;
    private InputAction dashAction;
    private InputAction grapplePullAction;
    private InputAction objectPickupAction;
    private InputAction objectThrowAction;
    private InputAction resetPickupsAction;

    public Vector2 MovementInput { get; private set; }
    public Vector2 RotationInput { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool SprintTriggered { get; private set; }
    public bool CrouchTriggered { get; private set; }
    public bool DashTriggered { get; private set; }
    public bool GrapplePullTriggered { get; private set; }
    public bool JumpHeld { get; private set; }

    public bool ObjectPickupTriggered { get; private set; }
    public bool ObjectThrowTriggered { get; private set; }
    public bool ResetPickupsTriggered { get; private set; }


    private void Awake()
    {
        InputActionMap mapReference = playerControls.FindActionMap(actionMapName);


        movementAction = mapReference.FindAction(movement);
        rotationAction = mapReference.FindAction(rotation);
        jumpAction = mapReference.FindAction(jump);
        sprintAction = mapReference.FindAction(sprint);
        crouchAction = mapReference.FindAction(crouch);
        dashAction = mapReference.FindAction(dash);
        grapplePullAction = mapReference.FindAction(grapplePull);
        objectPickupAction = mapReference.FindAction(objectPickup);
        objectThrowAction = mapReference.FindAction(objectThrow);
        resetPickupsAction = mapReference.FindAction(resetPickups);





        SubscribeActionValuesToInputEvents();
    }


    private void SubscribeActionValuesToInputEvents()
    {
        movementAction.performed += inputInfo => MovementInput = inputInfo.ReadValue<Vector2>();
        movementAction.canceled += inputInfo => MovementInput = Vector2.zero;


        rotationAction.performed += inputInfo => RotationInput = inputInfo.ReadValue<Vector2>();
        rotationAction.canceled += inputInfo => RotationInput = Vector2.zero;


        jumpAction.performed += inputInfo =>
        {
            JumpTriggered = true;
            JumpHeld = true;
        };

        jumpAction.canceled += inputInfo =>
        {
            JumpHeld = false;
        };


        sprintAction.performed += inputInfo => SprintTriggered = true;
        sprintAction.canceled += inputInfo => SprintTriggered = false;
        
        crouchAction.performed += inputInfo => CrouchTriggered = true;
        crouchAction.canceled += inputInfo => CrouchTriggered = false;
          
        dashAction.performed += inputInfo => DashTriggered = true;
        dashAction.canceled += inputInfo => DashTriggered = false;
        
        grapplePullAction.performed += inputInfo => GrapplePullTriggered = true;
        grapplePullAction.canceled += inputInfo => GrapplePullTriggered = false;

        objectPickupAction.performed += inputInfo => ObjectPickupTriggered = true;
        objectThrowAction.performed += inputInfo => ObjectThrowTriggered = true;
        resetPickupsAction.performed += inputInfo => ResetPickupsTriggered = true;
    }
    public void ClearFrameInput()
    {
        JumpTriggered = false;
    }
    private void LateUpdate()
    {
        ObjectPickupTriggered = false;
        ObjectThrowTriggered = false;
        ResetPickupsTriggered = false;
    }
    private void OnEnable()
    {
        playerControls.FindActionMap(actionMapName).Enable();
    }


    private void OnDisable()
    {
        playerControls.FindActionMap(actionMapName).Disable();
    }
}
