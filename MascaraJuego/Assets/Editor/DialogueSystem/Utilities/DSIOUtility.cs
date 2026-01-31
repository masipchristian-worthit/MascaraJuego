using DS.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using DS.Data.Save;

namespace DS.Utilities
{


    using Data;
    using Elements;
    using ScriptableObjects;
    using Windows;
    using DS.windows;
    

    public static class DSIOUtility
    {

        private static DSGraphView graphView;
        private static string graphFileName;
        private static string containerFolderPath;

        private static List<DSGroup> groups;
        private static List<DSNode>nodes;

        private static Dictionary<string, DSDialogueGroupSO> createdDialogueGroups;
        private static Dictionary<string, DSDialogueSO> createdDialogues;

        private static Dictionary<string, DSGroup> loadedGroups;
        private static Dictionary<string, DSNode> loadedNodes;
        public static void Initialize( DSGraphView dSGraphView,  string graphName)
        {
            graphView = dSGraphView;
            graphFileName = graphName;
            containerFolderPath = $"Assets/DialogueSystem/Dialogues/{graphFileName}";

            groups = new List<DSGroup>();
            nodes = new List<DSNode>();

            createdDialogueGroups = new Dictionary<string, DSDialogueGroupSO>();
            createdDialogues = new Dictionary<string, DSDialogueSO>();

            loadedGroups = new Dictionary<string, DSGroup>();
            loadedNodes = new Dictionary<string, DSNode>();
        }


        #region Save Methods
        public static void Save()
        {
            CreateStaticFolders();

            GetElementsFromGraphView();

            DSGraphSaveData graphData = CreateAsset<DSGraphSaveData>("Assets/Editor/DialogueSystem/Graphs", $"{graphFileName}Graph");

            graphData.Initialize(graphFileName);

            DSDialogueContainerSO dialogueContainer = CreateAsset<DSDialogueContainerSO>(containerFolderPath, graphFileName);

            dialogueContainer.Initialize(graphFileName);

            SaveGroups(graphData, dialogueContainer);
            SaveNodes(graphData, dialogueContainer);

            SaveAsset(graphData);
            SaveAsset(dialogueContainer);
        }

        #region Groups
        private static void SaveGroups(DSGraphSaveData graphData, DSDialogueContainerSO dialogueContainer)
        {
            List <string> groupNames = new List<string>();
            foreach (DSGroup group in groups)
            {
                SaveGroupToGraph(group, graphData);
                SaveGroupToScriptableObject(group, dialogueContainer);

                groupNames.Add(group.title);
            }

            UpdateOldGroups(groupNames,graphData);
        }

        private static void SaveGroupToGraph(DSGroup group, DSGraphSaveData graphData)
        {
            DSGroupSaveData groupData = new DSGroupSaveData()
            {
                ID = group.ID,
                Name = group.title,
                Position = group.GetPosition().position
            };

            graphData.Groups.Add(groupData);
        }

        private static void SaveGroupToScriptableObject(DSGroup group, DSDialogueContainerSO dialogueContainer)
        {
            string groupName = group.title;

            CreateFolder($"{containerFolderPath}/Groups", groupName);

            CreateFolder($"{containerFolderPath}/Groups/{groupName}", "Dialogues");

            DSDialogueGroupSO dialogueGroup = CreateAsset<DSDialogueGroupSO>($"{containerFolderPath}/Groups/{groupName}", groupName);

            dialogueGroup.Initialize(groupName);

            createdDialogueGroups.Add(group.ID, dialogueGroup);

            dialogueContainer.DialogueGroups.Add(dialogueGroup, new List<DSDialogueSO>());

            SaveAsset(dialogueGroup);
        }

        private static void UpdateOldGroups(List <string> currentGroupnames, DSGraphSaveData graphData)
        {
            if (graphData.OldGroupNames != null && graphData.OldGroupNames.Count !=0)
            {
                List<string> groupsToRemove = graphData.OldGroupNames.Except(currentGroupnames).ToList();

                foreach (string groupToRemove in groupsToRemove)
                {
                    RemoveFolder($"{containerFolderPath}/Groups/{groupsToRemove}");
                }
            }

            graphData.OldGroupNames = new List<string>(currentGroupnames);
        }

       

