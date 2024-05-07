using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public TMP_Text dialogueText;
    public RectTransform backgroundBox;

    Dialogue[] currentDialogues;
    int activeDialogue = 0;
    public static bool isActive = false;
    public float textSpeed = 2f;

    //sounds
    public AudioSource audioSource;
    // public AudioClip audioClip;

    private void Start() {
        audioSource = GetComponent<AudioSource>();
        backgroundBox.transform.localScale = Vector3.zero;
    }

    public void OpenDialogue(Dialogue[] dialogues)
    {
        currentDialogues = dialogues;
        activeDialogue = 0;
        isActive = true;

        dialogueText.text = string.Empty;
        DisplayDialogue();
        
        backgroundBox.transform.localScale = Vector3.one;
    }

    void DisplayDialogue()
    {
        Dialogue dialogueTextToDisplay = currentDialogues[activeDialogue];
        StartCoroutine(TypeDialogue(dialogueTextToDisplay));
    }

    IEnumerator TypeDialogue(Dialogue dialogue)
    {
        audioSource.Play();
        foreach (char letter in dialogue.line)
        {
            dialogueText.text += char.ToUpper(letter);
            yield return new WaitForSeconds(textSpeed);
        }
        audioSource.Stop();
        yield return new WaitForSeconds(1f);

        NextLine();
    }

    public void NextLine()
    {
        activeDialogue++;
        if (activeDialogue < currentDialogues.Length)
        {
            dialogueText.text = string.Empty;
            DisplayDialogue();
        }
        else{
            //dialogue ended
            isActive = false;
            backgroundBox.transform.localScale = Vector3.zero;
        }
    }
}
