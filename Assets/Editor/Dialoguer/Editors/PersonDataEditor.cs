using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GreenGremlins.Dialoguer.Editor.Editors
{
    [CustomEditor(typeof(PersonData))]
    public class PersonDataEditor : UnityEditor.Editor
    {
        private SerializedProperty propPersonName;
        private SerializedProperty propPersonId;
        private SerializedProperty propEmotions;
        private ReorderableList emotions;

        private void LoadEmotions()
        {
            Object[] objs = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(target));
            for (int i = 0; i < objs.Length; i++)
            {
                propEmotions.InsertArrayElementAtIndex(i);
                propEmotions.GetArrayElementAtIndex(i).objectReferenceValue = objs[i];
            }
            AssetDatabase.SaveAssets();
        }
        
        private void OnEnable()
        {
            propPersonName = serializedObject.FindProperty("Name");
            propPersonId = serializedObject.FindProperty("Id");
            propEmotions = serializedObject.FindProperty("Emotions");
            
            LoadEmotions();

            emotions = new ReorderableList(serializedObject, propEmotions,
                false, true, true, true);
            
            emotions.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "Emotions", EditorStyles.boldLabel);
            };
            
            emotions.drawElementCallback = (rect, index, _, _) =>
            {
                SerializedObject e = new SerializedObject(propEmotions.GetArrayElementAtIndex(index).objectReferenceValue);
                SerializedProperty emotion = e.FindProperty(Emotion.EMOTION_NAME);
                
                rect.y += 2;
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width/2f, EditorGUIUtility.singleLineHeight),
                    emotion, GUIContent.none);
                EditorGUI.PropertyField(new Rect(rect.x+rect.width/2f, rect.y, rect.width/2f, EditorGUIUtility.singleLineHeight),
                    e.FindProperty(Emotion.EMOTION_SPRITE), GUIContent.none);

                if (e.ApplyModifiedProperties())
                {
                    if (e.targetObject.name != emotion.enumNames[emotion.enumValueIndex])
                    {
                        e.targetObject.name = emotion.enumNames[emotion.enumValueIndex];
                        AssetDatabase.SaveAssets();
                    }
                }
            };

            emotions.onAddCallback = _ =>
            {
                propEmotions.arraySize++;
                propEmotions.GetArrayElementAtIndex(propEmotions.arraySize - 1).objectReferenceValue = CreateEmotion();
            };

            emotions.onRemoveCallback = list =>
            {
                DeleteEmotion(list.index);
            };
        }

        private Emotion CreateEmotion()
        {
            Emotion emotion = Emotion.Create();
            emotion.name = nameof(Emotion);
            emotion.name = emotion.emotion.ToString();
            AssetDatabase.AddObjectToAsset(emotion, serializedObject.targetObject);
            AssetDatabase.SaveAssets();
            return emotion;
        }

        private void DeleteEmotion(int idx)
        {
            Emotion emotion = propEmotions.GetArrayElementAtIndex(idx).objectReferenceValue as Emotion;
            propEmotions.GetArrayElementAtIndex(idx).objectReferenceValue = null;
            propEmotions.DeleteArrayElementAtIndex(idx);
            AssetDatabase.RemoveObjectFromAsset(emotion);
            DestroyImmediate(emotion, true);
            AssetDatabase.SaveAssets();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(propPersonName);
            EditorGUILayout.PropertyField(propPersonId);
            emotions.DoLayoutList();
            
            if (serializedObject.hasModifiedProperties)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}