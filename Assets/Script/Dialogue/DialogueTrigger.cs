//using System.Collections.Generic;
//using UnityEngine;

//[System.Serializable]
//public class DialogueCharacter
//{
//    public string name;
//    public string icon;
//}

//[System.Serializable]
//public class DialogueLine
//{
//    public DialogueCharacter character;
//    [TextArea(3, 10)]
//    public string text;
//}
//[System.Serializable]
//public class Dialogue
//{
//    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
//}
//public class DialogueTrigger : MonoBehaviour
//{
//    public Dialogue dialogue;

//    public void TriggerDialogue()
//    {
//        DialogueManager.Instance.StartDialogue(dialogue);
//    }
//}