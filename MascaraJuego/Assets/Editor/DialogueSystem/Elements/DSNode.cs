using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

namespace DS.Elements
{
    using UnityEngine.UIElements;
    using DS.Windows;
    using Enumerations;
    using System;
    using Utilities;
    using Data.Save;
    using System.Linq;
    using UnityEditor.UIElements;

    public class DSNode : Node
    {
        public string ID { get; set; }
        public string DialogueName { get; set; }
        public List<DSChoiceSaveData> Choices { get; set; }
        public string Text { get; set; }
        public string NameSpeaker { get; set; }
        public DSDialogueType DialogueType { get; set; }
        public DSGroup Group { get; set; }
        public string DialogueAudio { get; set; }
        public string DialogueSpeed { get; set; }
        public Sprite CharacterLeft { get; set; }
        public string DialogueAction { get; set; }
        
        protected DSGraphView graphView;

        private Color defaultBackgroundColor;

        private Image leftImageView;
        
        public virtual void Initialize(string nodeName, DSGraphView dsGraphView, Vector2 position)
        {
            ID = Guid.NewGuid().ToString();
            DialogueName = nodeName;
            Choices = new List<DSChoiceSaveData>();
            Text = "Dialogue text.";

            graphView = dsGraphView;
            defaultBackgroundColor = new Color(29f / 255, 29f / 255, 30f / 255);

            SetPosition(new Rect(position, Vector2.zero));

            mainContainer.AddToClassList("ds-node__main-container");
            extensionContainer.AddToClassList("ds_node__extension-container");
        }

        public virtual void Draw()
        {
            // TITLE CONTAINER
            TextField dialogueNameTextField = DSElementUtility.CreateTextField(DialogueName, null, callback =>
            {
                TextField target = (TextField)callback.target;
                target.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();

                if (string.IsNullOrEmpty(target.value))
                {
                    if (!string.IsNullOrEmpty(DialogueName))
                    {
                        ++graphView.NameErrorsAmount;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(DialogueName))
                        {
                            --graphView.NameErrorsAmount;
                        }
                    }
                }

                if (Group == null)
                {
                    graphView.RemoveUngroupedNode(this);
                    DialogueName = target.value;
                    graphView.AddUngroupedNode(this);
                    return;
                }

                DSGroup currentGroup = Group;
                graphView.RemovedGroupedNode(this, Group);
                DialogueName = target.value;
                graphView.AddGroupedNode(this, currentGroup);
            });

            dialogueNameTextField.AddClasses(
                "ds-node__textfield",
                "ds-node__filename-textfield",
                "ds-node__textfield__hidden"
            );

            titleContainer.Insert(0, dialogueNameTextField);

