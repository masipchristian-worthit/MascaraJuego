using UnityEngine;
using DS.ScriptableObjects;

public class CharacterInstance : MonoBehaviour
{
    public CharacterDataSO data; // Arrastra el SO aquí en el inspector
    private DSDialogueContainerSO currentDialogue;

    // Se llama desde el GameInitializer
    public void InitializeRole(bool isKiller, bool isVictim)
    {
        if (isKiller)
        {
            currentDialogue = data.killerDialogue;
            Debug.Log($"{data.characterName} asignado rol: ASESINO");
        }
        else if (isVictim)
        {
            currentDialogue = data.victimDialogue;
            Debug.Log($"{data.characterName} asignado rol: VÍCTIMA");
        }
        else
        {
            currentDialogue = data.normalDialogue;
        }
    }

    // Método para conectar con tu sistema de diálogo al interactuar
    public DSDialogueContainerSO GetCurrentDialogue()
    {
        return currentDialogue;
    }
}
