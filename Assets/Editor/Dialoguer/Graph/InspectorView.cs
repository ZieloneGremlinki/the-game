using System;
using GreenGremlins.Dialoguer.Editor.Nodes;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace GreenGremlins.Dialoguer.Editor
{
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits>
        {
        }
        
        public InspectorView()
        {
            
        }

        public static DialoguerAsset currentDialogue;

        internal void UpdateSelection(DialoguerNode node)
        {
            try
            {
                Debug.Log($"Node: {node.data}");
                Clear();
                Object.DestroyImmediate(editor);
                editor = UnityEditor.Editor.CreateEditor(node.data);
                IMGUIContainer container = new IMGUIContainer(() =>
                {
                    if (editor.target)
                        editor.OnInspectorGUI();
                });
                Add(container);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        } 

        private UnityEditor.Editor editor;
    }
}