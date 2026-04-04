using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance { get; private set; }

    public bool JumpUnlocked { get; private set; }
    public bool SprintUnlocked { get; private set; }
    public bool CrouchUnlocked { get; private set; }
    public bool DashUnlocked { get; private set; }
    public bool GrappleUnlocked { get; private set; }

    public bool objectPickupUnlocked { get; private set; }

    public bool DoubleJumpUnlocked { get; private set; }

    public bool GlideUnlocked { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Starting state: only walking is available
        JumpUnlocked = false;
        SprintUnlocked = false;
        CrouchUnlocked = false;
        DashUnlocked = false;
        GrappleUnlocked = false;
        objectPickupUnlocked = false;
        DoubleJumpUnlocked = false;
        GlideUnlocked = false;
    }

    public void UnlockJump()
    {
        JumpUnlocked = true;
    }

    public void UnlockSprint()
    {
        SprintUnlocked = true;
    }

    public void UnlockCrouch()
    {
        CrouchUnlocked = true;
    }

    public void UnlockDash()
    {
        DashUnlocked = true;
    }

    public void UnlockGrapple()
    {
        GrappleUnlocked = true;
    }

    public void UnlockObjectPickup()
    {
        objectPickupUnlocked = true;
    }
    public void UnlockDoubleJump()
    {
        DoubleJumpUnlocked = true;
    }


    public void UnlockGlide()
    {
        GlideUnlocked = true;
    }
}
