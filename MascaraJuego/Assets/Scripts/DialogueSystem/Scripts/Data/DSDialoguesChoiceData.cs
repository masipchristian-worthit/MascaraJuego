using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DS.Data
{
    using ScriptableObjects;
    using System;

    [Serializable]
    public class DSDialoguesChoiceData
    {
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public DSDialogueSO NextDialogue { get; set; }
    }
}

