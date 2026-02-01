using DS.ScriptableObjects;
using UnityEngine;

public class DialogueNPC : MonoBehaviour
{
    [SerializeField] private DSDialogueContainerSO _dialogueContainerSO;

    public void startDialogue()
    {
        DSDialogueUI.Instance.PlayDialogueNPC(_dialogueContainerSO);
    }
}
