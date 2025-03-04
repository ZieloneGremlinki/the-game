using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GreenGremlins.Dialoguer.Editor.Editors
{
    //[CustomEditor(typeof(PersonData))]
    public class PersonDataEditor : UnityEditor.Editor
    {
        private SerializedProperty propPersonName;
        private SerializedProperty propEmotions;
        private ReorderableList emotions;
        
        private void OnEnable()
        {
            propPersonName = serializedObject.FindProperty("Name");
            propEmotions = serializedObject.FindProperty("Emotions");

            emotions = new ReorderableList(serializedObject, propEmotions,
                false, true, true, true);
            
            emotions.drawElementCallback = (rect, index, _, _) =>
            {
                SerializedProperty el = emotions.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    el.FindPropertyRelative("emotion"), GUIContent.none);
            };

            emotions.onAddCallback = (lst) =>
            {
                propEmotions.arraySize++;
                Emotion emotion = CreateInstance(typeof(Emotion)) as Emotion;
                AssetDatabase.AddObjectToAsset(serializedObject.targetObject, emotion);
                
                emotions.list[emotions.list.Count] = emotion;
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(propPersonName);
            emotions.DoLayoutList();
            
            if (serializedObject.hasModifiedProperties)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}