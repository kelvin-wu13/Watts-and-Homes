using UnityEngine;
using System.Collections.Generic;

public class DialogueTrigger : MonoBehaviour
{
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
    public bool playOnStart = false;
    public bool playOnce = false;
    private bool hasPlayed = false;
    public bool playOncePersistent = false;
    public string persistentKeyOverride = "";

    private void Start()
    {
        if (playOnStart)
        {
            TriggerDialogue();
        }
    }

    public void TriggerDialogue()
    {
        if (playOnce && hasPlayed) return;

        if (playOncePersistent)
        {
            string key = string.IsNullOrEmpty(persistentKeyOverride)
                ? $"dlg_seen__{UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}__{gameObject.name}"
                : persistentKeyOverride;

            if (PlayerPrefs.GetInt(key, 0) == 1) return;
            PlayerPrefs.SetInt(key, 1);
            PlayerPrefs.Save();
        }

        DialogueManager.Instance.StartDialogue(dialogueLines);
        hasPlayed = true;
    }

}