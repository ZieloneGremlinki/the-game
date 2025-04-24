using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum NodeType
{
    StartNode,
    EncounterNode,
    EndNode
}

[DisallowMultipleComponent]
public class MapNode : MonoBehaviour
{
    private NodeType _nodeType;
    private Button _button;

    public NodeType NodeType
    {
        get => _nodeType;
        set
        {
            if (value == NodeType.StartNode) Interactable = false;
            _nodeType = value;
        }

    }

    public bool Interactable
    {
        get => _button.interactable;
        set => _button.interactable = value;
    }

    public void AddEvent(UnityAction ev)
    {
        _button.onClick.AddListener(ev);
    }
    
    private void Awake()
    {
        _button = GetComponent<Button>();
    }
}