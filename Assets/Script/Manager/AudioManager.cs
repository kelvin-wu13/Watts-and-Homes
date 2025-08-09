using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("BGM Clips")]
    public AudioClip mainMenuBGM;
    public AudioClip gameBGM;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        PlayBGMForScene(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayBGMForScene(scene.name);
    }

    private void PlayBGMForScene(string sceneName)
    {
        if (sceneName == "MainMenu")
        {
            if (audioSource.clip != mainMenuBGM)
            {
                audioSource.clip = mainMenuBGM;
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.clip != gameBGM)
            {
                audioSource.clip = gameBGM;
                audioSource.Play();
            }
        }
    }
}
