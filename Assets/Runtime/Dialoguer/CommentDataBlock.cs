using System;
using System.Collections.Generic;
using UnityEngine;

namespace GreenGremlins.Dialoguer
{
    [Serializable]
    public class CommentDataBlock
    {
        public List<string> Children = new List<string>();
        public Vector2 Position;
        public string Title = "Comment Block";
    }
}