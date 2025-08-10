using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

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

            if (!bgmSource)
            {
                bgmSource = gameObject.AddComponent<AudioSource>();
                bgmSource.loop = true;
                bgmSource.playOnAwake = false;
            }
            if (!sfxSource)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
            }

            float bgm = PlayerPrefs.HasKey("BGM_VOLUME") ? PlayerPrefs.GetFloat("BGM_VOLUME") : defaultBGMVolume;
            float sfx = PlayerPrefs.HasKey("SFX_VOLUME") ? PlayerPrefs.GetFloat("SFX_VOLUME") : defaultSFXVolume;
            bgmSource.volume = Mathf.Clamp01(bgm);
            sfxSource.volume = Mathf.Clamp01(sfx);
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        PlayBGMForScene(SceneManager.GetActiveScene().name);
    }
    void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayBGMForScene(scene.name);
    }

    private void PlayBGMForScene(string sceneName)
    {
        if (!bgmSource) return;

        AudioClip target = (sceneName == "MainMenu") ? mainMenuBGM : gameBGM;
        if (target == null)
        {
            if (bgmSource.isPlaying) bgmSource.Stop();
            bgmSource.clip = null;
            return;
        }

        if (bgmSource.clip == target && bgmSource.isPlaying) return;
        bgmSource.clip = target;
        bgmSource.Play();
    }

    public void PlaySFX(AudioClip clip, float pitch = 1f)
    {
        if (clip == null || !sfxSource) return;
        float oldPitch = sfxSource.pitch;
        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip);
        sfxSource.pitch = oldPitch;
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
