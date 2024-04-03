using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueScript : MonoBehaviour
{
    public string[] dialogue;
    public bool isComplete = false;
    private int index = 0;

    public string RunDialogue()
    {
        string currentDialogue = GetDialogue();
        index++;
        return currentDialogue;
    }

    private string GetDialogue()
    {
        if (index < dialogue.Length)
        {
            return dialogue[index];
        }
        else
        {
            isComplete = true;
            return null;
        }
    }

    public void ResetDialogue()
    {
        index = 0;
        //isComplete = false;
    }
}
