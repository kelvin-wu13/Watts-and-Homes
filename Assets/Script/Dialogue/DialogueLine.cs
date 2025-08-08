using UnityEngine;

public class DialogueLine : MonoBehaviour
{
    public string characterName;
    public Sprite characterIcon;

    [TextArea(3,10)]
    public string text;
}
