using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialoguePrompt : MonoBehaviour
{
    public Text promptTextLabel;
    public Text dialoguePopUp;

    public GameObject playerStatePanel;

    public KeyCode promptKey;
    public string promptText;
    public bool requiredToProgress = false;

    public bool isMerchant = false;

    public GameObject nextDialogueObject;

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
        Time.timeScale = 0;
        Debug.Log("Displaying Dialogue!");
        promptTextLabel.gameObject.SetActive(false);
        dialoguePopUp.gameObject.SetActive(true);
        dialoguePopUp.text = dialogueScript.RunDialogue();
        EnableUI(false);

        if (dialogueScript.isComplete)
        {
            ExitDialogue();
            if (requiredToProgress)
            {
                LevelManager.currObjective.ObjectiveUpdate(ObjectiveType.INTERACTION, 1);
                //no longer allow this dialogue to progress the story
                requiredToProgress = false;
            }
            if (isMerchant)
            {
            //open shop menu
            GameObject.FindWithTag("Shop").GetComponent<ShopMenuBehavior>().OpenShop();
            }
        }
    }

    void ExitDialogue()
    {
        Time.timeScale = 1;
        dialoguePopUp.gameObject.SetActive(false);
        dialogueScript.ResetDialogue();
        isDialogueRunning = false;
        EnableUI(true);

        if (nextDialogueObject != null)
        {
            nextDialogueObject.GetComponent<Collider>().enabled = true;
        }
    }

    void EnableUI(bool isEnabled)
    {
        if (playerStatePanel)
        {
            playerStatePanel.SetActive(isEnabled);
        }
    }
}
