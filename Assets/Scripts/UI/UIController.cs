using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private DialogueController dialogues;

    public DialogueController Dialogue
    {
        get => dialogues;
    }

    private void Awake()
    {
    }

    
    private static UIController _controller;
    public static UIController Controller
    {
        get
        {
            if (_controller == null) _controller = GameObject.FindWithTag("UI").GetComponent<UIController>();
            return _controller;
        }
    }
}
