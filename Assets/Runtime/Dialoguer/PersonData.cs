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

        public static Emotion Create()
        {
            return (Emotion)CreateInstance(typeof(Emotion));
        }

        public static string EMOTION_NAME => nameof(emotion);
        public static string EMOTION_SPRITE => nameof(sprite);
    }
    
    [CreateAssetMenu(menuName = "Dialoguer/New Person Data", fileName = "New Person Data"), Serializable]
    public class PersonData : ScriptableObject
    {
        public string Name;
        public Emotion[] Emotions;

        public Emotion GetEmotion(Emotion.EmotionType emotion)
        {
            for (int i = 0; i < Emotions.Length; i++)
            {
                if (Emotions[i].emotion == emotion) return Emotions[i];
            }
            return Emotions[0];
        }
    }
}