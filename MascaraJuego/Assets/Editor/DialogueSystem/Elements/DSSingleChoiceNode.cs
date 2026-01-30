using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

namespace DS.Elements
{
    using Data.Save;
    using Enumerations;
    using Utilities;
    using Windows;

    public class DSSingleChoiceNode : DSNode
    {
        public override void Draw()
        {
            base.Draw();

            //outpout container (el autput del contenedor donde los nodos para arrastrar)

            foreach (DSChoiceSaveData choice in Choices)
            {
                Port choicePort = this.CreatePort(choice.Text);

                choicePort.userData = choice;

                outputContainer.Add(choicePort);


            }
            RefreshExpandedState();
        }


        public override void Initialize(string nodeName,DSGraphView graphView, Vector2 position)
        {
            base.Initialize( nodeName,graphView, position);

            DialogueType = DSDialogueType.SingleChoice;

            DSChoiceSaveData choiceData = new DSChoiceSaveData()
            {
                Text = "Next DIalogue"
            };

            Choices.Add(choiceData);
        }
    }
}

