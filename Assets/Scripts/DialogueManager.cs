using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    public Animator animator;
    
    private Queue<string> sentences;

    void Start()
    {
        sentences = new Queue<string>();        
    }

    public void StartDialogue(Dialogue dialogue)
    {
        animator.SetBool("isShowing", true);
        sentences.Clear();

        nameText.text = dialogue.name;

        foreach(string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }

    public void DisplayNextSentence ()
    {
        if(sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach(char s in sentence.ToCharArray())
        {
            dialogueText.text += s;
            yield return null;
        }
    }

    void EndDialogue()
    {
        animator.SetBool("isShowing", false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
