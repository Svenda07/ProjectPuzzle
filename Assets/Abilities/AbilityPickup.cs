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
            case AbilityType.DoubleJump:
                AbilityManager.Instance.UnlockDoubleJump();
                abilityName = "Double Jump";
                buttonName = "Space";
                break;
            case AbilityType.Glide:
                AbilityManager.Instance.UnlockGlide();
                abilityName = "Glide";
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