        #endregion

        #region Nodes
        private static void SaveNodes(DSGraphSaveData graphData, DSDialogueContainerSO dialogueContainer)
        {
            SerializableDictionary<string, List<string>> groupedNodeNames = new SerializableDictionary<string, List<string>>();
            List<string> ungroupedNodeNames = new List<string>();
            foreach(DSNode node in nodes)
            {
                SaveNodeToGraph(node, graphData);
                SaveNodeToScriptableObject(node, dialogueContainer);
                
                if (node.Group != null)
                {
                    groupedNodeNames.AddItem(node.Group.title, node.DialogueName);

                    continue;
                }

                ungroupedNodeNames.Add(node.DialogueName);
            }

            UpdateDialogueChoicesConnections();

            UpdateOldGroupedNodes(groupedNodeNames, graphData);

            UpdateOldUngroupedNodes(ungroupedNodeNames, graphData);
        }

        

        private static void SaveNodeToGraph(DSNode node, DSGraphSaveData graphData)
        {
            List<DSChoiceSaveData> choices = CloneNodeChoices(node.Choices);

            DSNodeSaveData nodeData = new DSNodeSaveData()
            {
                ID = node.ID,
                Name = node.DialogueName,
                
                SpeakerName = node.NameSpeaker,
                DialogueSpeed = node.DialogueSpeed,
                Audio = node.DialogueAudio,
                Choices = choices,
                Text = node.Text,
                GroupID = node.Group?.ID,
                DialogueType = node.DialogueType,
                Position = node.GetPosition().position,
                CharacterImageLeft = node.CharacterLeft,
                Actions = node.DialogueAction,
                HintSo = node.HintSO
            };

            graphData.Nodes.Add(nodeData);
        }

        private static void SaveNodeToScriptableObject(DSNode node, DSDialogueContainerSO dialogueContainer)
        {
            DSDialogueSO dialogue;

            if (node.Group != null)
            {
                dialogue = CreateAsset<DSDialogueSO>($"{containerFolderPath}/Groups/{node.Group.title}/Dialogues", node.DialogueName);

                dialogueContainer.DialogueGroups.AddItem(createdDialogueGroups[node.Group.ID], dialogue);
            }
            else
            {
                dialogue = CreateAsset<DSDialogueSO>($"{containerFolderPath}/Global/Dialogues", node.DialogueName);

                dialogueContainer.UngroupedDialogues.Add(dialogue);
            }

            dialogue.Initialize(
                node.DialogueName,
                node.Text,
                node.NameSpeaker,
                ConvertNodeChoicesToDialogueChoices(node.Choices),
                node.DialogueType,
                node.DialogueAudio, 
                node.IsStartingNode(),
                node.DialogueSpeed,
                node.CharacterLeft,
                node.DialogueAction,
                node.HintSO
                );

            createdDialogues.Add(node.ID, dialogue);

            SaveAsset(dialogue);
        }

        private static List<DSDialoguesChoiceData> ConvertNodeChoicesToDialogueChoices(List<DSChoiceSaveData> nodeChoices)
        {
            List<DSDialoguesChoiceData> dialoguesChoices = new List<DSDialoguesChoiceData>();

            foreach (DSChoiceSaveData nodechoice in nodeChoices)
            {
                DSDialoguesChoiceData choiceData = new DSDialoguesChoiceData()
                {
                    Text = nodechoice.Text,
                };

                dialoguesChoices.Add(choiceData);
            }
            return dialoguesChoices;
        }

        private static void UpdateDialogueChoicesConnections()
        {
            foreach (DSNode node in nodes)
            {
                DSDialogueSO dialogue = createdDialogues[node.ID];

                for (int choiceIndex = 0; choiceIndex < node.Choices.Count; ++choiceIndex)
                {
                    DSChoiceSaveData nodeChoice = node.Choices[choiceIndex];

                    if (string.IsNullOrEmpty(nodeChoice.NodeID))
                    {
                        continue;
                    }

                    dialogue.Choices[choiceIndex].NextDialogue = createdDialogues[nodeChoice.NodeID];

                    SaveAsset(dialogue);
                }

            }
        }

