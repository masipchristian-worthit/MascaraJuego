using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace DS.Data.Save
{
    using Enumerations;

    [Serializable]
    public class DSNodeSaveData
    {
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public string Text { get; set; }
        [field:SerializeField] public string SpeakerName { get; set; }
        [field: SerializeField] public List<DSChoiceSaveData> Choices { get; set; }
        [field: SerializeField] public string GroupID { get; set; }
        [field: SerializeField] public string DialogueSpeed { get; set; }
        [field: SerializeField] public string Audio { get; set; }
        [field: SerializeField] public DSDialogueType DialogueType { get; set; }
        [field: SerializeField] public Vector2 Position { get; set; }
        [field:SerializeField] public Sprite CharacterImageLeft { get; set; }
        [field:SerializeField] public string Actions { get; set; }
        
    }
}

