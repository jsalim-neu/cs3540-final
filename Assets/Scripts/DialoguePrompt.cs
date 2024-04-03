using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialoguePrompt : MonoBehaviour
{
    public Text promptTextLabel;
    public Text dialoguePopUp;
    public KeyCode promptKey;
    public string promptText;
    public bool requiredToProgress = false;

    public enum InteractableEnityType
    {
        NPC,
        OBJECT
    }

    public InteractableEnityType interactableType = InteractableEnityType.NPC;

    DialogueScript dialogueScript;
    EntityBehaviour entityBehaviour;
    bool isDialogueRunning = false;
    bool canActivateDialogue = false;
    
    void Start()
    {
        dialogueScript = GetComponent<DialogueScript>();
        promptTextLabel.text = promptText;

        if (interactableType == InteractableEnityType.NPC)
        {
            entityBehaviour = GetComponent<EntityBehaviour>();
        }
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
            promptTextLabel.text = promptText;
            canActivateDialogue = true;

            if (interactableType == InteractableEnityType.NPC)
            {
                entityBehaviour.currentState = EntityBehaviour.FSMState.Idle;
                print("Player in range");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            promptTextLabel.gameObject.SetActive(false);
            canActivateDialogue = false;
            ExitDialogue();

            if (interactableType == InteractableEnityType.NPC)
            {
                entityBehaviour.currentState = EntityBehaviour.FSMState.Patrol;
            }
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
                LevelManager.currObjective.ObjectiveUpdate(ObjectiveType.INTERACTION, 1);
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
