using System;
using System.Collections.Generic;
using System.Linq;
using GreenGremlins.Dialoguer.Editor.Editor.Dialoguer.Nodes;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GreenGremlins.Dialoguer.Editor
{
    public class DialoguerGraphView : GraphView
    {
        public readonly Vector2 DefaultNodeSize = new Vector2(200, 150);
        public readonly Vector2 DefaultCommentBlockSize = new Vector2(300, 200);

        public DialoguerNode RootNode;
        public List<ExposedProperty> ExposedProperties { get; private set; } = new List<ExposedProperty>();
        public Blackboard Blackboard = new Blackboard();
        private NodeSearchWnd _nodeSearchWnd;

        public DialoguerGraphView(DialoguerGraph graph)
        {
            styleSheets.Add(Resources.Load<StyleSheet>("DialoguerGraph"));
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new FreehandSelector());

            GridBackground grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();
            
            AddElement(GetRootNodeInstance());

            AddSearchWindow(graph);
        }

        private void AddSearchWindow(DialoguerGraph graph)
        {
            _nodeSearchWnd = ScriptableObject.CreateInstance<NodeSearchWnd>();
            _nodeSearchWnd.Configure(graph, this);
            nodeCreationRequest = ctx => SearchWindow.Open(new SearchWindowContext(ctx.screenMousePosition), _nodeSearchWnd);
        }

        private void RemovePort(Node node, Port socket)
        {
            IEnumerable<Edge> targets = edges.ToList().Where(x => x.output.portName == socket.portName && x.output.node == socket.node);
            if (targets.Any())
            {
                Edge edge = targets.First();
                edge.input.Disconnect(edge);
                RemoveElement(targets.First());
            }
            
            node.outputContainer.Remove(socket);
            node.RefreshPorts();
            node.RefreshExpandedState();
        }

        private Port GetPortInstance(DialoguerNode node, Direction direction,
            Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, direction, capacity, typeof(float));
        }

        private DialoguerNode GetRootNodeInstance()
        {
            DialoguerNode nodeCache = new DialoguerNode()
            {
                title = "START",
                GUID = Guid.NewGuid().ToString(),
                DialogueKey = "ENTRYPOINT",
                EntryPoint = true
            };

            Port generatedPort = GetPortInstance(nodeCache, Direction.Output);
            generatedPort.portName = "Next";
            nodeCache.outputContainer.Add(generatedPort);

            nodeCache.capabilities &= ~Capabilities.Movable;
            nodeCache.capabilities &= ~Capabilities.Deletable;
            
            nodeCache.RefreshExpandedState();
            nodeCache.RefreshPorts();
            nodeCache.SetPosition(new Rect(100, 200, 100, 150));
            return nodeCache;
        }

        public void ClearExpAndBlackboard()
        {
            ExposedProperties.Clear();
            Blackboard.Clear();
        }

        public Group CreateCommentBlock(Rect rect, CommentDataBlock data = null)
        {
            if (data == null) data = new CommentDataBlock();

            Group grp = new Group
            {
                autoUpdateGeometry = true,
                title = data.Title
            };
            
            AddElement(grp);
            grp.SetPosition(rect);
            return grp;
        }

        public void AddPropertyToBlackboard(ExposedProperty prop, bool loadMode = false)
        {
            string localPropName = prop.PropertyName;
            string localPropValue = prop.PropertyValue;
            if (!loadMode)
            {
                while (ExposedProperties.Any(x => x.PropertyName == localPropName))
                    localPropName = $"{localPropName}(1)";
            }
            
            ExposedProperty item = ExposedProperty.CreateInstance();
            item.PropertyName = localPropName;
            item.PropertyValue = localPropValue;
            ExposedProperties.Add(item);

            VisualElement cont = new VisualElement();
            BlackboardField bb = new BlackboardField
            {
                text = localPropName,
                typeText = "string"
            };
            cont.Add(bb);

            TextField propValField = new TextField("Value:")
            {
                value = localPropValue
            };
            propValField.RegisterValueChangedCallback(e =>
            {
                int idx = ExposedProperties.FindIndex(x => x.PropertyName == item.PropertyName);
                ExposedProperties[idx].PropertyValue = e.newValue;
            });

            BlackboardRow row = new BlackboardRow(bb, propValField);
            cont.Add(row);
            Blackboard.Add(cont);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            var startPortView = startPort;

            ports.ForEach((port) =>
            {
                var portView = port;
                if (startPortView != portView && startPortView.node != portView.node)
                    compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }

        public void CreateNewDialogueNode(string nodeName, Vector2 position)
        {
            AddElement(CreateNode(nodeName, position));
        }

        public DialoguerNode CreateNode(string nodeName, Vector2 position)
        {
            var tempDialogueNode = new DialoguerNode
            {
                title = nodeName,
                DialogueKey = nodeName,
                GUID = Guid.NewGuid().ToString()
            };
            tempDialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
            var inputPort = GetPortInstance(tempDialogueNode, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Input";
            tempDialogueNode.inputContainer.Add(inputPort);
            tempDialogueNode.RefreshExpandedState();
            tempDialogueNode.RefreshPorts();
            tempDialogueNode.SetPosition(new Rect(position, DefaultNodeSize));

            var textField = new TextField("");
            textField.RegisterValueChangedCallback(evt =>
            {
                tempDialogueNode.DialogueKey = evt.newValue;
                tempDialogueNode.title = evt.newValue;
            });
            textField.SetValueWithoutNotify(tempDialogueNode.title);
            tempDialogueNode.mainContainer.Add(textField);

            var button = new Button(() => { AddChoicePort(tempDialogueNode); })
            {
                text = "Add Choice"
            };
            tempDialogueNode.titleButtonContainer.Add(button);
            return tempDialogueNode;
        }


        public void AddChoicePort(DialoguerNode nodeCache, string overriddenPortName = "")
        {
            var generatedPort = GetPortInstance(nodeCache, Direction.Output);
            var portLabel = generatedPort.contentContainer.Q<Label>("type");
            generatedPort.contentContainer.Remove(portLabel);

            var outputPortCount = nodeCache.outputContainer.Query("connector").ToList().Count();
            var outputPortName = string.IsNullOrEmpty(overriddenPortName)
                ? $"Option {outputPortCount + 1}"
                : overriddenPortName;


            var textField = new TextField()
            {
                name = string.Empty,
                value = outputPortName
            };
            textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
            generatedPort.contentContainer.Add(new Label("  "));
            generatedPort.contentContainer.Add(textField);
            var deleteButton = new Button(() => RemovePort(nodeCache, generatedPort))
            {
                text = "X"
            };
            generatedPort.contentContainer.Add(deleteButton);
            generatedPort.portName = outputPortName;
            nodeCache.outputContainer.Add(generatedPort);
            nodeCache.RefreshPorts();
            nodeCache.RefreshExpandedState();
        }

    }
}