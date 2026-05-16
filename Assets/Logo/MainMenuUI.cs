using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private string levelSelectSceneName = "LevelSelect";
    [SerializeField] private Button continueButton;

    private void Start()
    {
        if (continueButton != null)
        {
            continueButton.interactable = PlayerPrefs.HasKey("HasSave");
        }
    }

    public void NewGame()
    {
        PlayerPrefs.SetInt("HasSave", 1);
        PlayerPrefs.SetInt("UnlockedLevel", 1); // only tutorial available
        PlayerPrefs.SetInt("LastLevel", 1);
        PlayerPrefs.Save();

        if (AbilityManager.Instance != null)
        {
            AbilityManager.Instance.ResetAbilities();
        }

        SceneManager.LoadScene(levelSelectSceneName);
    }

    public void ContinueGame()
    {
        if (PlayerPrefs.HasKey("HasSave"))
        {
            SceneManager.LoadScene(levelSelectSceneName);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}