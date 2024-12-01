using System;
using System.Collections.Generic;
using UnityEngine;

namespace GreenGremlins.Dialoguer
{
    [Serializable]
    public class DialoguerContainer : ScriptableObject
    {
        public List<NodeLinkData> Links = new List<NodeLinkData>();
        public List<DialoguerNodeData> Nodes = new List<DialoguerNodeData>();
        public List<ExposedProperty> ExposedProperties = new List<ExposedProperty>();
        public List<CommentDataBlock> Comments = new List<CommentDataBlock>();
    }
}