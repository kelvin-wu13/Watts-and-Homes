using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public enum SpeakerType { Player, NPC }
    public SpeakerType speaker;

    public enum SpeakerSlot { Left1, Left2, Right1, Right2 }
    public SpeakerSlot slot;

    public Sprite characterSprite;
    public Sprite characterNameplate;

    [TextArea(3, 10)]
    public string text;
}
