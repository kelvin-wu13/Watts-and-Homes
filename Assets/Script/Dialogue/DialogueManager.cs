using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }


    public GameObject dialoguePanel;
    public Image characterIconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    private Queue<DialogueLine> linesQueue;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        linesQueue = new Queue<DialogueLine>(); ;
    }

    public void StartDialogue(List<DialogueLine> lines)
    {
        linesQueue.Clear();
        foreach (DialogueLine line in lines)
        {
            linesQueue.Enqueue(line);
        }

        dialoguePanel.SetActive(true);
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (linesQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = linesQueue.Dequeue();

        characterIconImage.sprite = currentLine.characterIcon;
        nameText.text = currentLine.characterName;
        dialogueText.text = currentLine.text;
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
    }
}