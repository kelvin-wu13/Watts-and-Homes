using UnityEngine;
using System.Collections.Generic;

public class DialogueTrigger : MonoBehaviour
{
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();

    public void TriggerDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogueLines);
    }
}