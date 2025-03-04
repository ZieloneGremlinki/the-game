using System;
using System.Collections.Generic;
using UnityEngine;

namespace GreenGremlins.Dialoguer
{
    [Serializable]
    public class Emotion : ScriptableObject
    {
        public enum EmotionType
        {
            Neutral,
            Surprised,
            Happy,
            Angry,
            Sad,
            Embarrassed,
            Disgusted,
        }

        public EmotionType emotion;
        public Sprite sprite;
    }
    
    [CreateAssetMenu(menuName = "Dialoguer/New Person Data", fileName = "New Person Data"), Serializable]
    public class PersonData : ScriptableObject
    {
        public string Name;
        public Emotion[] Emotions;
    }
}