using System.Collections;
using System.Collections.Generic;
using GreenGremlins.Dialoguer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{

    [SerializeField] private TMP_Text personName;
    [SerializeField] private TMP_Text dialogue;
    [SerializeField] private Image personImage;
    [SerializeField] private Transform dialogueOptionsContainer;
    [SerializeField] private DialogueOption dialoguePrefab;
    private List<DialogueOption> _options = new List<DialogueOption>();

    public void SetImage(Sprite sprite)
    {
        if (personImage.sprite == sprite) return;
        personImage.sprite = sprite;
    }
    
    public void SetName(string name)
    {
        if (personName.text == name) return;
        personName.text = name;
    }
    
    public void SetOptions(List<string> options)
    {
        _options.ForEach(Destroy);
        _options.Clear();
        foreach (string option in options)
        {
            DialogueOption dialOpt = Instantiate(dialoguePrefab, dialogueOptionsContainer);
            dialOpt.SetText(option);
            _options.Add(dialOpt);
        }
    }

    public DialogueOption CreateOption(string text)
    {
        DialogueOption opt = Instantiate(dialoguePrefab, dialogueOptionsContainer);
        opt.SetText(text);
        _options.Add(opt);
        return opt;
    }
    
    public void ClearOptions()
    {
        _options.ForEach(opt => Destroy(opt.gameObject));
        _options.Clear();
    }

    public void SetDialogue(string text)
    {
        if (dialogue.text == text) return;
        dialogue.text = text;
    }
}
