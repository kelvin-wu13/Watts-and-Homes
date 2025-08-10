using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class ButtonSFX : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, ISubmitHandler, IPointerDownHandler
{
   
    public AudioClip clickClip;
    public AudioClip hoverClip;

    public bool playOnPointerDown = false;

    [Range(0f, 2f)] public float volumeMultiplier = 1f;

    [Range(0.5f, 1.5f)] public float pitchMin = 1f;
    [Range(0.5f, 1.5f)] public float pitchMax = 1f;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!playOnPointerDown) PlayClick();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (playOnPointerDown) PlayClick();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayHover();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        PlayClick();
    }

    void PlayClick()
    {
        if (AudioManager.Instance == null) return;

        float pitch = (pitchMax >= pitchMin) ? Random.Range(pitchMin, pitchMax) : 1f;

        if (clickClip != null)
        {
            AudioManager.Instance.PlaySFX(clickClip, pitch);
        }
        else
        {
            AudioManager.Instance.PlayUIClick();
        }

        if (volumeMultiplier != 1f)
            AudioManager.Instance.SetSFXVolume(AudioManager.Instance != null ? AudioManager.Instance.GetComponent<AudioSource>()?.volume ?? 1f : 1f);
    }

    void PlayHover()
    {
        if (AudioManager.Instance == null) return;

        float pitch = (pitchMax >= pitchMin) ? Random.Range(pitchMin, pitchMax) : 1f;

        if (hoverClip != null)
        {
            AudioManager.Instance.PlaySFX(hoverClip, pitch);
        }
        else
        {
            AudioManager.Instance.PlayUIHover();
        }
    }
}
