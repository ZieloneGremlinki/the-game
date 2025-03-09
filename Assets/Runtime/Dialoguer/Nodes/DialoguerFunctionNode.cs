using UnityEngine.Events;

namespace GreenGremlins.Dialoguer
{
    public class DialoguerFunctionNode : DialoguerNodeData
    {
        public UnityEvent OnDialogueEnd;

        public static string FUNCTION_NAME = nameof(OnDialogueEnd);
    }
}