using UnityEngine;
using DS.ScriptableObjects;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "GameJam/Character Data")]
public class CharacterDialogueChoose : ScriptableObject
{
    [Header("Identidad")]
    public GameCharacterID characterID; // Sincronizado con el nuevo Enum
    public string characterName;

    [Header("Contenedores de Diálogo")]
    public DSDialogueContainerSO neutralDialogue;
    public DSDialogueContainerSO killerDialogue; 
    public DSDialogueContainerSO victimDialogue;

    [Header("Pista del Crimen")]
    public GameObject cluePrefab; 
    
    [TextArea] public string normalClueDescription;
    [TextArea] public string incriminatingClueDescription;
}