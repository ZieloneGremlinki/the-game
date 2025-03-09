using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace GreenGremlins.Dialoguer.Editor.Editors
{
    
    [CustomEditor(typeof(DialoguerFunctionNode))]
    public class FunctionNodeEditor : UnityEditor.Editor
    {
        private SerializedProperty propFunction;
        
        private void OnEnable()
        {
            propFunction = serializedObject.FindProperty(DialoguerFunctionNode.FUNCTION_NAME);
        }

        public override void OnInspectorGUI()
        {
            DialoguerNodeData node = target as DialoguerNodeData;
            
            serializedObject.Update();

            GUILayout.Label(new GUIContent(serializedObject.targetObject.name), EditorStyles.boldLabel);
            
            EditorGUILayout.PropertyField(propFunction);

            if (serializedObject.hasModifiedProperties)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}