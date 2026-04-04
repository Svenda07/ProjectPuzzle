using UnityEngine;
using System;
[RequireComponent(typeof(Rigidbody))]
public class DiceObject : MonoBehaviour
{
    [Header("Dice Faces")]
    [SerializeField] private Transform[] faceTransforms = new Transform[6];

    [Header("Roll Detection")]
    [SerializeField] private float minVelocityToBeRolling = 0.1f;
    [SerializeField] private float settledTimeRequired = 0.5f;

    private Rigidbody rb;
    private bool isCheckingRoll;
    private float settledTimer;

    public int CurrentValue { get; private set; } = 1;
    public bool IsRolling { get; private set; }

    public event Action<int> OnDiceValueFinalized;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!isCheckingRoll)
            return;

        float linearSpeed = rb.linearVelocity.magnitude;
        float angularSpeed = rb.angularVelocity.magnitude;

        if (linearSpeed <= minVelocityToBeRolling && angularSpeed <= minVelocityToBeRolling)
        {
            settledTimer += Time.deltaTime;

            if (settledTimer >= settledTimeRequired)
            {
                isCheckingRoll = false;
                IsRolling = false;
                CurrentValue = GetTopFaceValue();
                OnDiceValueFinalized?.Invoke(CurrentValue);
                Debug.Log(gameObject.name + " rolled: " + CurrentValue);
            }
        }
        else
        {
            settledTimer = 0f;
            IsRolling = true;
        }
    }

    public void BeginRollCheck()
    {
        isCheckingRoll = true;
        IsRolling = true;
        settledTimer = 0f;
    }

    public int GetTopFaceValue()
    {
        if (faceTransforms == null || faceTransforms.Length != 6)
        {
            Debug.LogWarning("DiceObject on " + gameObject.name + " does not have exactly 6 face transforms assigned.");
            return 1;
        }

        int topFaceIndex = 0;
        float highestY = faceTransforms[0].position.y;

        for (int i = 1; i < faceTransforms.Length; i++)
        {
            if (faceTransforms[i].position.y > highestY)
            {
                highestY = faceTransforms[i].position.y;
                topFaceIndex = i;
            }
        }

        return topFaceIndex + 1;
    }
}
