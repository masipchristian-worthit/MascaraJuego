using System.Collections.Generic;
using System.Linq;
using System.Text; // Necesario para StringBuilder
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [Header("Referencias de Escena")]
    public List<CharacterInstance> allCharacters;
    public List<ClueInstance> allClues;

    [Header("Debug")]
    [Tooltip("Activa esto para ver el informe detallado en la consola")]
    public bool showDebugLog = true;

    // Listas internas para el estado actual (para facilitar el debug)
    private List<CharacterInstance> activeKillers = new List<CharacterInstance>();
    private List<CharacterInstance> activeVictims = new List<CharacterInstance>();
    private List<CharacterInstance> activeNormals = new List<CharacterInstance>();

private void Awake()
    {
        // -----------------------------------------------------------
        // AUTO-DETECCIÓN: Si las listas están vacías, las busca solo
        // -----------------------------------------------------------
        if (allCharacters == null || allCharacters.Count == 0)
        {
            // Busca todos los objetos que tengan el script CharacterInstance en la escena
            allCharacters = new List<CharacterInstance>(FindObjectsOfType<CharacterInstance>());
        }

        if (allClues == null || allClues.Count == 0)
        {
            // Busca todos los objetos que tengan el script ClueInstance en la escena
            allClues = new List<ClueInstance>(FindObjectsOfType<ClueInstance>());
        }

        // Validación de seguridad
        if (allCharacters.Count < 8 || allClues.Count < 8)
        {
            Debug.LogError("¡CUIDADO! No he encontrado suficientes personajes o pistas. " +
                           "Asegúrate de que todos tienen sus scripts puestos (CharacterInstance y ClueInstance).");
            return; // Paramos para evitar errores mayores
        }

        // Ejecutar la lógica normal
        SetupGame();
        
        if (showDebugLog) LogDebugState();
    }

    private void SetupGame()
    {
        // 1. Barajar
        List<CharacterInstance> shuffledList = allCharacters.OrderBy(x => Random.value).ToList();

        // 2. Asignar Roles (Guardamos referencias en listas globales para el debug)
        activeKillers = shuffledList.Take(3).ToList();
        activeVictims = shuffledList.Skip(3).Take(2).ToList();
        activeNormals = shuffledList.Skip(5).ToList();

        // Inicializar NPCs
        foreach (var chara in activeKillers) chara.InitializeRole(true, false);
        foreach (var chara in activeVictims) chara.InitializeRole(false, true);
        foreach (var chara in activeNormals) chara.InitializeRole(false, false);

        // 3. Configurar Pistas (Lógica anterior mantenida)
        SetupCluesLogic(activeKillers, activeVictims.Concat(activeNormals).ToList());
    }

    private void SetupCluesLogic(List<CharacterInstance> killers, List<CharacterInstance> nonKillers)
    {
        foreach(var clue in allClues) clue.gameObject.SetActive(false);

        // Pistas Asesinas (3)
        foreach (var killer in killers)
        {
            ClueInstance clue = FindClueForCharacter(killer.data);
            if(clue != null) clue.SetupClue(true);
        }

        // Pistas Normales (2 random de los inocentes)
        List<CharacterInstance> randomInnocents = nonKillers.OrderBy(x => Random.value).Take(2).ToList();
        foreach (var innocent in randomInnocents)
        {
            ClueInstance clue = FindClueForCharacter(innocent.data);
            if (clue != null) clue.SetupClue(false);
        }
    }

    // ------------------------------------------------------------------------
    // SISTEMA DE DEBUGGING Y VALIDACIÓN
    // ------------------------------------------------------------------------
    private void LogDebugState()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("<color=#FFA500>=== REPORT DEL SORTEO (GAME JAM) ===</color>");

        // 1. REPORTE DE ASESINOS Y SUS OBJETIVOS
        sb.AppendLine("\n<b>[ASESINOS ACTIVOS (3)]</b>");
        foreach (var killer in activeKillers)
        {
            CharacterID targetID = GetTargetVictimID(killer.data.characterID);
            
            // Verificación Lógica: ¿Está su objetivo en la lista de víctimas activas?
            bool isTargetAVictim = activeVictims.Any(v => v.data.characterID == targetID);
            string matchStatus = isTargetAVictim ? "<color=green>[MATCH]</color>" : "<color=red>[MISS]</color>";
            
            sb.AppendLine($" ► <b>{killer.data.characterName}</b> quiere matar a: {targetID} {matchStatus}");
        }

        // 2. REPORTE DE VÍCTIMAS
        sb.AppendLine("\n<b>[VÍCTIMAS ACTIVAS (2)]</b>");
        foreach (var victim in activeVictims)
        {
            sb.AppendLine($" ► <b>{victim.data.characterName}</b> (Asustado)");
        }

        // 3. REPORTE DE PISTAS (ESTADO VISUAL)
        sb.AppendLine("\n<b>[ESTADO DE PISTAS]</b>");
        foreach (var clue in allClues)
        {
            if (clue.gameObject.activeSelf)
            {
                // Obtenemos si es incriminatoria chequeando su descripción actual vs la data
                // Esto es un hack rápido para debug, idealmente ClueInstance tendría una propiedad pública 'IsIncriminating'
                bool isIncriminating = clue.GetDescription() == clue.ownerData.incriminatingClueDescription;
                string state = isIncriminating ? "<color=red>INCRIMINATORIA</color>" : "<color=cyan>NORMAL</color>";
                
                sb.AppendLine($" 🔎 {clue.ownerData.clueItemName} ({clue.ownerData.characterName}): {state}");
            }
            else
            {
                sb.AppendLine($" <color=grey>❌ {clue.ownerData.clueItemName}: Oculta</color>");
            }
        }

        Debug.Log(sb.ToString());
    }

    // Helpers y Switch (Igual que antes)
    private ClueInstance FindClueForCharacter(CharacterDataSO data) => allClues.FirstOrDefault(c => c.ownerData == data);
    
    public CharacterID GetTargetVictimID(CharacterID killerID)
    {
        switch (killerID)
        {
            case CharacterID.DonaldTrump: return CharacterID.NickiMinaj;
            case CharacterID.ShaquilleONeal: return CharacterID.BelenEsteban;
            case CharacterID.ElonMusk: return CharacterID.DonaldTrump;
            case CharacterID.Torrente: return CharacterID.TimotheeChalamet;
            case CharacterID.KanyeWest: return CharacterID.ElonMusk;
            case CharacterID.BelenEsteban: return CharacterID.ShaquilleONeal;
            case CharacterID.TimotheeChalamet: return CharacterID.Torrente;
            case CharacterID.NickiMinaj: return CharacterID.KanyeWest;
            default: return CharacterID.DonaldTrump;
        }
    }
}