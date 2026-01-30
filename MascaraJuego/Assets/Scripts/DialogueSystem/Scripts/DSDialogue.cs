using UnityEngine;
using UnityEngine.InputSystem;

namespace DS
{
    using ScriptableObjects;
    using System;

    public class DSDialogue : MonoBehaviour
    {
        [SerializeField] public DSDialogueContainerSO dialogueContainer;
        [SerializeField] public DSDialogueGroupSO dialogueGroup;
        [SerializeField] public DSDialogueSO dialogue;

        // Filtros
        [SerializeField] public bool groupDialogues;
        [SerializeField] public bool startingDialoguesOnly;

        // �ndices
        [SerializeField] public int selectedDialogueGroupIndex;
        [SerializeField] public int selectedDialogueIndex;

        // Variables para detecci�n de proximidad y entrada del jugador
        [SerializeField] public Transform player;
        [SerializeField] public Animator myAnimator;
        [SerializeField] public float interactionDistance = 3f;
        private DSDialogueUI dialogueUI;

        private void Awake()
        {
            
        }

        private void Start()
        {
            dialogueUI = FindObjectOfType<DSDialogueUI>();
            if (dialogueUI == null)
            {
                Debug.LogError("No se encontr� DSDialogueUI en la escena.");
            }
        }

       
        public void StartDialogueFromOtherScript(DSDialogueSO dialogueSO)
        {
            dialogue = dialogueSO;
            BeginDialogue();
        }

       

        public void BeginDialogue()
        {
            if (dialogueUI == null)
            {
                Debug.LogError("dialogueUI no est� asignado");
                return;
            }

            if (!dialogueUI.inDialogue)
            {
                dialogueUI.inDialogue = true;

                if (dialogue == null)
                {
                    Debug.LogError("dialogue no est� asignado");
                    return;
                }

                dialogueUI.dialogueBox.SetActive(true);
                dialogueUI.currentDialogue = dialogue;
                dialogueUI.ShowText(dialogue);
            }
        }
    }
}
