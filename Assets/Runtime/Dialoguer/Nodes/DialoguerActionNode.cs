using System;

namespace GreenGremlins.Dialoguer
{
    [Serializable]
    public class DialoguerActionNode : DialoguerNodeData
    {
        public string PersonId;
        public string DialogueKey;
        public string[] DialogueOptions;
        public Emotion.EmotionType Emotion;
    }
}