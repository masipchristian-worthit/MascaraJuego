using DS.ScriptableObjects;
using TMPro;
using UnityEngine;

public class NPCBase : MonoBehaviour
{
    public bool isBad;
    [SerializeField] private DSDialogueContainerSO dsDialogue;
    private DSDialogueUI dsDialogueUI;
    void Start()
    {
        dsDialogueUI = GetComponent<DSDialogueUI>();
    }

    private void InteractNPC()
    {
        
    }
}
