using System;
using System.Linq;
using GreenGremlins.Dialoguer.Editor.Editor.Dialoguer;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GreenGremlins.Dialoguer.Editor
{
    public class DialoguerGraph : EditorWindow
    {
        private string _fileName = "NewDialogue";

        private DialoguerGraphView _graphView;
        private DialoguerContainer _dialoguerContainer;

        [MenuItem("Dialoguer/Open Graph")]
        public static void CreateGraphViewWindow()
        {
            DialoguerGraph window = GetWindow<DialoguerGraph>();
            window.titleContent = new GUIContent("Dialoguer Graph");
        }

        private void ConstructGraphView()
        {
            _graphView = new DialoguerGraphView(this)
            {
                name = "Dialoguer Graph"
            };
            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView);
        }

        private void GenerateToolbar()
        {
            Toolbar toolbar = new Toolbar();
            
            TextField fileNameTF = new TextField("File Name:");
            fileNameTF.SetValueWithoutNotify(_fileName);
            fileNameTF.MarkDirtyRepaint();
            fileNameTF.RegisterValueChangedCallback(e => _fileName = e.newValue);
            toolbar.Add(fileNameTF);
            
            toolbar.Add(new Button(() => RequestDataOperation(true)) {text = "Save"});
            toolbar.Add(new Button(() => RequestDataOperation(false)) {text = "Load"});
            rootVisualElement.Add(toolbar);
        }

        private void RequestDataOperation(bool save)
        {
            if (!string.IsNullOrEmpty(_fileName))
            {
                GraphSaveUtility saveUtility = GraphSaveUtility.GetInstance(_graphView);
                if (save) saveUtility.SaveGraph(_fileName);
                else saveUtility.LoadDialogue(_fileName);
            }
            else
            {
                EditorUtility.DisplayDialog("Invalid File Name", "Please enter a valid filename", "OK");
            }
        }

        private void GenerateMinimap()
        {
            MiniMap miniMap = new MiniMap { anchored = true };
            Vector2 coords = _graphView.contentViewContainer.WorldToLocal(new Vector2(this.maxSize.x - 10, 30));
            miniMap.SetPosition(new Rect(coords.x, coords.y, 200, 140));
            _graphView.Add(miniMap);
        }

        private void GenerateBlackboard()
        {
            Blackboard blackboard = new Blackboard(_graphView);
            blackboard.Add(new BlackboardSection {title = "Exposed Variables"});
            blackboard.addItemRequested = _blackboard =>
            {
                _graphView.AddPropertyToBlackboard(ExposedProperty.CreateInstance(), false);
            };
            blackboard.editTextRequested = (_blackboard, element, newValue) =>
            {
                var oldPropertyName = ((BlackboardField) element).text;
                if (_graphView.ExposedProperties.Any(x => x.PropertyName == newValue))
                {
                    EditorUtility.DisplayDialog("Error", 
                        "This property name already exists, please chose another one.", "OK");
                    return;
                }

                var targetIndex = _graphView.ExposedProperties.FindIndex(x => x.PropertyName == oldPropertyName);
                _graphView.ExposedProperties[targetIndex].PropertyName = newValue;
                ((BlackboardField) element).text = newValue;
            };
            blackboard.SetPosition(new Rect(10,30,200,300));
            _graphView.Add(blackboard);
            _graphView.Blackboard = blackboard;
        }

        private void OnEnable()
        {
            ConstructGraphView();
            GenerateToolbar();
            GenerateMinimap();
            GenerateBlackboard();
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(_graphView);
        }
    }
}
