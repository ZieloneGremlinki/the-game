using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueOption : MonoBehaviour
{
    [SerializeField] private TMP_Text optionText;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void SetClick(UnityAction e)
    {
        button.onClick.AddListener(e);
    }

    public void SetText(string option)
    {
        optionText.text = option;
    }
}
