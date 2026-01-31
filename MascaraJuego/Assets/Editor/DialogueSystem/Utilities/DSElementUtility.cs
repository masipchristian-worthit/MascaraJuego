using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Utilities
{
    using Elements;
    using UnityEditor.UIElements;

    public static class DSElementUtility 
    {

        public static Button CreateButton(string text, Action onClick = null)
        {
            Button button = new Button(onClick)
            {
                text = text,
            };
            return button;
        }
        public static Foldout CreateFouldout(string title, bool collaped = false)
        {
            Foldout foldout = new Foldout()
            {
                text = title,
                value = !collaped
            };
            return foldout;
        }

        public static Port CreatePort(this DSNode node, string portName = "", Orientation orientation = Orientation.Horizontal, Direction direction = Direction.Output, Port.Capacity capacity = Port.Capacity.Single)
        {
            Port port = node.InstantiatePort(orientation, direction, capacity, typeof(bool));

            port.portName = portName;

            return port;
        }

        public static TextField CreateTextField(string value = null,string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            TextField textField = new TextField()
            {
                value = value,
                label = label,
            };

            if (onValueChanged != null)
            {
                textField.RegisterValueChangedCallback(onValueChanged);
            }

            return textField;
        }

        public static TextField CreateTextArea(string value = null, string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            TextField textArea = CreateTextField(value,label, onValueChanged);

            textArea.multiline = true;

            return textArea;
        }

        public static ObjectField CreateImageArea(Sprite sprite = null)
        {
            ObjectField imageField = new ObjectField
            {
                objectType = typeof(Sprite),
                allowSceneObjects = false,
                value = sprite
            };

            return imageField;
        }
        
        public static ObjectField CreateScriptableObjectArea<T>(
            T value = null,
            string label = null
        ) where T : ScriptableObject
        {
            var objectField = new ObjectField
            {
                label = label,
                objectType = typeof(T),
                allowSceneObjects = false,
                value = value 
            };

            objectField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue != null)
                {
                    value = (T)evt.newValue;
                }
            });

            return objectField;
        }

        public static DropdownField CreateDropdown( string label = null, List<string> options = null, string selectedOption = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
           
            var dropdown = new DropdownField
            {
                label = label,
                choices = options ?? new List<string>(),
                value = selectedOption
            };

           
            if (onValueChanged != null)
            {
                dropdown.RegisterValueChangedCallback(onValueChanged);
            }

            return dropdown;
        }


    }

}


