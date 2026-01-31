using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DS.ScriptableObjects
{
    using Enumerations;
    using Data;
    using System;


    public class DSDialogueSO : ScriptableObject
    {
        [field: SerializeField] public string DialogueName { get; set; }
        [field: SerializeField] [field: TextArea()] public string Text { get; set; }
        [field: SerializeField] public string SpeakerName { get; set; }
        [field: SerializeField] public List<DSDialoguesChoiceData> Choices { get; set; }
        [field: SerializeField] public string Audio { get; set; }
        [field: SerializeField] public DSDialogueType DialogueType { get; set; }
        [field: SerializeField] public bool IsStartingDialogue { get; set; }
        [field: SerializeField] public string DialogueSpeed { get; set; }
        [field: SerializeField] public Sprite DialogueCharacterLeft { get; set; }
        [field: SerializeField] public string DialogueActionName { get; set; }
        [field: SerializeField] public HintSO HintSo { get; set; }
        

        public void Initialize(string dialogueName, string text, string speakerName,
            List<DSDialoguesChoiceData> choices, DSDialogueType dialogueType,
            string audio, bool isStartingDialogue,
            string dialogueSpeed, Sprite dialogueCharacterLeft, string actionName,HintSO hintSo)
        {
            DialogueName = dialogueName;
            Text = text;
            SpeakerName = speakerName;
            Choices = choices;
            DialogueType = dialogueType;
            IsStartingDialogue = isStartingDialogue;
            Audio = audio;
            DialogueSpeed = dialogueSpeed;
            DialogueCharacterLeft = dialogueCharacterLeft;
            DialogueActionName = actionName;
            HintSo = hintSo;
        }
    }

}