        private static void UpdateOldGroupedNodes(SerializableDictionary<string, List<string>> currentGroupedNodeNames, DSGraphSaveData graphData)
        {
            if (graphData.OldGroupedNodeNames != null && graphData.OldGroupedNodeNames.Count != 0)
            {
                foreach (KeyValuePair<string, List<string>> oldGroupedNode in graphData.OldGroupedNodeNames)
                {
                    List<string> nodesToRemove = new List<string>();

                    if (currentGroupedNodeNames.ContainsKey(oldGroupedNode.Key))
                    {
                        nodesToRemove = oldGroupedNode.Value.Except(currentGroupedNodeNames[oldGroupedNode.Key]).ToList();
                    }

                    foreach (string nodeToRemove in nodesToRemove)
                    {
                        RemoveAsset($"{containerFolderPath}/Groups/{oldGroupedNode.Key}/Dialogues", nodeToRemove);
                    }
                }
            }

            graphData.OldGroupedNodeNames = new SerializableDictionary<string, List<string>>(currentGroupedNodeNames);
        }
        private static void UpdateOldUngroupedNodes(List<string> currentUngroupedNodeNames, DSGraphSaveData graphData)
        {
            if (graphData.OldUngroupedNodesNames != null && graphData.OldUngroupedNodesNames.Count != 0)
            {
                List<string> nodesToRemove = graphData.OldUngroupedNodesNames.Except(currentUngroupedNodeNames).ToList();

                foreach (string nodeToRemove in nodesToRemove)
                {
                    RemoveAsset($"{containerFolderPath}/Global/Dialogues", nodeToRemove);
                }
            }

            graphData.OldUngroupedNodesNames = new List<string>(currentUngroupedNodeNames);
        }

        #endregion

        #endregion
        #region Load methods

        public static void Load()
        {
            DSGraphSaveData graphData = LoadAsset<DSGraphSaveData>("Assets/Editor/DialogueSystem/Graphs", graphFileName);

            if (graphData == null)
            {
                EditorUtility.DisplayDialog(
                    "El archivo no se ha podido cargar, dia triste",
                    "El archivo no se ha podido encontrar en la siguiente direccion: /n/n" +
                    $"Assets/Editor/DialogueSystem/Graphs/{graphFileName}/n/n" +
                    "Asegurate que el archivo esta en la direccion de arriba, empanao",
                    "Suerte!"
                    );
                return;
            }

            DSEditorWindows.UpdateFileName( graphData.FileName );

            LoadGroups(graphData.Groups);
            LoadNodes(graphData.Nodes);
            LoadNodesConnections();
        }

        private static void LoadGroups(List<DSGroupSaveData> groups)
        {
            foreach(DSGroupSaveData groupData in groups)
            {
                DSGroup group = graphView.CreateGroup(groupData.Name, groupData.Position);

                group.ID = groupData.ID;

                loadedGroups.Add(group.ID, group);
            }
        }

        private static void LoadNodes(List<DSNodeSaveData> nodes)
        {
            foreach(DSNodeSaveData nodeData in nodes)
            {
                List<DSChoiceSaveData> choices = CloneNodeChoices(nodeData.Choices);

                DSNode node = graphView.CreateNode(nodeData.Name,nodeData.DialogueType, nodeData.Position,false);
                node.ID = nodeData.ID;
                node.Choices = choices;
                node.Text = nodeData.Text;
                node.NameSpeaker = nodeData.SpeakerName;
                node.DialogueSpeed = nodeData.DialogueSpeed;
                node.DialogueAudio = nodeData.Audio;
                node.CharacterLeft = nodeData.CharacterImageLeft;
                node.DialogueAction = nodeData.Actions;
                node.HintSO = nodeData.HintSo;

                node.Draw();

                graphView.AddElement(node);

                loadedNodes.Add(node.ID, node);

                if (string.IsNullOrEmpty(nodeData.GroupID))
                {
                    continue;
                }

                DSGroup group = loadedGroups[nodeData.GroupID];

                node.Group = group;

                group.AddElement(node);
            }
        }

