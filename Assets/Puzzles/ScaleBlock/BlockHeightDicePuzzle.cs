using UnityEngine;
using TMPro;

public class BlockHeightDicePuzzle : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform targetBlockVisual;
    [SerializeField] private TMP_Text displayText;

    [Header("Dice Settings")]
    [SerializeField] private string diceTag = "Dice";

    [Header("Block Height")]
    [SerializeField] private float baseHeight = 1f;
    [SerializeField] private bool keepBottomAnchored = true;

    private DiceObject currentDiceInZone;
    private Vector3 originalLocalScale;

    private void Start()
    {
        if (targetBlockVisual != null)
        {
            originalLocalScale = targetBlockVisual.localScale;
        }

        UpdateDisplay(1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(diceTag))
            return;

        DiceObject dice = other.GetComponent<DiceObject>();
        if (dice == null)
            return;

        if (currentDiceInZone != null)
            currentDiceInZone.OnDiceValueFinalized -= HandleDiceValueFinalized;

        currentDiceInZone = dice;
        currentDiceInZone.OnDiceValueFinalized += HandleDiceValueFinalized;

        if (!dice.IsRolling)
        {
            HandleDiceValueFinalized(dice.CurrentValue);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(diceTag))
            return;

        DiceObject dice = other.GetComponent<DiceObject>();
        if (dice == null)
            return;

        if (currentDiceInZone == dice)
        {
            currentDiceInZone.OnDiceValueFinalized -= HandleDiceValueFinalized;
            currentDiceInZone = null;
        }
    }

    private void HandleDiceValueFinalized(int value)
    {
        SetBlockHeight(value);
        UpdateDisplay(value);
    }

    private void SetBlockHeight(int value)
    {
        if (targetBlockVisual == null)
            return;

        float newHeight = baseHeight * value;

        Vector3 newScale = originalLocalScale;
        newScale.y = newHeight;
        targetBlockVisual.localScale = newScale;

        if (keepBottomAnchored)
        {
            Vector3 newLocalPosition = targetBlockVisual.localPosition;
            newLocalPosition.y = newHeight * 0.5f;
            targetBlockVisual.localPosition = newLocalPosition;
        }
    }

    private void UpdateDisplay(int value)
    {
        if (displayText != null)
        {
            displayText.text = value.ToString();
        }
    }
}