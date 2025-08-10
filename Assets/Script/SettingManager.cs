using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif


public class SettingsManager : MonoBehaviour
{
    public GameObject settingsPanel;

    public GameObject tutorialPanel;
    public Image tutorialImage; 
    public Sprite[] tutorialPages; 
    public bool loopPages = true;

    public GameObject[] hideWhileTutorial;
    public bool pauseWhilePopup = false;

    public Slider bgmSlider;
    public Slider sfxSlider;

    private int page = 0;
    float _prevTimeScale = 1f;
    bool IsSettingsOpen() => settingsPanel && settingsPanel.activeSelf;
    bool IsTutorialOpen() => tutorialPanel && tutorialPanel.activeSelf;


    void Update()
    {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            HandleEsc();
        }
#else
    if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
    {
        HandleEsc();
    }
#endif
    }

    void HandleEsc()
    {
        if (IsTutorialOpen()) CloseTutorial();
        else if (IsSettingsOpen()) CloseSettings();
    }

    void OnEnable()
    {
        if (AudioManager.Instance != null)
        {
            if (bgmSlider)
            {
                bgmSlider.SetValueWithoutNotify(AudioManager.Instance.BGMVolume);
                bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
            }
            if (sfxSlider)
            {
                sfxSlider.SetValueWithoutNotify(AudioManager.Instance.SFXVolume);
                sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            }
        }
    }

    void OnDisable()
    {
        if (bgmSlider) bgmSlider.onValueChanged.RemoveListener(OnBGMVolumeChanged);
        if (sfxSlider) sfxSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
    }
    public void OnBGMVolumeChanged(float v)
    {
        AudioManager.Instance?.SetBGMVolume(v);
    }

    public void OnSFXVolumeChanged(float v)
    {
        AudioManager.Instance?.SetSFXVolume(v);
    }

    public void GoToMainMenu()
    {
        if (pauseWhilePopup) Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }
    public void ToggleSettingsPanel()
    {
        if (IsSettingsOpen()) CloseSettings();
        else OpenSettings();
    }

    public void ToggleTutorialPanel()
    {
        if (IsTutorialOpen()) CloseTutorial();
        else OpenTutorial();
    }

    public void OpenSettings()
    {
        if (tutorialPanel) tutorialPanel.SetActive(false);
        if (!settingsPanel) return;
        settingsPanel.SetActive(true);

        if (pauseWhilePopup) { _prevTimeScale = Time.timeScale; Time.timeScale = 0f; }
    }
    public void CloseSettings()
    {
        if (!settingsPanel) return;
        settingsPanel.SetActive(false);
    }

    public void OpenTutorial()
    {
        if (settingsPanel) settingsPanel.SetActive(false);
        page = 0;
        ApplyPage();
        if (tutorialPanel) tutorialPanel.SetActive(true);

        SetHidden(true);

        if (pauseWhilePopup) { _prevTimeScale = Time.timeScale; Time.timeScale = 0f; }
    }
    public void CloseTutorial()
    {
        if (!tutorialPanel) return;
        tutorialPanel.SetActive(false);

        SetHidden(false);

        if (pauseWhilePopup) Time.timeScale = _prevTimeScale;
    }
    public void NextTutorialPage()
    {
        if (tutorialPages == null || tutorialPages.Length == 0) return;

        page++;
        if (page >= tutorialPages.Length)
            page = loopPages ? 0 : tutorialPages.Length - 1;

        ApplyPage();
    }

    private void ApplyPage()
    {
        if (!tutorialImage) return;
        if (tutorialPages == null || tutorialPages.Length == 0) return;

        page = Mathf.Clamp(page, 0, tutorialPages.Length - 1);
        tutorialImage.sprite = tutorialPages[page];
        tutorialImage.SetNativeSize(); 
        tutorialImage.preserveAspect = true; 
    }
    void SetHidden(bool hidden)
    {
        if (hideWhileTutorial == null) return;
        foreach (var go in hideWhileTutorial)
            if (go) go.SetActive(!hidden);
    }
}
