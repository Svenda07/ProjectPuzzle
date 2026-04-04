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
        objectPickup
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
                abilityName = "Jump";
                buttonName = "Space";
                break;

            case AbilityType.Sprint:
                AbilityManager.Instance.UnlockSprint();
                abilityName = "Sprint";
                buttonName = "Left Shift";
                break;

            case AbilityType.Crouch:
                AbilityManager.Instance.UnlockCrouch();
                abilityName = "Crouch";
                buttonName = "C";
                break;

            case AbilityType.Dash:
                AbilityManager.Instance.UnlockDash();
                abilityName = "Dash";
                buttonName = "B";
                break;
            case AbilityType.Grapple:
                AbilityManager.Instance.UnlockGrapple();
                abilityName = "Grapple Hook";
                buttonName = "Right Click";
                break;
            case AbilityType.objectPickup:
                AbilityManager.Instance.UnlockObjectPickup();
                abilityName = "Object Pickup";
                buttonName = "E";
                break;
        }


        if (AbilityPopupUI.Instance != null)
        {
            AbilityPopupUI.Instance.ShowAbilityPopup(abilityName, buttonName);
        }

        Destroy(gameObject);
    }


}
