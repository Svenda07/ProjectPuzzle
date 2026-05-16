using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEndTrigger : MonoBehaviour
{
    [SerializeField] private string nextSceneName;
    [SerializeField] private int nextLevelToUnlock;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            int currentUnlocked = PlayerPrefs.GetInt("UnlockedLevel", 1);

            if (nextLevelToUnlock > currentUnlocked)
            {
                PlayerPrefs.SetInt("UnlockedLevel", nextLevelToUnlock);
            }

            PlayerPrefs.SetInt("HasSave", 1);
            PlayerPrefs.SetInt("LastLevel", nextLevelToUnlock);
            PlayerPrefs.Save();

            SceneManager.LoadScene(nextSceneName);
        }
    }
}