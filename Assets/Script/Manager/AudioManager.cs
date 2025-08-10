using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("BGM Clips")]
    public AudioClip mainMenuBGM;
    public AudioClip gameBGM;

    [Header("UI SFX Clips")]
    public AudioClip uiClickSFX;
    public AudioClip uiHoverSFX;

    private AudioSource bgmSource;
    private AudioSource sfxSource;

    [Range(0f, 1f)] public float defaultBGMVolume = 0.7f;
    [Range(0f, 1f)] public float defaultSFXVolume = 1f;

    public float BGMVolume => bgmSource ? bgmSource.volume : defaultBGMVolume;
    public float SFXVolume => sfxSource ? sfxSource.volume : defaultSFXVolume;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            float bgm = PlayerPrefs.HasKey("BGM_VOLUME") ? PlayerPrefs.GetFloat("BGM_VOLUME") : defaultBGMVolume;
            float sfx = PlayerPrefs.HasKey("SFX_VOLUME") ? PlayerPrefs.GetFloat("SFX_VOLUME") : defaultSFXVolume;
            bgmSource.volume = Mathf.Clamp01(bgm);
            sfxSource.volume = Mathf.Clamp01(sfx);

            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;

            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
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
            if (bgmSource.clip != mainMenuBGM)
            {
                bgmSource.clip = mainMenuBGM;
                bgmSource.Play();
            }
        }
        else
        {
            if (bgmSource.clip != gameBGM)
            {
                bgmSource.clip = gameBGM;
                bgmSource.Play();
            }
        }
    }

    public void PlaySFX(AudioClip clip, float pitch = 1f)
    {
        if (clip == null) return;
        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip);
        sfxSource.pitch = 1f;
    }
    public void SetBGMVolume(float v)
    {
        if (!bgmSource) return;
        bgmSource.volume = Mathf.Clamp01(v);
        PlayerPrefs.SetFloat("BGM_VOLUME", bgmSource.volume);
    }

    public void SetSFXVolume(float v)
    {
        if (!sfxSource) return;
        sfxSource.volume = Mathf.Clamp01(v);
        PlayerPrefs.SetFloat("SFX_VOLUME", sfxSource.volume);
    }

    public void PlayUIClick()
    {
        PlaySFX(uiClickSFX);
    }

    public void PlayUIHover()
    {
        PlaySFX(uiHoverSFX);
    }
}
