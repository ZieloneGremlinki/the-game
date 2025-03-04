using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace GreenGremlins.Dialoguer
{
    [Serializable]
    public class NodeLink
    {
        public int outId;
        public string output;
        public string input;

        public NodeLink(int outId, string input, string output)
        {
            this.outId = outId;
            this.input = input;
            this.output = output;
        }
    }
    
    [CreateAssetMenu(menuName = "Dialoguer/New Dialogue Map", fileName = "New Dialogue Map"), Serializable]
    public class DialoguerAsset : ScriptableObject
    {
        public static event UnityAction<DialoguerAsset> OnOpenDialoguerAsset;
        
        public List<DialoguerNodeData> Nodes = new List<DialoguerNodeData>();
        public List<NodeLink> Links = new List<NodeLink>();
        public List<CommentDataBlock> Comments = new List<CommentDataBlock>();

        private DialoguerNodeData root;

        public DialoguerNodeData GetStartNode()
        {
            return Nodes.Where(n => n.IsStartNode).ToList()[0];
        }
        
        public List<DialoguerNodeData> GetEndNodes()
        {
            return Nodes.Where(n => n.IsEndNode).ToList();
        }
        
        #if UNITY_EDITOR
        private void Awake()
        {
            if (root == null) 
            {
                root = CreateNode(typeof(DialoguerStartNode));
            }
        }

        public DialoguerNodeData CreateNode(Type type)
        {
            try
            {
                DialoguerNodeData node = CreateInstance(type) as DialoguerNodeData;
                node.NodeGUID = GUID.Generate().ToString();
                node.name = node.NodeGUID;
                Nodes.Add(node);

                AssetDatabase.AddObjectToAsset(node, this);
                Undo.RegisterCreatedObjectUndo(node, "Dialoguer (Node Created)");
                AssetDatabase.SaveAssets();
                return node;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return null;
            }
        }

        public void DeleteNode(DialoguerNodeData node)
        {
            Nodes.Remove(node);
            Undo.DestroyObjectImmediate(node);
            AssetDatabase.SaveAssets();
        }

        public void AddChild(DialoguerNodeData parent, int parentId, DialoguerNodeData child)
        {
            Undo.RecordObject(parent, "Dialoguer (Add Link)");
            parent.children.Add(child);
            Links.Add(new NodeLink(parentId, child.NodeGUID, parent.NodeGUID));
            EditorUtility.SetDirty(parent);
        }

        public void RemoveChild(DialoguerNodeData parent, DialoguerNodeData child)
        {
            Undo.RecordObject(parent, "Dialoguer (Remove Link)");
            parent.children.Remove(child);
            foreach (NodeLink link in Links)
            {
                if (link.output == parent.NodeGUID && link.input == child.NodeGUID)
                {
                    Links.Remove(link);
                    break;
                }
            }
            EditorUtility.SetDirty(parent);
        }
        #endif
    }
}