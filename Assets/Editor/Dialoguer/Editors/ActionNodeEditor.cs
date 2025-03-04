using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace GreenGremlins.Dialoguer.Editor.Editors
{
    
    [CustomEditor(typeof(DialoguerActionNode))]
    public class ActionNodeEditor : UnityEditor.Editor
    {
        private SerializedProperty propDialogueKey;
        private SerializedProperty propDialogueOptions;
        private SerializedProperty propEmotion;
        private ReorderableList dialogueOptions;
        private Vector2 scrollPos;
        
        private void OnEnable()
        {
            propDialogueKey = serializedObject.FindProperty("DialogueKey");
            propDialogueOptions = serializedObject.FindProperty("DialogueOptions");
            propEmotion = serializedObject.FindProperty("Emotion");
            dialogueOptions = new ReorderableList(serializedObject, serializedObject.FindProperty("DialogueOptions"),
                false, true, true, true);

            dialogueOptions.drawElementCallback =
                (rect, index, _, _) =>
                {
                    SerializedProperty element = dialogueOptions.serializedProperty.GetArrayElementAtIndex(index);
                    rect.y += 2;
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, 20, EditorGUIUtility.singleLineHeight),
                        new GUIContent(index.ToString()));
                    EditorGUI.PropertyField(
                        new Rect(rect.x + 20, rect.y, rect.width - 20, EditorGUIUtility.singleLineHeight),
                        element, GUIContent.none);
                };
        }

        public override void OnInspectorGUI()
        {
            DialoguerNodeData node = target as DialoguerNodeData;
            
            serializedObject.Update();

            GUILayout.Label(new GUIContent(serializedObject.targetObject.name), EditorStyles.boldLabel);
            
            EditorGUILayout.PropertyField(propDialogueKey);
            EditorGUILayout.PropertyField(propEmotion);
            
            EditorGUILayout.Separator();
            
            //scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(EditorGUIUtility.currentViewWidth-20), GUILayout.Height(230));
            dialogueOptions.DoLayoutList();
            //EditorGUILayout.EndScrollView();

            if (serializedObject.hasModifiedProperties)
            {
                serializedObject.ApplyModifiedProperties();
                node.OnNodeUpdated?.Invoke();
            }
        }
    }
}