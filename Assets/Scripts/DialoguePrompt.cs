using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialoguePrompt : MonoBehaviour
{
    public LevelManager levelManager;
    public Text promptTextLabel;
    public Text dialoguePopUp;
    public KeyCode promptKey;
    public string promptText;
    public bool requiredToProgress = false;

    DialogueScript dialogueScript;
    bool isDialogueRunning = false;
    bool canActivateDialogue = false;
    
    void Start()
    {
        dialogueScript = GetComponent<DialogueScript>();
        promptTextLabel.text = promptText;
    }

    void Update()
    {
        // OnPromptButtonPressed();
        if (Input.GetKeyDown(promptKey) && canActivateDialogue)
        {
            DisplayDialogue();
        }

        if (isDialogueRunning)
        {
            dialoguePopUp.text = dialogueScript.RunDialogue();
        }

        if (dialogueScript.isComplete)
        {
            ExitDialogue();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            promptTextLabel.gameObject.SetActive(true);
            canActivateDialogue = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            promptTextLabel.gameObject.SetActive(false);
            canActivateDialogue = false;
            ExitDialogue();
        }
    }

    void DisplayDialogue()
    {
        promptTextLabel.gameObject.SetActive(false);
        dialoguePopUp.gameObject.SetActive(true);
        dialoguePopUp.text = dialogueScript.RunDialogue();

        if (dialogueScript.isComplete)
        {
            ExitDialogue();
            if (requiredToProgress)
            {
                levelManager.objective.ObjectiveUpdate(ObjectiveType.INTERACTION, 1);
            }
        }
    }

    void ExitDialogue()
    {
        dialoguePopUp.gameObject.SetActive(false);
        dialogueScript.ResetDialogue();
        isDialogueRunning = false;
    }
}
