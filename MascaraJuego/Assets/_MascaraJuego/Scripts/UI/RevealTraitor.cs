using System.Collections.Generic;
using UnityEngine;
using System.Linq; 
using System;
using DS.ScriptableObjects; 

// ---------------------------------------------------------
// DEFINICIÓN DEL ENUM GLOBAL
// ---------------------------------------------------------
public enum GameCharacterID
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

public class RevealTraitor : MonoBehaviour
{
    [Header("Configuración General")]
    public List<CharacterDialogueChoose> allCharacterData; 

    [Header("Referencias de Escena")]
    public List<DialogueNPC> sceneNPCs;
    
    // Variables para Debug
    [SerializeField] private GameCharacterID trueAssassinID;
    [SerializeField] private GameCharacterID trueVictimID;

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        // 1. SORTEO DE ROLES
        int randomIndex = UnityEngine.Random.Range(0, allCharacterData.Count);
        CharacterDialogueChoose trueAssassinData = allCharacterData[randomIndex];
        trueAssassinID = trueAssassinData.characterID;

        trueVictimID = GetFixedVictimFor(trueAssassinID);
        CharacterDialogueChoose trueVictimData = allCharacterData.Find(x => x.characterID == trueVictimID);

        var remainingCharacters = allCharacterData
            .Where(c => c.characterID != trueAssassinID && c.characterID != trueVictimID)
            .OrderBy(x => UnityEngine.Random.value)
            .ToList();

        List<CharacterDialogueChoose> fakeKillers = remainingCharacters.Take(2).ToList();
        List<CharacterDialogueChoose> fakeVictims = remainingCharacters.Skip(2).Take(1).ToList();
        List<CharacterDialogueChoose> neutrals = remainingCharacters.Skip(3).ToList();

        // 2. ASIGNAR DIÁLOGOS
        AssignRoleToScene(trueAssassinData, RoleType.Killer);
        foreach (var fake in fakeKillers) AssignRoleToScene(fake, RoleType.Killer);

        AssignRoleToScene(trueVictimData, RoleType.Victim);
        foreach (var fakeV in fakeVictims) AssignRoleToScene(fakeV, RoleType.Victim);

        foreach (var neutral in neutrals) AssignRoleToScene(neutral, RoleType.Neutral);

        // 3. GENERAR PISTAS
        SpawnClue(trueAssassinData, isIncriminating: true);
        foreach (var fake in fakeKillers) SpawnClue(fake, isIncriminating: true);

        List<CharacterDialogueChoose> poolOfInnocents = new List<CharacterDialogueChoose>();
        poolOfInnocents.Add(trueVictimData);
        poolOfInnocents.AddRange(fakeVictims);
        poolOfInnocents.AddRange(neutrals);

        var selectedNormalClues = poolOfInnocents.OrderBy(x => UnityEngine.Random.value).Take(2).ToList();
        foreach (var normalData in selectedNormalClues) SpawnClue(normalData, isIncriminating: false);
        
        Debug.Log($"<color=red>JUEGO INICIADO:</color> Asesino: {trueAssassinID} | Víctima: {trueVictimID}");
    }

    private GameCharacterID GetFixedVictimFor(GameCharacterID killer)
    {
        switch (killer)
        {
            case GameCharacterID.KanyeWest: return GameCharacterID.ElonMusk;
            case GameCharacterID.Torrente: return GameCharacterID.TimotheeChalamet;
            case GameCharacterID.DonaldTrump: return GameCharacterID.NickiMinaj;
            case GameCharacterID.ShaquilleONeal: return GameCharacterID.BelenEsteban;
            case GameCharacterID.ElonMusk: return GameCharacterID.DonaldTrump;
            case GameCharacterID.BelenEsteban: return GameCharacterID.ShaquilleONeal;
            case GameCharacterID.TimotheeChalamet: return GameCharacterID.Torrente;
            case GameCharacterID.NickiMinaj: return GameCharacterID.KanyeWest;
            default: return GameCharacterID.ElonMusk; 
        }
    }

    private void AssignRoleToScene(CharacterDialogueChoose data, RoleType role)
    {
        DialogueNPC npc = sceneNPCs.Find(n => n.myID == data.characterID);
        
        if (npc != null)
        {
            DSDialogueContainerSO selectedDialogue = null;
            switch (role)
            {
                case RoleType.Neutral: selectedDialogue = data.neutralDialogue; break;
                case RoleType.Killer: selectedDialogue = data.killerDialogue; break; 
                case RoleType.Victim: selectedDialogue = data.victimDialogue; break;
            }
            npc.SetDialogue(selectedDialogue);
        }
    }

    private void SpawnClue(CharacterDialogueChoose data, bool isIncriminating)
    {
        if (data.cluePrefab == null) return;

        DialogueNPC ownerNPC = sceneNPCs.Find(n => n.myID == data.characterID);
        Vector3 spawnPos = transform.position; 
        
        if (ownerNPC != null) spawnPos = ownerNPC.transform.position; 
        
        GameObject newClue = Instantiate(data.cluePrefab, spawnPos, Quaternion.identity);
        newClue.name = "Clue_" + data.characterName;
    }

    private enum RoleType { Neutral, Killer, Victim }
}