        private static void LoadNodesConnections()
        {
            foreach(KeyValuePair<string, DSNode> loadedNode in loadedNodes)
            {
                foreach (Port choicePort in loadedNode.Value.outputContainer.Children())
                {
                    DSChoiceSaveData choiceData = (DSChoiceSaveData) choicePort.userData;
                    
                    if (string.IsNullOrEmpty(choiceData.NodeID))
                    {
                        continue;
                    }

                    DSNode nextNode = loadedNodes[choiceData.NodeID];

                    Port nextNodeInputPort = (Port) nextNode.inputContainer.Children().First();

                    Edge edge = choicePort.ConnectTo(nextNodeInputPort);

                    graphView.AddElement(edge);

                    loadedNode.Value.RefreshPorts();
                }
            }
        }
        #endregion

        #region Creation Methods
        private static void CreateStaticFolders()
        {
            CreateFolder("Assets/Editor/DialogueSystem", "Graphs");

            CreateFolder("Assets", "DialogueSystem");

            CreateFolder("Assets/DialogueSystem", "Dialogues");

            CreateFolder("Assets/DialogueSystem/Dialogues/", graphFileName);
            CreateFolder(containerFolderPath, "Global");
            CreateFolder(containerFolderPath, "Groups");
            CreateFolder($"{containerFolderPath}/Global", "Dialogues");
        }
        #endregion

        #region Fetch Methods
        private static void GetElementsFromGraphView()
        {
            Type groupType = typeof(DSGroup);

            graphView.graphElements.ForEach(graphElement =>
            {
                if (graphElement is DSNode node)
                {
                    nodes.Add(node);
                    return;
                }

                if(graphElement.GetType() == groupType)
                {
                    DSGroup group = (DSGroup)graphElement;

                    groups.Add(group);

                    return;
                }
            });
        }

        #endregion

        #region Utility Methods
        public static void CreateFolder(string path, string folderName)
        {
            if (AssetDatabase.IsValidFolder($"{path}/{folderName}"))
            {
                return;
            }

            AssetDatabase.CreateFolder(path, folderName);
        }

        public static void RemoveFolder(string fullPath)
        {
            FileUtil.DeleteFileOrDirectory($"{fullPath}.meta");
            FileUtil.DeleteFileOrDirectory($"{fullPath}/");
        }

        public static T CreateAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            // Verifica si el directorio existe, si no lo crea IMPORTANTE SI NO NO SE CREA
            if (!AssetDatabase.IsValidFolder(path))
            {
                string[] folders = path.Split('/');
                string currentPath = "";
                foreach (string folder in folders)
                {
                    if (string.IsNullOrEmpty(currentPath))
                    {
                        currentPath = folder;
                    }
                    else
                    {
                        if (!AssetDatabase.IsValidFolder(currentPath + "/" + folder))
                        {
                            AssetDatabase.CreateFolder(currentPath, folder);
                        }
                        currentPath += "/" + folder;
                    }
                }
            }

            string fullPath = $"{path}/{assetName}.asset";
            T asset = LoadAsset<T>(path, assetName);

            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();

                try
                {
                    AssetDatabase.CreateAsset(asset, fullPath);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to create asset at {fullPath}: {e.Message}");
                    throw;
                }
            }

            return asset;
        }

        public static T LoadAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            string fullPath = $"{path}/{assetName}.asset";
            return AssetDatabase.LoadAssetAtPath<T>(fullPath);
        }

        public static void RemoveAsset(string path, string assetName)
        {
            AssetDatabase.DeleteAsset($"{path}/{assetName}.asset");
        }

        public static void SaveAsset(UnityEngine.Object asset)
        {
            EditorUtility.SetDirty(asset);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static List<DSChoiceSaveData> CloneNodeChoices(List<DSChoiceSaveData> nodeChoices)
        {
            List<DSChoiceSaveData> choices = new List<DSChoiceSaveData>();

            foreach (DSChoiceSaveData choice in nodeChoices)
            {
                DSChoiceSaveData choiceData = new DSChoiceSaveData()
                {
                    Text = choice.Text,
                    NodeID = choice.NodeID,
                };

                choices.Add(choiceData);
            }

            return choices;
        }
        #endregion
    }
}


