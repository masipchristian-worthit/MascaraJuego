using UnityEngine;
// - Referencia al sistema de diálogos del usuario
using DS.ScriptableObjects; 

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "GameJam/Character Data")]
public class CharacterDataSO : ScriptableObject
{
    [Header("Identidad")]
    public CharacterID characterID; // Enum definido más abajo
    public string characterName;

    [Header("Diálogos (Referencias)")]
    // Referencias a tus objetos de diálogo específicos
    public DSDialogueContainerSO normalDialogue;
    public DSDialogueContainerSO victimDialogue;
    public DSDialogueContainerSO killerDialogue;

    [Header("Pistas de Objeto")]
    public string clueItemName; // Ej: "Golden Steak Knife"
    [TextArea] public string normalClueDescription; //
    [TextArea] public string incriminatingClueDescription; //
}

// Enum para el Switch solicitado
public enum CharacterID
{
    DonaldTrump,
    ShaquilleONeal,
    ElonMusk,
    Torrente,
    KanyeWest,
    BelenEsteban,
    TimotheeChalamet,
    NickiMinaj
}