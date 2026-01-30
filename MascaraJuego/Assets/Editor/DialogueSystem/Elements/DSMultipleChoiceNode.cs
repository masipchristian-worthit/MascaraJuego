using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using DS.Enumerations;
using DS.Utilities;

namespace DS.Elements
{
    using Data.Save;
    using Windows;
    public class DSMultipleChoiceNode : DSNode
    {
        public override void Initialize(string nodeName, DSGraphView graphView, Vector2 position)
        {
            base.Initialize(nodeName, graphView, position);

            DialogueType = DSDialogueType.MultipleChoice;

            DSChoiceSaveData choiceData = new DSChoiceSaveData()
            {
                Text = "New Choice"
            };

            Choices.Add(choiceData);  
        }

        public override void Draw()
        {
            base.Draw();

            Button addChoiceButton = DSElementUtility.CreateButton("Add Choice", () =>
            {
                DSChoiceSaveData choiceData = new DSChoiceSaveData()
                {
                    Text = "New Choice"
                };

                Choices.Add(choiceData);

                Port choicePort = CreateChoicePort(choiceData);

                outputContainer.Add(choicePort);

            });

            addChoiceButton.AddToClassList("ds-node__button");
            mainContainer.Insert(1, addChoiceButton);

            foreach (DSChoiceSaveData choice in Choices)
            {
                Port choicePort = CreateChoicePort(choice);

                outputContainer.Add(choicePort);
            }

            RefreshExpandedState();
        }

        private Port CreateChoicePort(object userData)
        {
            Port choicePort = this.CreatePort();

            choicePort.userData = userData;

            DSChoiceSaveData choiceData = (DSChoiceSaveData)userData;

            Button deleteChoiceButton = DSElementUtility.CreateButton("X", () =>
            {
                if (Choices.Count ==1)
                {
                    return;
                }
                if (choicePort.connected)
                {
                    graphView.DeleteElements(choicePort.connections);
                }
                
                Choices.Remove(choiceData);

                graphView.RemoveElement(choicePort);
            });

            deleteChoiceButton.AddToClassList("ds-node__button");

            
            TextField choiceTextField = DSElementUtility.CreateTextField(choiceData.Text, null, callback =>
            {
                choiceData.Text = callback.newValue;
            });

            choiceTextField.AddToClassList("ds-node__textfield");
            choiceTextField.AddToClassList("ds-node__choice-textfield");
            choiceTextField.style.flexGrow = 1;

            VisualElement choiceContainer = new VisualElement();
            choiceContainer.AddToClassList("ds-node__choice-container");
            choiceContainer.Add(deleteChoiceButton);
            choiceContainer.Add(choiceTextField);

            choicePort.Add(choiceContainer);

            return choicePort;
        }
    }
}
