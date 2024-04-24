using System.Collections;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue[] dialogues;

    public void StartDialogue()
    {
        FindObjectOfType<DialogueManager>().OpenDialogue(dialogues);
    }
}

[System.Serializable]

public class Dialogue{
    public string line;
}