using UnityEngine;
using TMPro;

public class CaesarCipher : MonoBehaviour
{
    [Header("Puzzle Text")]
    [SerializeField] private TMP_Text displayText;
    [SerializeField] private string plainMessage = "Mozda je \\nnesto na \\nventilima?";
    [SerializeField] private bool showShiftValue = true;

    [Header("Dice Settings")]
    [SerializeField] private string diceTag = "Dice";

    [Header("Objects To Reveal")]
    [SerializeField] private GameObject objectToSpawn1;
    [SerializeField] private GameObject objectToSpawn2;
    [SerializeField] private GameObject objectToSpawn3;
    [SerializeField] private GameObject objectToSpawn4;
    

    private DiceObject currentDiceInZone;
    private int encodedShift = 1;
    private string encodedMessage;
    private bool puzzleSolved = false;

    private void Start()
    {
        encodedShift = Random.Range(2, 7);
        encodedMessage = CaesarEncrypt(plainMessage, encodedShift);

        if (objectToSpawn1 != null)
            objectToSpawn1.SetActive(false);

        if (objectToSpawn2 != null)
            objectToSpawn2.SetActive(false);
        if (objectToSpawn3 != null)
            objectToSpawn3.SetActive(false);
        if (objectToSpawn4 != null)
            objectToSpawn4.SetActive(false);


        if (displayText != null)
        {
            if (showShiftValue)
                displayText.text = encodedMessage + "\n\n?";
            else
                displayText.text = encodedMessage;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(diceTag))
            return;

        DiceObject dice = other.GetComponent<DiceObject>();
        if (dice == null)
            return;

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

    private void HandleDiceValueFinalized(int shift)
    {
        if (displayText == null)
            return;

        if (shift == encodedShift)
        {
            puzzleSolved = true;

            string decrypted = CaesarDecrypt(encodedMessage, shift);

            if (showShiftValue)
                displayText.text = decrypted + "\n\nBroj: " + shift;
            else
                displayText.text = decrypted;

            if (objectToSpawn1 != null)
                objectToSpawn1.SetActive(true);

            if (objectToSpawn2 != null)
                objectToSpawn2.SetActive(true);
            if (objectToSpawn3 != null)
                objectToSpawn3.SetActive(true);
            if (objectToSpawn4 != null)
                objectToSpawn4.SetActive(true);



        }
        else
        {
            if (!puzzleSolved)
            {
                if (showShiftValue)
                    displayText.text = encodedMessage + "\n\nBroj: " + shift;
                else
                    displayText.text = encodedMessage;
            }
        }
    }

    private string CaesarEncrypt(string input, int shift)
    {
        shift = shift % 26;
        char[] chars = input.ToCharArray();

        for (int i = 0; i < chars.Length; i++)
        {
            char c = chars[i];

            if (c >= 'A' && c <= 'Z')
                chars[i] = (char)('A' + ((c - 'A' + shift) % 26));
            else if (c >= 'a' && c <= 'z')
                chars[i] = (char)('a' + ((c - 'a' + shift) % 26));
        }

        return new string(chars);
    }

    private string CaesarDecrypt(string input, int shift)
    {
        return CaesarEncrypt(input, 26 - (shift % 26));
    }
}