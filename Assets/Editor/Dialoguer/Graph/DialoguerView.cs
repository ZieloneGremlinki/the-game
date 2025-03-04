using System;
using System.Collections.Generic;
using System.Linq;
using GreenGremlins.Dialoguer.Editor.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GreenGremlins.Dialoguer.Editor
{
    public class DialoguerView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<DialoguerView, UxmlTraits>
        {
        }

        public Action<DialoguerNode> OnNodeSelected;

        public DialoguerView()
        {
            Insert(0, new GridBackground());
            
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            
            StyleSheet styleSheet = DialoguerWindow.GetStyleSheet();
            styleSheets.Add(styleSheet);

            Undo.undoRedoPerformed += OnUndoRedo;
        }

        private void OnUndoRedo()
        {
            PopulateView(dialogue);
            AssetDatabase.SaveAssets();
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
                graphViewChange.elementsToRemove.ForEach(e =>
                {
                    DialoguerNode node = e as DialoguerNode;
                    if (node != null)
                        dialogue.DeleteNode(node.data);
                    
                    Edge edge = e as Edge;
                    if (edge != null)
                    {
                        DialoguerNode parent = edge.output.node as DialoguerNode;
                        DialoguerNode child = edge.input.node as DialoguerNode;
                        dialogue.RemoveChild(parent.data, child.data);
                    }
                });
            }

            if (graphViewChange.edgesToCreate != null)
            {
                graphViewChange.edgesToCreate.ForEach(e =>
                {
                    DialoguerNode parent = e.output.node as DialoguerNode;
                    DialoguerNode child = e.input.node as DialoguerNode;
                    
                    dialogue.AddChild(parent.data, GetId(e), child.data);
                });
            }

            return graphViewChange;
        }

        private int GetId(Edge e)
        {
            try
            {
                return int.Parse(e.output.portName);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        private DialoguerNode FindNode(DialoguerNodeData node)
        {
            return GetNodeByGuid(node.NodeGUID) as DialoguerNode;
        }
        
        internal void PopulateView(DialoguerAsset dialogue)
        {
            this.dialogue = dialogue;

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;
            
            dialogue.Nodes.ForEach(CreateViewNode);
            
            dialogue.Nodes.ForEach(n =>
            {
                List<DialoguerNodeData> children = n.children;
                children.ForEach(c =>
                {
                    DialoguerNode parent = FindNode(n);
                    DialoguerNode child = FindNode(c);

                    foreach (NodeLink link in dialogue.Links)
                    {
                        if (link.output == parent.NodeGUID && link.input == child.NodeGUID)
                        {
                            Edge edge = parent.outputs[link.outId].ConnectTo(child.input);
                            AddElement(edge);
                        }
                    }
                });
            });
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(endPort =>
                endPort.direction != startPort.direction &&
                endPort.node != startPort.node).ToList();
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom<DialoguerNodeData>();
            foreach (Type type in types)
            {
                if (type == typeof(DialoguerStartNode)) continue;
                evt.menu.AppendAction($"{type.Name}", (a) => CreateNode(type, viewTransform.matrix.inverse.MultiplyPoint(evt.localMousePosition)));
            }
        }

        private void CreateNode(System.Type type, Vector2 pos)
        {
            DialoguerNodeData node = dialogue.CreateNode(type);
            node.Position = pos;
            Debug.Log($"Position: {pos}");
            
            CreateViewNode(node);
        }

        private void CreateViewNode(DialoguerNodeData node)
        {
            DialoguerNode graphNode = new DialoguerNode(node);
            graphNode.OnNodeSelected = OnNodeSelected;
            AddElement(graphNode);
        }

        private DialoguerAsset dialogue;
    }
}