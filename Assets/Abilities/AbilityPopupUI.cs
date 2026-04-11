
using System.Collections;
using TMPro;
using UnityEngine;

public class AbilityPopupUI : MonoBehaviour
{
    public static AbilityPopupUI Instance { get; private set; }

    [SerializeField] private GameObject popupRoot;
    [SerializeField] private TextMeshProUGUI abilityNameText;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private float popupDuration = 3f;

    private Coroutine popupCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (popupRoot != null)
            popupRoot.SetActive(false);
    }

    public void ShowAbilityPopup(string abilityName, string buttonName)
    {
        if (popupCoroutine != null)
        {
            StopCoroutine(popupCoroutine);
        }

        abilityNameText.text = abilityName + " otkljucano!";
        buttonText.text = "Pritnisni " + buttonName + " za koristenje";

        popupRoot.SetActive(true);
        popupCoroutine = StartCoroutine(HidePopupAfterDelay());
    }

    private IEnumerator HidePopupAfterDelay()
    {
        yield return new WaitForSeconds(popupDuration);
        popupRoot.SetActive(false);
    }
}
