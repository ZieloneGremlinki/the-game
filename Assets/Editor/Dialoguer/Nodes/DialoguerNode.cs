using UnityEditor.Experimental.GraphView;

namespace GreenGremlins.Dialoguer.Editor.Editor.Dialoguer.Nodes
{
    public class DialoguerNode : Node
    {
        public string DialogueKey;
        public string GUID;
        public bool EntryPoint = false;
    }
}