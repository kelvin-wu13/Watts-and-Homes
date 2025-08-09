using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    public Image left1Display;
    public Image left2Display;
    public Image right1Display;
    public Image right2Display;

    public GameObject playerDialogueContainer;
    public GameObject npcDialogueContainer;

    public float typingSpeed = 0.04f;
    public float skipDelay = 0.5f;

    public event Action OnDialogueEnd;

    public Button blockerButton;
    private Queue<DialogueLine> linesQueue;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private DialogueLine currentLine;

    private Dictionary<DialogueLine.SpeakerSlot, Sprite> currentSprites =
    new Dictionary<DialogueLine.SpeakerSlot, Sprite>();

    private Sprite lastNameplatePlayer = null;
    private Sprite lastNameplateNPC = null;

    public bool IsRunning { get; private set; }

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
        linesQueue = new Queue<DialogueLine>();
    }

    public void StartDialogue(List<DialogueLine> lines)
    {
        IsRunning = true;
        linesQueue.Clear();
        foreach (DialogueLine line in lines)
        {
            linesQueue.Enqueue(line);
        }

        blockerButton.onClick.RemoveAllListeners();
        blockerButton.onClick.AddListener(DisplayNextLine);

        if (linesQueue.Count > 0)
        {
            currentLine = linesQueue.Dequeue();
            SetupVisuals(currentLine);
            StartCoroutine(ShowBlockerAndType(currentLine));
        }
    }
    private IEnumerator ShowBlockerAndType(DialogueLine line)
    {
        blockerButton.gameObject.SetActive(true);
        yield return null;
        typingCoroutine = StartCoroutine(TypeLine(line));
    }

    public void DisplayNextLine()
    {
        TextMeshProUGUI activeDialogueText = GetActiveDialogueText();
        if (isTyping && activeDialogueText != null)
        {
            StopCoroutine(typingCoroutine);
            activeDialogueText.text = currentLine.text;
            isTyping = false;
            return;
        }

        if (linesQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        currentLine = linesQueue.Dequeue();
        SetupVisuals(currentLine);
        typingCoroutine = StartCoroutine(TypeLine(currentLine));
    }
    private void EnsureUILayers(GameObject activeContainer)
    {
        if (playerDialogueContainer) playerDialogueContainer.transform.SetAsLastSibling();
        if (npcDialogueContainer) npcDialogueContainer.transform.SetAsLastSibling();

        var nameplate = activeContainer.transform.Find("Nameplate");
        if (nameplate) nameplate.SetAsLastSibling();

        var dialogueText = activeContainer.transform.Find("DialogueText");
        if (dialogueText && nameplate)
        {
            dialogueText.SetSiblingIndex(nameplate.GetSiblingIndex() - 1);
        }
    }

    private void BringSpeakerToFront(DialogueLine.SpeakerSlot slot)
    {
        if (left1Display && left2Display && left1Display.transform.parent == left2Display.transform.parent)
        {
            if (slot == DialogueLine.SpeakerSlot.Left1) left1Display.transform.SetAsLastSibling();
            if (slot == DialogueLine.SpeakerSlot.Left2) left2Display.transform.SetAsLastSibling();
        }

        if (right1Display && right2Display && right1Display.transform.parent == right2Display.transform.parent)
        {
            if (slot == DialogueLine.SpeakerSlot.Right1) right1Display.transform.SetAsLastSibling();
            if (slot == DialogueLine.SpeakerSlot.Right2) right2Display.transform.SetAsLastSibling();
        }
    }

    private void SetupVisuals(DialogueLine line)
    {
        playerDialogueContainer.SetActive(line.speaker == DialogueLine.SpeakerType.Player);
        npcDialogueContainer.SetActive(line.speaker == DialogueLine.SpeakerType.NPC);

        GameObject activeContainer = (line.speaker == DialogueLine.SpeakerType.Player) ? playerDialogueContainer : npcDialogueContainer;
        Image nameplate = activeContainer.transform.Find("Nameplate").GetComponent<Image>();

        if (nameplate != null)
        {
            if (line.characterNameplate != null)
            {
                nameplate.sprite = line.characterNameplate;
                if (line.speaker == DialogueLine.SpeakerType.Player) lastNameplatePlayer = line.characterNameplate;
                else lastNameplateNPC = line.characterNameplate;
            }
            else
            {
                nameplate.sprite = (line.speaker == DialogueLine.SpeakerType.Player) ? lastNameplatePlayer : lastNameplateNPC;
            }
        }

        Image target = GetDisplayForSlot(line.slot);

        if (line.characterSprite != null)
        {
            currentSprites[line.slot] = line.characterSprite;
        }
        Sprite targetSprite = currentSprites.ContainsKey(line.slot) ? currentSprites[line.slot] : null;

        ApplySprite(left1Display, DialogueLine.SpeakerSlot.Left1);
        ApplySprite(left2Display, DialogueLine.SpeakerSlot.Left2);
        ApplySprite(right1Display, DialogueLine.SpeakerSlot.Right1);
        ApplySprite(right2Display, DialogueLine.SpeakerSlot.Right2);

        DimAll();
        if (targetSprite != null)
        {
            target.color = Color.white;
        }

        BringSpeakerToFront(line.slot);
        EnsureUILayers(activeContainer);

        void ApplySprite(Image img, DialogueLine.SpeakerSlot slot)
        {
            Sprite s = currentSprites.ContainsKey(slot) ? currentSprites[slot] : null;
            if (slot == line.slot) s = targetSprite;
            if (s != null)
            {
                img.gameObject.SetActive(true);
                img.sprite = s;
            }
            else
            {
                img.gameObject.SetActive(false);
            }
        }

        void DimAll()
        {
            if (left1Display.gameObject.activeSelf) left1Display.color = Color.gray;
            if (left2Display.gameObject.activeSelf) left2Display.color = Color.gray;
            if (right1Display.gameObject.activeSelf) right1Display.color = Color.gray;
            if (right2Display.gameObject.activeSelf) right2Display.color = Color.gray;
        }
    }
    private Image GetDisplayForSlot(DialogueLine.SpeakerSlot slot)
    {
        switch (slot)
        {
            case DialogueLine.SpeakerSlot.Left1: return left1Display;
            case DialogueLine.SpeakerSlot.Left2: return left2Display;
            case DialogueLine.SpeakerSlot.Right1: return right1Display;
            case DialogueLine.SpeakerSlot.Right2: return right2Display;
            default: return left1Display;
        }
    }


    IEnumerator TypeLine(DialogueLine line)
    {
        TextMeshProUGUI activeDialogueText = GetActiveDialogueText();
        if (activeDialogueText == null) yield break;

        isTyping = true;
        activeDialogueText.text = "";

        foreach (char letter in line.text.ToCharArray())
        {
            activeDialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        yield return new WaitForSeconds(skipDelay);
    }

    private void EndDialogue()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        isTyping = false;

        if (blockerButton) blockerButton.gameObject.SetActive(false);
        if (playerDialogueContainer) playerDialogueContainer.SetActive(false);
        if (npcDialogueContainer) npcDialogueContainer.SetActive(false);

        left1Display.gameObject.SetActive(false);
        left2Display.gameObject.SetActive(false);
        right1Display.gameObject.SetActive(false);
        right2Display.gameObject.SetActive(false);

        currentSprites.Clear();
        lastNameplatePlayer = null;
        lastNameplateNPC = null;

        IsRunning = false;

        OnDialogueEnd?.Invoke();
    }


    private TextMeshProUGUI GetActiveDialogueText()
    {
        if (playerDialogueContainer.activeSelf)
        {
            return playerDialogueContainer.GetComponentInChildren<TextMeshProUGUI>();
        }
        if (npcDialogueContainer.activeSelf)
        {
            return npcDialogueContainer.GetComponentInChildren<TextMeshProUGUI>();
        }
        return null;
    }
}