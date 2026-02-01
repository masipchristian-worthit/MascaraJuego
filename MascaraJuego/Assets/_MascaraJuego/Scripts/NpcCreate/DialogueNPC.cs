using UnityEngine;
using DS.ScriptableObjects;

public class DialogueNPC : MonoBehaviour
{
    [Header("Identidad")]
    // Esta es la variable que faltaba y causaba el error 'does not contain definition for myID'
    public GameCharacterID myID; 

    [SerializeField] private DSDialogueContainerSO _dialogueContainerSO;

    // Esta es la función que faltaba y causaba el error 'does not contain definition for SetDialogue'
    public void SetDialogue(DSDialogueContainerSO newDialogue)
    {
        _dialogueContainerSO = newDialogue;
    }

    public void StartDialogue()
    {
        if (_dialogueContainerSO != null)
        {
            // Asegúrate de que DSDialogueUI es accesible
            DSDialogueUI.Instance.PlayDialogueNPC(_dialogueContainerSO);
        }
        else
        {
            Debug.LogWarning($"El personaje {name} no tiene diálogo asignado.");
        }
    }
}