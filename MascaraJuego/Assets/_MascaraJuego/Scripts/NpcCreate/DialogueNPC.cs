using DS.ScriptableObjects;
using UnityEngine;

public class DialogueNPC : MonoBehaviour
{
    [SerializeField] private DSDialogueContainerSO _dialogueContainerSO;
    public string Name;
    public void startDialogue()
    {
        DSDialogueUI.Instance.PlayDialogueNPC(_dialogueContainerSO);
    }
}
