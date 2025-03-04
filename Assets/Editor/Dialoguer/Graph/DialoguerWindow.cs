using System;
using GreenGremlins.Dialoguer;
using GreenGremlins.Dialoguer.Editor;
using GreenGremlins.Dialoguer.Editor.Nodes;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

public class DialoguerWindow : EditorWindow
{
    public static readonly string STYLESHEET_PATH = "Assets/Editor/Dialoguer/Resources/DialoguerWindow";

    public static StyleSheet GetStyleSheet()
    {
        return AssetDatabase.LoadAssetAtPath<StyleSheet>(STYLESHEET_PATH+".uss");
    }
    
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    private static void OpenWindow(DialoguerAsset dialogue)
    {
        DialoguerWindow wnd = GetWindow<DialoguerWindow>();
        wnd.titleContent = new GUIContent($"DialoguerWindow - {dialogue.name}");
        wnd.SetDialogue(dialogue);
    }

    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceId, int line)
    {
        if (Selection.activeObject is not DialoguerAsset) return false;
        DialoguerAsset dialogue = Selection.activeObject as DialoguerAsset;
        OpenWindow(dialogue);
        return true;
    }

    public void SetDialogue(DialoguerAsset dialogue)
    {
        if (dialogue) view.PopulateView(dialogue);
        InspectorView.currentDialogue = dialogue;
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        
        // Add stylesheet
        StyleSheet styleSheet = GetStyleSheet();
        root.styleSheets.Add(styleSheet);
        
        // Instantiate UXML
        VisualTreeAsset tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(STYLESHEET_PATH+".uxml");
        tree.CloneTree(root);

        view = root.Q<DialoguerView>();
        view.focusable = true;
        view.Focus();
        
        inspectorView = root.Q<InspectorView>();

        view.OnNodeSelected = OnNodeSelectionChange;
    }

    private void OnNodeSelectionChange(DialoguerNode node)
    {
        inspectorView.UpdateSelection(node);
    }

    private DialoguerView view;
    private InspectorView inspectorView;
}
