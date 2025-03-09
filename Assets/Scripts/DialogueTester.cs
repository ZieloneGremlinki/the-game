using System;
using DefaultNamespace;
using GreenGremlins.Dialoguer;
using UnityEngine;

[DisallowMultipleComponent]
public class DialogueTester : MonoBehaviour
{
    [SerializeField] private PersonData person;
    [SerializeField] private DialoguerAsset dialogue;

    private DialoguerParser parser;

    private void Awake()
    {
        parser = new DialoguerParser(dialogue, person);
    }

    private void Start()
    {
        parser.Start();
    }

    private void Update()
    {
    }
}