using BardicBytes.BardicFramework;
using BardicBytes.BardicFramework.EventVars;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsDebug : MonoBehaviour
{
    private struct MessageData
    {
        public string message;
        public float time;
        public int frame;
    }


    [SerializeField]
    private StringEventVar messageEvent = default;
    [SerializeField]
    private TMPro.TextMeshProUGUI textOutput = default;

    [SerializeField]
    private Queue<MessageData> messageQueue;

    private void Awake()
    {
        messageQueue = new Queue<MessageData>();
        textOutput.text = "";
    }

    private void OnEnable()
    {
        messageEvent.AddListener(HandleMessage);
    }

    private void OnDisable()
    {
        messageEvent.RemoveListener(HandleMessage);
    }

    private void HandleMessage(string newMessage)
    {
        messageQueue.Enqueue(new MessageData() { message = newMessage, time = Time.time, frame = Time.frameCount });
    }

    private void Update()
    {
        if(messageQueue.Count > 0)
        {
            ShowNext();
        }
    }

    public void ShowNext()
    {
        var d = messageQueue.Dequeue();
        textOutput.text += string.Format("\nt{1}, f{2}, m: {0}",d.message,d.time,d.frame);
    }
}
