using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Audio Source")]
    [SerializeField] private AudioSource musicSource;

    [Header("Shared Music")]
    [SerializeField] private AudioClip menuMusic;

    [Header("Level Music")]
    [SerializeField] private AudioClip level1Music;
    [SerializeField] private AudioClip level2Music;
    [SerializeField] private AudioClip level3Music;

    private const string MusicMutedKey = "MusicMuted";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource == null)
        {
            musicSource = GetComponent<AudioSource>();
        }

        ApplyMuteState();
    }

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    private void Start()
    {
        UpdateMusicForScene(SceneManager.GetActiveScene().name);
    }

    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        UpdateMusicForScene(newScene.name);
    }

    private void UpdateMusicForScene(string sceneName)
    {
        AudioClip targetClip = GetClipForScene(sceneName);

        if (musicSource == null || targetClip == null)
            return;

        if (musicSource.clip == targetClip && musicSource.isPlaying)
        {
            ApplyMuteState();
            return;
        }

        musicSource.clip = targetClip;
        musicSource.loop = true;
        musicSource.Play();

        ApplyMuteState();
    }

    private AudioClip GetClipForScene(string sceneName)
    {
        switch (sceneName)
        {
            case "MainMenu":
            case "LevelSelect":
            case "Tutorial":
                return menuMusic;

            case "DiceLevel":
                return level1Music;

            case "GrappleLevel":
                return level2Music;

            case "FinalLevel":
                return level3Music;

            default:
                return menuMusic;
        }
    }

    public void ToggleMusicMute()
    {
        bool muted = IsMusicMuted();
        PlayerPrefs.SetInt(MusicMutedKey, muted ? 0 : 1);
        PlayerPrefs.Save();
        ApplyMuteState();
    }

    public bool IsMusicMuted()
    {
        return PlayerPrefs.GetInt(MusicMutedKey, 0) == 1;
    }

    private void ApplyMuteState()
    {
        if (musicSource != null)
        {
            musicSource.mute = IsMusicMuted();
        }
    }
}