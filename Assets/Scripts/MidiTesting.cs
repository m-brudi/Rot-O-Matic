using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class MidiTesting : MonoBehaviour
{
    [SerializeField] TMP_InputField channelInput;
    [SerializeField] TMP_InputField statusInput;
    [SerializeField] TMP_InputField data1Input;
    [SerializeField] TMP_InputField data2Input;
    [SerializeField] Button sendBtn;
    [SerializeField] TMP_Text debugTxt; 

    private void Start() {
        sendBtn.onClick.AddListener(() => SendMidi());
    }

    void SendMidi() {
        if (channelInput.text.Length == 0)
            channelInput.text = "0";
        if (statusInput.text.Length == 0)
            statusInput.text = "0";
        if (data1Input.text.Length == 0)
            data1Input.text = "0";
        if (data2Input.text.Length == 0)
            data2Input.text = "0";

        var channel = (byte)Mathf.Clamp(Convert.ToInt32(channelInput.text), 0, 255);
        var status = (byte)Mathf.Clamp(Convert.ToInt32(statusInput.text), 0, 255);
        var data1 = (byte)Mathf.Clamp(Convert.ToInt32(data1Input.text), 0, 255);
        var data2 = (byte)Mathf.Clamp(Convert.ToInt32(data2Input.text), 0, 255);


        debugTxt.text = channel + " " + status + " " + data1 + " " + data2 + " SEND!";
    }

    void Update()
    {
        
    }
}
