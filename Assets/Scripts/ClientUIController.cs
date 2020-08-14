using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ClientUIController : MonoBehaviour
{
    [SerializeField] private InputField messageText;
    [SerializeField] private Dropdown ChatType;
    [SerializeField] private Transform ChatContent;
    [SerializeField] private Button send;

    [SerializeField] private GameObject messagePrefab;

    [SerializeField] private Client client;

    private int MAX_VISIBLE_CHAT = 30;
    private int currentCount = 0;

    private void Start()
    {
        client.SendLoginMessage("admin", "exhibition", "pavilion1");
        send.onClick.AddListener((() => AddMessage(null)));
    }

    public void AddMessage(Message message)
    {
        if (message == null)
        {
            message = new Message("fdsa", "admin", false, "gfdsa");
        }
        InstantinateMessage(message);
        currentCount += 1;
    }

    public void RemoveMessage(Message message)
    {
        for (int i = 0; i < ChatContent.childCount; i++)
        {
            if (ChatContent.GetChild(i).GetComponent<MessageUI>().Id == message.Id)
            {
                Destroy(ChatContent.GetChild(i).gameObject);
                currentCount -= 1;
                return;
            }
        }
    }

    private void InstantinateMessage(Message message)
    {
        GameObject mess = Instantiate(messagePrefab, ChatContent);
        mess.GetComponent<MessageUI>().SetMessage(message);
        RectTransform targetTransform = mess.GetComponent<RectTransform>();
        targetTransform.anchoredPosition = new Vector2(80, -50*currentCount);
    }

    public void SendMessage()
    {
        // client.SendLoginMessage("admin", "exhibition", "pavilion1");
        client.SendMessage(null);
    }
    
    
    
}
