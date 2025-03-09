using System;
using System.Collections.Generic;
using System.IO;
using FMOD;
using GreenGremlins.Dialoguer;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace DefaultNamespace
{
    public struct DialogueData : IEquatable<DialogueData>
    {
        public bool start;
        public bool end;
        
        public string dialogue;
        public List<string> options;
        public Emotion.EmotionType emotion;

        public bool Equals(DialogueData other)
        {
            return start == other.start && end == other.end && dialogue == other.dialogue && Equals(options, other.options) && emotion == other.emotion;
        }

        public override bool Equals(object obj)
        {
            return obj is DialogueData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(start, end, dialogue, options, (int)emotion);
        }
    }
    
    public class DialoguerParser
    {
        private DialoguerAsset dialogue;
        private PersonData person;
        private JObject dialogues;
        private int curNode = 0;

        public DialoguerParser(DialoguerAsset asset, PersonData data)
        {
            dialogue = asset;
            person = data;
            LoadDialogueFile();
        }

        private void LoadDialogueFile()
        {
            try
            {
                dialogues = JObject.Parse(File.ReadAllText(Application.streamingAssetsPath + "/dialogues.json"));
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private void OptionPressed()
        {
            
        }
        
        public void Start()
        {
            UIController.Controller.Dialogue.SetName(person.Name);
            TraverseTo(dialogue.GetStartNode().NodeGUID);
        }

        private string GetDialogue(string key)
        {
            return dialogues[key] == null ? "Not Found" : dialogues[key].ToObject<string>();
        }
        
        private void TraverseTo(string id)
        {
            DialoguerNodeData node = dialogue.Nodes.Find(x => x.NodeGUID == id);
            switch (node)
            {
                case DialoguerStartNode n:
                {
                    NodeLink link = dialogue.Links.Find(x => x.output == n.NodeGUID); 
                    TraverseTo(link.input);
                    break;
                }
                case DialoguerActionNode n:
                {
                    UIController.Controller.Dialogue.SetImage(person.GetEmotion(n.Emotion).sprite);
                    UIController.Controller.Dialogue.SetDialogue(GetDialogue(n.DialogueKey));
                    UIController.Controller.Dialogue.ClearOptions();
                    for (int i = 0; i < n.DialogueOptions.Length; i++)
                    {
                        DialogueOption opt = UIController.Controller.Dialogue.CreateOption(GetDialogue(n.DialogueOptions[i]));
                        NodeLink link = dialogue.Links.Find(x => x.outId == i && x.output == n.NodeGUID); 
                        opt.SetClick(() => TraverseTo(link.input));
                    }
                    break;
                }
                case DialoguerFunctionNode n:
                {
                    n.OnDialogueEnd?.Invoke();
                    break;
                }
                case DialoguerEndNode n:
                {
                    break;
                }
            }
        }
        
        public DialogueData ParseNode()
        {
            DialoguerNodeData node = dialogue.GetNode(curNode);
            if (node is DialoguerStartNode)
            {
                curNode++;
                DialogueData data = new DialogueData();
                data.start = true;
                return data;
            }
            else if (node is DialoguerActionNode)
            {
                DialoguerActionNode n = node as DialoguerActionNode;
                DialogueData data = new DialogueData();
                data.dialogue = dialogues[n.DialogueKey] == null ? "NOT FOUND" : dialogues[n.DialogueKey].ToObject<string>();
                data.emotion = n.Emotion;
                data.options = new List<string>();
                foreach (string opt in n.DialogueOptions)
                {
                    data.options.Add(dialogues[opt] == null ? "NOT FOUND" : dialogues[opt].ToObject<string>());
                }
                return data;
            }
            else
            {
                DialogueData data = new DialogueData();
                data.end = true;
                return data;
            }
        }
    }
}