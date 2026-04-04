using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintMultiplier = 2.0f;

    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float gravityMultiplier = 1.0f;

    [Header("Look Parameters")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float upDownLookRange = 80f;

    [Header("Crouch Parameters")]
    [SerializeField] private float crouchHeight = 1.0f;
    [SerializeField] private float standingHeight = 2.0f;
    [SerializeField] private float crouchSpeedMultiplier = 0.5f;
    [SerializeField] private float crouchTransitionSpeed = 8.0f;
    [SerializeField] private Vector3 crouchingCameraOffset = new Vector3(0, -0.5f, 0);

    [Header("Dash Parameters")]
    [SerializeField] private float dashSpeed = 12f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    [Header("Air Control")]
    [SerializeField] private float airControl = 2f;

    [Header("Slope Slide")]
    [SerializeField] private float maxWalkableSlopeAngle = 35f;
    [SerializeField] private float slopeSlideSpeed = 8f;

    [Header("Grapple")]
    [SerializeField] private LayerMask grappableLayer;
    private int grappleBlockerLayer => ~LayerMask.GetMask("Player");
    [SerializeField] private float maxGrappleDistance = 20f;
    [SerializeField] private float grapplePullSpeed = 20f;
    [SerializeField] private Transform grappleOrigin;
    [SerializeField] private LineRenderer grappleLine;

    [Header("Reticle")]
    [SerializeField] private GameObject normalReticle;
    [SerializeField] private GameObject validGrappleReticle;

    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerInputHandler playerInputHandler;
    
    [Header("Grapple Momentum")]
    [SerializeField] private float grappleMomentumSmoothTime = 0.4f;

    private Vector3 grappleReleaseMomentum;
    private float grappleMomentumXSmoothing;
    private float grappleMomentumZSmoothing;




    private Vector3 currentMovement;
    private float verticalRotation;
    private Vector3 standingCameraLocalPosition;

    private bool isCrouching = false;
    private float targetHeight;
    private Vector3 targetCameraLocalPosition;

    private bool isDashing;
    private float dashTimer;
    private float dashCooldownTimer;
    private Vector3 dashDirection;

    private RaycastHit slopeHit;
    private bool isSlidingOnSlope;


    private bool isPullGrappling;
    private Vector3 grapplePoint;
    private Vector3 grappleVelocity;
    private float CurrentSpeed
    {
        get
        {
            float speed = walkSpeed;

            if (AbilityManager.Instance != null &&
                AbilityManager.Instance.SprintUnlocked &&
                playerInputHandler.SprintTriggered &&
                !isCrouching)
            {
                speed *= sprintMultiplier;
            }

            if (isCrouching)
            {
                speed *= crouchSpeedMultiplier;
            }

            return speed;
        }
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        standingHeight = characterController.height;
        standingCameraLocalPosition = mainCamera.transform.localPosition;

        targetHeight = standingHeight;
        targetCameraLocalPosition = standingCameraLocalPosition;
    }

    void Update()
    {
      
        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        HandleDash();
        HandleCrouch();
        HandleMovement();
        HandleRotation();
        HandleGrapple();
        UpdateGrappleLine(); 
        UpdateReticle();
    }

    private Vector3 CalculateWorldDirection()
    {
        Vector3 inputDirection = new Vector3(playerInputHandler.MovementInput.x, 0f, playerInputHandler.MovementInput.y);
        Vector3 worldDirection = transform.TransformDirection(inputDirection);
        return worldDirection.normalized;
    }
    private void UpdateReticle()
    {
        if (normalReticle == null || validGrappleReticle == null)
            return;

        bool validTarget = false;

        if (Physics.Raycast(
            mainCamera.transform.position,
            mainCamera.transform.forward,
            out RaycastHit hit,
            maxGrappleDistance,
            grappleBlockerLayer,
            QueryTriggerInteraction.Ignore))
        {
            validTarget = ((1 << hit.collider.gameObject.layer) & grappableLayer) != 0;
        }

        normalReticle.SetActive(!validTarget);
        validGrappleReticle.SetActive(validTarget);
    }
    private void HandleJumping()
    {
        if (characterController.isGrounded)
        {
            currentMovement.y = -0.5f;

            if (AbilityManager.Instance != null &&
                AbilityManager.Instance.JumpUnlocked &&
                playerInputHandler.JumpTriggered &&
                !isCrouching &&
                !isSlidingOnSlope)
            {
                currentMovement.y = jumpForce;
            }
        }
        else
        {
            currentMovement.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
        }
    }

    private void HandleMovement()
    {
        Vector3 worldDirection = CalculateWorldDirection();
        Vector3 targetHorizontalMovement = worldDirection * CurrentSpeed;

        isSlidingOnSlope = OnSteepSlope();

        if (isDashing)
        {
            currentMovement.x = dashDirection.x * dashSpeed;
            currentMovement.z = dashDirection.z * dashSpeed;
        }
        else if (isPullGrappling)
        {
            currentMovement.x = grappleVelocity.x;
            currentMovement.z = grappleVelocity.z;
        }
        else if (isSlidingOnSlope)
        {
            Vector3 slideDirection = GetSlopeSlideDirection();
            currentMovement.x = slideDirection.x * slopeSlideSpeed;
            currentMovement.z = slideDirection.z * slopeSlideSpeed;
        }
        else if (characterController.isGrounded)
        {
            currentMovement.x = targetHorizontalMovement.x;
            currentMovement.z = targetHorizontalMovement.z;
        }
        else
        {
            currentMovement.x = Mathf.Lerp(
                currentMovement.x,
                targetHorizontalMovement.x + grappleReleaseMomentum.x,
                airControl * Time.deltaTime
            );

            currentMovement.z = Mathf.Lerp(
                currentMovement.z,
                targetHorizontalMovement.z + grappleReleaseMomentum.z,
                airControl * Time.deltaTime
            );
        }

        grappleReleaseMomentum.x = Mathf.SmoothDamp(
            grappleReleaseMomentum.x,
            0f,
            ref grappleMomentumXSmoothing,
            grappleMomentumSmoothTime
        );

        grappleReleaseMomentum.z = Mathf.SmoothDamp(
            grappleReleaseMomentum.z,
            0f,
            ref grappleMomentumZSmoothing,
            grappleMomentumSmoothTime
        );

        HandleJumping();
        characterController.Move(currentMovement * Time.deltaTime);
    }

    private void HandleCrouch()
    {
        if (AbilityManager.Instance == null || !AbilityManager.Instance.CrouchUnlocked)
        {
            isCrouching = false;
            targetHeight = standingHeight;
            targetCameraLocalPosition = standingCameraLocalPosition;
        }
        else if (playerInputHandler.CrouchTriggered)
        {
            isCrouching = true;
            targetHeight = crouchHeight;
            targetCameraLocalPosition = standingCameraLocalPosition + crouchingCameraOffset;
        }
        else
        {
            if (!CheckCeilingAbove())
            {
                isCrouching = false;
                targetHeight = standingHeight;
                targetCameraLocalPosition = standingCameraLocalPosition;
            }
            else
            {
                isCrouching = true;
                targetHeight = crouchHeight;
                targetCameraLocalPosition = standingCameraLocalPosition + crouchingCameraOffset;
            }
        }

        float newHeight = Mathf.Lerp(characterController.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);
        float heightDifference = characterController.height - newHeight;
        characterController.height = newHeight;

        Vector3 center = characterController.center;
        center.y -= heightDifference / 2f;
        characterController.center = center;

        mainCamera.transform.localPosition = Vector3.Lerp(
            mainCamera.transform.localPosition,
            targetCameraLocalPosition,
            Time.deltaTime * crouchTransitionSpeed
        );
    }

    private bool CheckCeilingAbove()
    {
        float radius = characterController.radius * 0.95f;

        Vector3 worldCenter = transform.TransformPoint(characterController.center);

        float bottomY = worldCenter.y - (characterController.height / 2f) + radius;
        Vector3 bottom = new Vector3(worldCenter.x, bottomY, worldCenter.z);

        Vector3 top = bottom + Vector3.up * (standingHeight - radius * 2f);

        int layerMask = ~LayerMask.GetMask("Player");

        return Physics.CheckCapsule(
            bottom,
            top,
            radius,
            layerMask,
            QueryTriggerInteraction.Ignore
        );
    }

    private bool OnSteepSlope()
    {
        if (characterController.isGrounded &&
            Physics.Raycast(transform.position, Vector3.down, out slopeHit, characterController.height / 2f + 0.5f))
        {
            float slopeAngle = Vector3.Angle(slopeHit.normal, Vector3.up);
            return slopeAngle > maxWalkableSlopeAngle;
        }

        return false;
    }

    private Vector3 GetSlopeSlideDirection()
    {
        return Vector3.ProjectOnPlane(Vector3.down, slopeHit.normal).normalized;
    }

    private void HandleDash()
    {
        if (AbilityManager.Instance == null || !AbilityManager.Instance.DashUnlocked)
            return;

        if (!isDashing && dashCooldownTimer <= 0f && playerInputHandler.DashTriggered)
        {
            StopGrapple(false);

            grappleReleaseMomentum = Vector3.zero;
            grappleVelocity = Vector3.zero;
            grappleMomentumXSmoothing = 0f;
            grappleMomentumZSmoothing = 0f;

            isDashing = true;
            dashTimer = dashDuration;
            dashCooldownTimer = dashCooldown;

            dashDirection = mainCamera.transform.forward;
            dashDirection.y = 0f;
            dashDirection.Normalize();
        }

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;

            if (dashTimer <= 0f)
            {
                isDashing = false;
            }
        }
    }
    private void HandleGrapple()
    {
        if (AbilityManager.Instance == null || !AbilityManager.Instance.GrappleUnlocked)
            return;

        if (playerInputHandler.GrapplePullTriggered && !isPullGrappling)
        {
            StartPullGrapple();
        }

        if (!playerInputHandler.GrapplePullTriggered && isPullGrappling)
        {
            StopGrapple();
        }

        if (isPullGrappling)
        {
            Vector3 directionToPoint = (grapplePoint - transform.position).normalized;
            grappleVelocity = directionToPoint * grapplePullSpeed;

            characterController.Move(grappleVelocity * Time.deltaTime);

            if (Vector3.Distance(transform.position, grapplePoint) < 2f)
            {
                StopGrapple();
            }
        }
    }

    private void StartPullGrapple()
    {
        if (Physics.Raycast(
            mainCamera.transform.position,
            mainCamera.transform.forward,
            out RaycastHit hit,
            maxGrappleDistance,
            grappleBlockerLayer,
            QueryTriggerInteraction.Ignore))
        {
            if (((1 << hit.collider.gameObject.layer) & grappableLayer) != 0)
            {
                grapplePoint = hit.point;
                isPullGrappling = true;
                grappleVelocity = Vector3.zero;

                if (grappleLine != null)
                {
                    grappleLine.enabled = true;
                }
            }
        }
    }

    private void StopGrapple(bool preserveMomentum = true)
    {
        isPullGrappling = false;

        if (preserveMomentum)
        {
            grappleReleaseMomentum = new Vector3(grappleVelocity.x, 0f, grappleVelocity.z);
        }
        else
        {
            grappleReleaseMomentum = Vector3.zero;
            grappleVelocity = Vector3.zero;
            grappleMomentumXSmoothing = 0f;
            grappleMomentumZSmoothing = 0f;
        }

        if (grappleLine != null)
        {
            grappleLine.enabled = false;
        }
    }
    private void UpdateGrappleLine()
    {
        if (grappleLine == null)
            return;

        if (!isPullGrappling)
        {
            grappleLine.enabled = false;
            return;
        }

        Vector3 startPoint = grappleOrigin.position;
        Vector3 direction = (grapplePoint - startPoint).normalized;
        float actualDistance = Vector3.Distance(startPoint, grapplePoint);
        float clampedDistance = Mathf.Min(actualDistance, maxGrappleDistance);
        Vector3 endPoint = startPoint + direction * clampedDistance;

        grappleLine.enabled = true;
        grappleLine.SetPosition(0, startPoint);
        grappleLine.SetPosition(1, endPoint);
    }
    private void ApplyHorizontalRotation(float rotationAmount)
    {
        transform.Rotate(0, rotationAmount, 0);
    }

    private void ApplyVerticalRotation(float rotationAmount)
    {
        verticalRotation = Mathf.Clamp(verticalRotation - rotationAmount, -upDownLookRange, upDownLookRange);
        mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    private void HandleRotation()
    {
        float mouseXRotation = playerInputHandler.RotationInput.x * mouseSensitivity;
        float mouseYRotation = playerInputHandler.RotationInput.y * mouseSensitivity;

        ApplyHorizontalRotation(mouseXRotation);
        ApplyVerticalRotation(mouseYRotation);
    }
}