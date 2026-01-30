using DS.Windows;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;

namespace DS.windows
{
    using System;
    using UnityEditor.UIElements;
    using Utilities;
    public class DSEditorWindows : EditorWindow
    {
        private DSGraphView graphView;
        private readonly string defaultFileName = "DialogueFileName";
        private static TextField fileNameTextField;
        private Button saveButton;
        private Button miniMapButton;

        [MenuItem("Window/DS/Dialogue Graph")]
        public static void Open()
        {
            GetWindow<DSEditorWindows>("Dialogue Graph");
            
        }
        private void OnEnable()
        {
            AddGraphView();
            AddToolBar();

            AddStyles();
        }
        #region Elements Addition

        private void AddGraphView()
        {
             graphView = new DSGraphView(this);

            graphView.StretchToParentSize();

            rootVisualElement.Add(graphView);
        }
        private void AddToolBar()
        {
            Toolbar toolbar = new Toolbar();

            fileNameTextField = DSElementUtility.CreateTextField(defaultFileName, "File Name:", callback =>
            {
                fileNameTextField.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
            });

            saveButton = DSElementUtility.CreateButton("Save", () => Save());

            Button loadButton = DSElementUtility.CreateButton("Load", () => Load());
            Button clearButton = DSElementUtility.CreateButton("Clear", () => Clear());
            Button resetButton = DSElementUtility.CreateButton("Reset", () => ResetGraph());
            Button fracasadoButton = DSElementUtility.CreateButton("Fracasado", () => fracasadoGraph());
            Button Ayuda = DSElementUtility.CreateButton("Ayuda", () => ayudaGraph());
            miniMapButton = DSElementUtility.CreateButton("Minimapa", () => ToggleMinimap());
            toolbar.Add(fileNameTextField);
            toolbar.Add(saveButton);
            toolbar.Add(loadButton);
            toolbar.Add(clearButton);
            toolbar.Add(resetButton);
            toolbar.Add(miniMapButton);
            toolbar.Add(fracasadoButton);
            toolbar.Add(Ayuda);

            toolbar.AddStyleSheets("DialogueSystem/DSToolBarStyles.uss");

            rootVisualElement.Add(toolbar);
        }

        

        private void AddStyles()
        {
            rootVisualElement.AddStyleSheets(
                "DialogueSystem/DSVariables.uss"

                );
        }
        #endregion
        #region ToolBar Actions
       
        private void Save()
        {
            if (string.IsNullOrEmpty(fileNameTextField.value))
            {
                EditorUtility.DisplayDialog(
                    "Nombre no valido, espabila.",
                    "Mira bien el nombre que hayas escrito que sea valido puede que este en blanco o algo no se",
                    "Vale tio, perdona era un error no me pegues no porfa, para nooo..."
                    );

                return;
            }

            DSIOUtility.Initialize(graphView, fileNameTextField.value);
            DSIOUtility.Save();
        }

        private void Load()
        {
            string filePath = EditorUtility.OpenFilePanel("Dialogue Graphs", "Assets/Editor/DialogueSystem/Graphs", "asset");

            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            Clear();

            DSIOUtility.Initialize(graphView, Path.GetFileNameWithoutExtension(filePath));

            DSIOUtility.Load();
        }


        private void Clear()
        {
            graphView.ClearGraph();
        }

        private void ResetGraph()
        {
            Clear();

            UpdateFileName(defaultFileName);
        }

        private void ToggleMinimap()
        {
            graphView.ToggleMiniMap();

            miniMapButton.ToggleInClassList("ds-toolbar__button__selected");
        }

        private void fracasadoGraph()
        {
            Application.OpenURL("https://www.youtube.com/watch?v=mofjeM7cVUo");
        }

        private void ayudaGraph()
        {
            Application.OpenURL("https://pastebin.com/94Sfej6K");
        }
        #endregion
        #region Utility Methods
        public static void UpdateFileName(string newFileName)
        {
            fileNameTextField.value = newFileName;
        }
        public void EnableSaving()
        {
            saveButton.SetEnabled(true);
        }

        public void DisableSaving()
        {
            saveButton.SetEnabled(false);
        }
        #endregion
    }
}

