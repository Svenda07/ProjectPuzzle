using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelEndTrigger : MonoBehaviour
{

    [SerializeField] private string nextSceneName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