            // INPUT CONTAINER
            Port inputPort = this.CreatePort("Dialogue Connection", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Dialogue Connection";
            inputContainer.Add(inputPort);

            // EXTENSION CONTAINER
            VisualElement customDataContainer = new VisualElement();
            customDataContainer.AddToClassList("ds-node__custom-data-container");

            VisualElement imageDataContainer = new VisualElement();
            imageDataContainer.AddToClassList("ds-node__custom-data-container");

            // Speaker Name TextField
            Label speakerNameLabel = new Label("Speaker Name");
            speakerNameLabel.AddToClassList("ds-node__dialogue-label");
            TextField speakerNameTextField = DSElementUtility.CreateTextArea(NameSpeaker, null, callback =>
            {
                NameSpeaker = callback.newValue;
            });

            speakerNameTextField.AddClasses(
                "ds-node__textfield",
                "ds-node__speakerNameTextfield"
            );

            // Dialogue Text Field
            Label dialogueLabel = new Label("Dialogue Text");
            dialogueLabel.AddToClassList("ds-node__dialogue-label");
            TextField textTextField = DSElementUtility.CreateTextArea(Text, null, callback =>
            {
                Text = callback.newValue;
            });
            textTextField.AddClasses(
                "ds-node__textfield",
                "ds-node__quote-textfield"
            );

            // DIALOGUE CONTENT FOLDOUT
            Foldout textFoldout = DSElementUtility.CreateFouldout("Dialogue Content");
            textFoldout.Add(speakerNameLabel);
            textFoldout.Add(speakerNameTextField);
            textFoldout.Add(dialogueLabel);
            textFoldout.Add(textTextField);
            customDataContainer.Add(textFoldout);

            // Dialogue Audio Name Foldout
            Label audioLabel = new Label("Audio Name");
            audioLabel.AddToClassList("ds-node__dialogue-label");
            TextField dialogueAudioTextField = DSElementUtility.CreateTextField(DialogueAudio, null, callback =>
            {
                DialogueAudio = callback.newValue;
            });
            dialogueAudioTextField.AddClasses(
                "ds-node__textfield",
                "ds-node__audio-textfield"
            );

            // Dialogue Speed Parameter 
            Label speedLabel = new Label("Speed rc: (0.01)");
            speedLabel.AddToClassList("ds-node__dialogue-label");
            TextField dialogueSpeedTextField = DSElementUtility.CreateTextField(DialogueSpeed, null, callback =>
            {
                DialogueSpeed = callback.newValue;
            });
            dialogueSpeedTextField.AddClasses(
                "ds-node__textfield",
                "ds-node__speed-textfield"
            );

            // Left Character Image Field and Preview
            Label CharacterleftImageLabel = new Label("Left Character Image");
            CharacterleftImageLabel.AddToClassList("ds-node__dialogue-label");

            ObjectField CharacterLeftImageField = DSElementUtility.CreateImageArea(
                sprite: CharacterLeft != null ? CharacterLeft : null
            );
            CharacterLeftImageField.AddToClassList("ds-node__image-field");

            // Crear vista previa de imagen para izquierda
            leftImageView = new Image
            {
                scaleMode = ScaleMode.StretchToFill,
                image = CharacterLeft != null ? CharacterLeft.texture : null,
                style = {
                    maxWidth = 200,
                    maxHeight = 200,
                    marginBottom = 10
                }
            };

            CharacterLeftImageField.RegisterValueChangedCallback(evt =>
            {
                CharacterLeft = (Sprite)evt.newValue;
                leftImageView.image = CharacterLeft != null ? CharacterLeft.texture : null;
            });

            ////DROPDOWN ACCIONES

            List<string> dropdownActions = new List<string> { "Nothing", "Dice", "Fight","Suborn","GiveObject","GiveAbility" };
            
            DropdownField actionsDropdown = DSElementUtility.CreateDropdown(
                "ActionsEnd",
                dropdownActions,
                DialogueAction,
                callback =>
                {
                    DialogueAction = callback.newValue.ToString();
                    Debug.Log("actionName: " + DialogueAction);
                }
            );

            Foldout imagesFoldout = DSElementUtility.CreateFouldout("Dialogue Images");
            imagesFoldout.Add(CharacterleftImageLabel);
            imagesFoldout.Add(CharacterLeftImageField);
            imagesFoldout.Add(leftImageView);  
            imageDataContainer.Add(imagesFoldout);

            // Dialogue Properties Foldout
            Foldout propertiesFoldout = DSElementUtility.CreateFouldout("Dialogue Properties");
            propertiesFoldout.Add(audioLabel);
            propertiesFoldout.Add(dialogueAudioTextField);
            propertiesFoldout.Add(speedLabel);
            propertiesFoldout.Add(dialogueSpeedTextField);
            propertiesFoldout.Add(actionsDropdown);
            customDataContainer.Add(propertiesFoldout);

            extensionContainer.Add(customDataContainer);
            extensionContainer.Add(imagesFoldout);
        }

        #region Overrided Methods
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Disconnect Input Ports", actionEvent => DisconnectInputPorts());
            evt.menu.AppendAction("Disconnect Output Ports", actionEvent => DisconnectOutpuPorts());

            base.BuildContextualMenu(evt);
        }
        #endregion

        #region Utility Methods
        public void DisconnectAllPorts()
        {
            DisconnectInputPorts();
            DisconnectOutpuPorts();
        }

        private void DisconnectInputPorts()
        {
            DisconnectPorts(inputContainer);
        }

        private void DisconnectOutpuPorts()
        {
            DisconnectPorts(outputContainer);
        }

        private void DisconnectPorts(VisualElement container)
        {
            foreach (Port port in container.Children())
            {
                if (!port.connected)
                {
                    continue;
                }

                graphView.DeleteElements(port.connections);
            }
        }

        public bool IsStartingNode()
        {
            Port inputPort = (Port)inputContainer.Children().First();
            return !inputPort.connected;
        }

        public void SetErrorStyle(Color color)
        {
            mainContainer.style.backgroundColor = color;
        }

        public void ResetStyle()
        {
            mainContainer.style.backgroundColor = defaultBackgroundColor;
        }
        #endregion
    }
}
