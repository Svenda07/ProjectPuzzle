using UnityEngine;

public class AbilityPickup : MonoBehaviour
{
    public enum AbilityType
    {
        Jump,
        Sprint,
        Crouch,
        Dash,
        Grapple,
        objectPickup,
        DoubleJump,
        Glide
    }

    [SerializeField] private AbilityType abilityType;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (AbilityManager.Instance == null)
            return;

        string abilityName = "";
        string buttonName = "";

        switch (abilityType)
        {
            case AbilityType.Jump:
                AbilityManager.Instance.UnlockJump();
                abilityName = "Skok";
                buttonName = "Space";
                break;

            case AbilityType.Sprint:
                AbilityManager.Instance.UnlockSprint();
                abilityName = "Trcanje";
                buttonName = "Left Shift";
                break;

            case AbilityType.Crouch:
                AbilityManager.Instance.UnlockCrouch();
                abilityName = "Cucanj";
                buttonName = "C";
                break;

            case AbilityType.Dash:
                AbilityManager.Instance.UnlockDash();
                abilityName = "Odgurivanje u zraku";
                buttonName = "B";
                break;
            case AbilityType.Grapple:
                AbilityManager.Instance.UnlockGrapple();
                abilityName = "Kuka za penjanje";
                buttonName = "Right Click";
                break;
            case AbilityType.objectPickup:
                AbilityManager.Instance.UnlockObjectPickup();
                abilityName = "Podizanje objekata";
                buttonName = "E";
                break;
            case AbilityType.DoubleJump:
                AbilityManager.Instance.UnlockDoubleJump();
                abilityName = "Dvostruki skok";
                buttonName = "Space";
                break;
            case AbilityType.Glide:
                AbilityManager.Instance.UnlockGlide();
                abilityName = "Lebdenje";
                buttonName = "Hold Space";
                break;
        }


        if (AbilityPopupUI.Instance != null)
        {
            AbilityPopupUI.Instance.ShowAbilityPopup(abilityName, buttonName);
        }

        Destroy(gameObject);
    }


}
