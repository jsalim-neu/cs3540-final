using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//
public class DialoguePrompt : MonoBehaviour
{
    //how does the prompt show up in the overworld?
    public Text promptTextLabel;
    
    //actual text
    public Text dialoguePopUp;

    //what key continues the dialogue?
    public KeyCode promptKey;

    //
    public string promptText;

    //triggers a level flag upon dialogue completion if true
    public bool requiredToProgress = false;

    //opens shop prompt if true
    public bool isMerchant = false;

    //refers to the "next" dialogue object in the level, makes it interactable if so 
    public GameObject nextDialogueObject;

    public enum InteractableEnityType
    {
        NPC,
        OBJECT
    }

    public InteractableEnityType interactableType = InteractableEnityType.NPC;

    //separate script object that contains the lines to be "said" (attached to same GameObject)
    DialogueScript dialogueScript;

    //NPC/Object behavior script (attached to same GameObject)
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
        Debug.Log("Displaying Dialogue!");
        entityBehaviour.currentState = EntityBehaviour.FSMState.Talk;

        promptTextLabel.gameObject.SetActive(false);
        dialoguePopUp.gameObject.SetActive(true);
        dialoguePopUp.text = dialogueScript.RunDialogue();

        if (dialogueScript.isComplete)
        {
            ExitDialogue();
            if (requiredToProgress)
            {
                //send LevelManager an update to the objective
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
        dialoguePopUp.gameObject.SetActive(false);
        dialogueScript.ResetDialogue();
        isDialogueRunning = false;
        if (nextDialogueObject != null)
        {
            nextDialogueObject.GetComponent<Collider>().enabled = true;
        }
    }
}
