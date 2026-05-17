using UnityEngine;
using TMPro;

public class MusicMuteButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buttonText;

    private void Start()
    {
        RefreshLabel();
    }

    private void OnEnable()
    {
        RefreshLabel();
    }

    public void ToggleMusic()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.ToggleMusicMute();
            RefreshLabel();
        }
    }

    public void RefreshLabel()
    {
        if (buttonText == null || MusicManager.Instance == null)
            return;

        buttonText.text = MusicManager.Instance.IsMusicMuted()
            ? "Zvuk"
            : "Tišina";
    }
}