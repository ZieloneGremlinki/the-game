using System;
using System.Collections.Generic;
using Codice.CM.Common.Serialization;
using UnityEngine;

namespace GreenGremlins.Dialoguer
{
    [Serializable]
    public class DialoguerNodeData : ScriptableObject
    {
        public Action OnNodeUpdated;
        
        public string NodeGUID;
        public Vector2 Position;

        public List<DialoguerNodeData> parents = new List<DialoguerNodeData>();
        public List<DialoguerNodeData> children = new List<DialoguerNodeData>();
        
        public bool IsStartNode
        {
            get
            {
                return GetType() == typeof(DialoguerStartNode);
            }
        }
        
        public bool IsEndNode
        {
            get
            {
                return GetType() == typeof(DialoguerEndNode);
            }
        }
    }
}