using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class Pickupobject : MonoBehaviour
{
    [Header("Weight Settings")]
    [SerializeField] private float objectWeight = 5f;
    [SerializeField] private float maxCarryWeight = 15f;
    [SerializeField] private float maxThrowWeight = 10f;

    [Header("Movement Settings")]
    [SerializeField] private bool canBePushed = true;
    [SerializeField] private bool canBePickedUp = true;
    [SerializeField] private float noTipMassThreshold = 20f;

    private Rigidbody rb;
    private Vector3 startPosition;
    private Quaternion startRotation;

    public float ObjectWeight => objectWeight;
    public bool CanBePushed => canBePushed;
    public bool CanBePickedUp => canBePickedUp;
    public bool CanBeCarried => canBePickedUp && objectWeight <= maxCarryWeight;
    public bool CanBeThrown => objectWeight <= maxThrowWeight;
    public Rigidbody RB => rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        startRotation = transform.rotation;

        ApplyHeavyObjectConstraints();
    }

    private void ApplyHeavyObjectConstraints()
    {
        if (rb == null)
            return;

        if (rb.mass > noTipMassThreshold)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.None;
        }
    }

    public void ResetToStart()
    {
        if (rb == null)
            return;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        transform.position = startPosition;
        transform.rotation = startRotation;

        ApplyHeavyObjectConstraints();
    }
}

