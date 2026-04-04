using System;
using UnityEngine;

public class PlayerObjectInteraction : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private PlayerInputHandler inputHandler;
    [SerializeField] private Transform holdPoint;
    [SerializeField] private Camera playerCamera;

    [Header("Detection")]
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private LayerMask pickupLayer;

    [Header("Push")]
    [SerializeField] private float pushForce = 2f;
    [SerializeField] private float noTipMassThreshold = 20f;

    [Header("Carry")]
    [SerializeField] private float carryMoveSpeed = 18f;
    [SerializeField] private float maxCarryVelocity = 12f;
    [SerializeField] private float carryRotateSpeed = 15f;
    [SerializeField] private float stopDistance = 0.08f;

    [Header("Throw")]
    [SerializeField] private float throwForce = 18f;
    [SerializeField] private float upwardThrowForce = 0.02f;
    [Header("Dice Throw")]
    [SerializeField] private float minDiceSpinForce = 12f;
    [SerializeField] private float maxDiceSpinForce = 20f;
    private Pickupobject currentTarget;
    private Pickupobject heldObject;
    private Rigidbody heldRb;

    private void Update()
    {
        if (AbilityManager.Instance == null || !AbilityManager.Instance.objectPickupUnlocked)
            return;

        FindTarget();

        if (inputHandler.ResetPickupsTriggered)
        {
            ResetAllPickupObjects();
            return;
        }

        if (inputHandler.ObjectPickupTriggered)
        {
            if (heldObject == null)
                TryPickup();
            else
                DropHeldObject();
        }

        if (inputHandler.ObjectThrowTriggered && heldObject != null)
        {
            TryThrow();
        }
    }

    private void FixedUpdate()
    {
        if (heldObject != null && heldRb != null)
            MoveHeldObject();
    }

    private void FindTarget()
    {
        currentTarget = null;

        if (playerCamera == null)
            return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, pickupLayer))
        {
            currentTarget = hit.collider.GetComponentInParent<Pickupobject>();
        }
    }

    private void TryPickup()
    {
        if (currentTarget == null)
            return;

        if (!currentTarget.CanBeCarried)
            return;

        heldObject = currentTarget;
        heldRb = heldObject.RB;

        if (heldRb == null)
        {
            heldObject = null;
            return;
        }

        heldRb.useGravity = false;
        heldRb.linearVelocity = Vector3.zero;
        heldRb.angularVelocity = Vector3.zero;
        heldRb.linearDamping = 8f;
        heldRb.angularDamping = 12f;
        heldRb.constraints = RigidbodyConstraints.FreezeRotation;
        heldRb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void MoveHeldObject()
    {
        Vector3 targetPosition = holdPoint.position;
        Vector3 toTarget = targetPosition - heldRb.position;
        float distance = toTarget.magnitude;

        if (distance < stopDistance)
        {
            heldRb.linearVelocity = Vector3.zero;
        }
        else
        {
            Vector3 desiredVelocity = toTarget * carryMoveSpeed;
            desiredVelocity = Vector3.ClampMagnitude(desiredVelocity, maxCarryVelocity);
            heldRb.linearVelocity = desiredVelocity;
        }

        Quaternion targetRotation = holdPoint.rotation;
        Quaternion newRotation = Quaternion.Slerp(
            heldRb.rotation,
            targetRotation,
            carryRotateSpeed * Time.fixedDeltaTime
        );

        heldRb.MoveRotation(newRotation);
    }

    private void TryThrow()
    {
        if (heldObject == null || heldRb == null)
            return;

        if (!heldObject.CanBeThrown)
            return;

        Rigidbody rbToThrow = heldRb;
        GameObject thrownObject = heldObject.gameObject;

        DropHeldObject();

        Vector3 throwDirection = playerCamera.transform.forward + (playerCamera.transform.up * upwardThrowForce);

        rbToThrow.linearVelocity = Vector3.zero;
        rbToThrow.angularVelocity = Vector3.zero;

        rbToThrow.AddForce(throwDirection.normalized * throwForce, ForceMode.VelocityChange);

        if (thrownObject.CompareTag("Dice"))
        {
            Vector3 randomSpin =
                new Vector3(
                    UnityEngine.Random.Range(minDiceSpinForce, maxDiceSpinForce),
                    UnityEngine.Random.Range(minDiceSpinForce, maxDiceSpinForce),
                    UnityEngine.Random.Range(minDiceSpinForce, maxDiceSpinForce)
                );

            randomSpin.x *= UnityEngine.Random.value < 0.5f ? -1f : 1f;
            randomSpin.y *= UnityEngine.Random.value < 0.5f ? -1f : 1f;
            randomSpin.z *= UnityEngine.Random.value < 0.5f ? -1f : 1f;

            rbToThrow.AddTorque(randomSpin, ForceMode.VelocityChange);

            DiceObject dice = thrownObject.GetComponent<DiceObject>();
            if (dice != null)
            {
                dice.BeginRollCheck();
            }
        }
    }
    private void DropHeldObject()
    {
        if (heldRb == null)
            return;

        Rigidbody rbToRestore = heldRb;

        heldRb.useGravity = true;
        heldRb.linearVelocity = Vector3.zero;
        heldRb.angularVelocity = Vector3.zero;
        heldRb.linearDamping = 0f;
        heldRb.angularDamping = 0.05f;

        if (rbToRestore.mass > noTipMassThreshold)
            rbToRestore.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        else
            rbToRestore.constraints = RigidbodyConstraints.None;

        heldObject = null;
        heldRb = null;
    }

    private void ResetAllPickupObjects()
    {
        if (heldObject != null)
            DropHeldObject();

        Pickupobject[] allPickupObjects = FindObjectsByType<Pickupobject>();
        foreach (Pickupobject pickupObject in allPickupObjects)
        {
            pickupObject.ResetToStart();
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (AbilityManager.Instance == null || !AbilityManager.Instance.objectPickupUnlocked)
            return;

        if (((1 << hit.gameObject.layer) & pickupLayer) == 0)
            return;

        Pickupobject pickupObject = hit.collider.GetComponentInParent<Pickupobject>();
        if (pickupObject == null || !pickupObject.CanBePushed || heldObject != null)
            return;

        Rigidbody rb = pickupObject.RB;
        if (rb == null || rb.isKinematic)
            return;

        if (hit.moveDirection.y < -0.3f)
            return;

        if (rb.mass > noTipMassThreshold)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        Vector3 pushDirection = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);
        rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
    }
}
