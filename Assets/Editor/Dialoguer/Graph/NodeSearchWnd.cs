using System.Collections.Generic;
using GreenGremlins.Dialoguer.Editor.Editor.Dialoguer.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GreenGremlins.Dialoguer.Editor
{
    public class NodeSearchWnd : ScriptableObject,ISearchWindowProvider
    {
        private EditorWindow _window;
        private DialoguerGraphView _graphView;

        private Texture2D _indentationIcon;
        
        public void Configure(EditorWindow window, DialoguerGraphView graphView)
        {
            _window = window;
            _graphView = graphView;
            
            _indentationIcon = new Texture2D(1,1);
            _indentationIcon.SetPixel(0,0,new Color(0,0,0,0));
            _indentationIcon.Apply();
        }
        
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Node"), 0),
                new SearchTreeGroupEntry(new GUIContent("Dialogue"), 1),
                new SearchTreeEntry(new GUIContent("Dialoguer Node", _indentationIcon))
                {
                    level = 2, userData = new DialoguerNode()
                },
                new SearchTreeEntry(new GUIContent("Comment Block",_indentationIcon))
                {
                    level = 1,
                    userData = new Group()
                }
            };

            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            //Editor window-based mouse position
            var mousePosition = _window.rootVisualElement.ChangeCoordinatesTo(_window.rootVisualElement.parent,
                context.screenMousePosition - _window.position.position);
            var graphMousePosition = _graphView.contentViewContainer.WorldToLocal(mousePosition);
            switch (SearchTreeEntry.userData)
            {
                case DialoguerNode dialogueNode:
                    _graphView.CreateNewDialogueNode("Dialogue Node",graphMousePosition);
                    return true;
                case Group group:
                    var rect = new Rect(graphMousePosition, _graphView.DefaultCommentBlockSize);
                     _graphView.CreateCommentBlock(rect);
                    return true;
            }
            return false;
        }
    }
}