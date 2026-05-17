using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuRoot;
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private FirstPersonController firstPersonController;
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isPaused;

    private void Start()
    {
        pauseMenuRoot.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        if (sensitivitySlider != null && firstPersonController != null)
        {
            sensitivitySlider.minValue = 0.01f;
            sensitivitySlider.maxValue = 1.0f;
            sensitivitySlider.SetValueWithoutNotify(firstPersonController.GetMouseSensitivity());
            sensitivitySlider.onValueChanged.RemoveAllListeners();
            sensitivitySlider.onValueChanged.AddListener(firstPersonController.SetMouseSensitivity);
        }

        LockCursor();
    }

    private void Update()
    {
        if (playerInputHandler != null && playerInputHandler.PausePressedThisFrame)
        {
            playerInputHandler.ClearFrameInput();

            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        if (isPaused) return;

        isPaused = true;
        pauseMenuRoot.SetActive(true);
        Time.timeScale = 0f;
        UnlockCursor();
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        isPaused = false;
        pauseMenuRoot.SetActive(false);
        Time.timeScale = 1f;
        LockCursor();
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}