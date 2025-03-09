using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GreenGremlins.Dialoguer.Editor.Nodes
{
    public class NodeUpdateData
    {
    }
    
    public class DialoguerNode : Node
    {
        public Action<DialoguerNode> OnNodeSelected;
        public DialoguerNodeData data;

        private List<Port> _outputs;

        public Port input;
        public List<Port> outputs
        {
            get
            {
                if (_outputs == null) _outputs = new List<Port>();
                return _outputs;
            }
        }

        public string NodeGUID
        {
            get
            {
                return data.NodeGUID;
            }
        }
        
        public DialoguerNode(DialoguerNodeData data)
        {
            this.data = data;
            title = "Dialoguer Node";
            
            viewDataKey = data.NodeGUID;

            switch (data)
            {
                case DialoguerStartNode n:
                {
                    title = "Entry Point";
                    capabilities &= ~Capabilities.Deletable;
                    capabilities &= ~Capabilities.Movable;
                    capabilities &= ~Capabilities.Selectable;
                    CreateStartOut();
                    break;
                }
                case DialoguerEndNode n:
                {
                    title = "End Point";
                    CreateEndIn();
                    break;
                }
                case DialoguerFunctionNode n:
                {
                    title = "Function Node";
                    CreateInput();
                    break;
                }
                case DialoguerActionNode n:
                {
                    CreateInput();
                    NodeUpdated();
                    break;
                }
            }

            style.left = data.Position.x;
            style.top = data.Position.y;

            data.OnNodeUpdated = NodeUpdated;
        }

        private void NodeUpdated()
        {
            foreach (Port output in outputs)
            {
                DialoguerAsset dialogue = InspectorView.currentDialogue;
                if (dialogue != null)
                {
                    dialogue.Links.Where(link =>
                        link.output == NodeGUID &&
                        link.input == output.connections.ToList()[0].input.node.viewDataKey).ToList().ForEach(link => dialogue.Links.Remove(link));
                }
                
                output.DisconnectAll();
                outputContainer.Remove(output);
            }
            outputs.Clear();
            
            DialoguerActionNode n = data as DialoguerActionNode;

            if (n && n.DialogueOptions != null)
            {
                for (int i = 0; i < n.DialogueOptions.Length; i++)
                {
                    Port output = AddOutput();
                }
            }
        }

        private Port AddOutput()
        {
            Port output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            if (output != null)
            {
                output.portName = outputs.Count.ToString();
                outputContainer.Add(output);
            }
            outputs.Add(output);
            return output;
        }
        
        private void CreateInput()
        {
            input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            if (input != null)
            {
                input.portName = "IN";
                inputContainer.Add(input);
            }
        }

        private void CreateStartOut()
        {
            Port output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            if (output != null)
            {
                output.portName = "START";
                outputContainer.Add(output);
            }
            outputs.Add(output);
        }
        
        private void CreateEndIn()
        {
            input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            if (input != null)
            {
                input.portName = "END";
                inputContainer.Add(input);
            }
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Undo.RecordObject(data, "Dialoguer (Set Position)");
            data.Position.x = newPos.xMin;
            data.Position.y = newPos.yMin;
            EditorUtility.SetDirty(data);
        }

        public override void OnSelected()
        {
            base.OnSelected();
            if (OnNodeSelected != null) OnNodeSelected.Invoke(this);
        }
    }
}