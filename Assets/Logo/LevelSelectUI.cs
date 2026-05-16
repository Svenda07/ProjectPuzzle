using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectUI : MonoBehaviour
{
    [System.Serializable]
    public class LevelButtonData
    {
        public Button button;
        public string sceneName;
        public int levelNumber;
    }

    [SerializeField] private LevelButtonData[] levels;

    private void Start()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        foreach (LevelButtonData level in levels)
        {
            bool isUnlocked = level.levelNumber <= unlockedLevel;
            level.button.interactable = isUnlocked;
        }
    }

    public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}