using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Client : MonoBehaviour
{
    [SerializeField] private ClientUIController uiController;
    
    public Text mylog;
    public InputField field;

    private const int MAX_CONNECTIONS = 10000;
    private const string SERVER_IP = "127.0.0.1";
    private const int SERVER_PORT = 2019;
    private const int SERVER_WEB_PORT = 2222;
    private const int BUFFER_SIZE = 4096;

    private int reliableChannelId;

    private int hostId;
    private int connectionId;

    private byte error;
    private byte[] buffer = new byte[BUFFER_SIZE];
    private bool isConnected = false;
    private bool isLoggedIn = false;
    private List<Message> messages = new List<Message>();

    private int i = 0;
    
    public void ReconnectToServer()
    {
        isConnected = false;
        isLoggedIn = false;
#if UNITY_WEBGL

        connectionId = NetworkTransport.Connect(hostId, SERVER_IP, SERVER_WEB_PORT, 0, out error);
        
#else

        connectionId = NetworkTransport.Connect(hostId, SERVER_IP, SERVER_PORT, 0, out error);
#endif
    }

    public bool IsConnected => isConnected;

    public bool IsLoggedIn => isLoggedIn;


    public void SendMessage(string text)
    {
        if(!isConnected || !isLoggedIn)
            return;
        Message obj = new Message(text, "admin",false, "all");
        IFormatter formatter = new BinaryFormatter();
        byte[] buffer;
        using (MemoryStream stream = new MemoryStream())
        {
            formatter.Serialize(stream, obj);
            buffer = stream.ToArray();
        }

        NetworkTransport.Send(hostId, connectionId, reliableChannelId, buffer, buffer.Length, out error);

    }

    public void SendLoginMessage(string login, string exhibitionName, string pavilionName)
    {
        if (!isConnected)
            return;
        LoginMessage obj = new LoginMessage(login, exhibitionName, pavilionName);
        IFormatter formatter = new BinaryFormatter();
        byte[] buffer;
        using (MemoryStream stream = new MemoryStream())
        {
            formatter.Serialize(stream, obj);
            buffer = stream.ToArray();
        }
        NetworkTransport.Send(hostId, connectionId, reliableChannelId, buffer, buffer.Length, out error);
        isLoggedIn = true;
    }

    public void SendLogoutMessage(string login, string exhibitionName, string pavilionName)
    {
        if(!isConnected || !isLoggedIn)
            return;
        LoginMessage obj = new LoginMessage(login, exhibitionName, pavilionName, true);
        IFormatter formatter = new BinaryFormatter();
        byte[] buffer;
        using (MemoryStream stream = new MemoryStream())
        {
            formatter.Serialize(stream, obj);
            buffer = stream.ToArray();
        }
        NetworkTransport.Send(hostId, connectionId, reliableChannelId, buffer, buffer.Length, out error);
    }
    
    private void Start()
    {
        GlobalConfig config = new GlobalConfig();
        NetworkTransport.Init(config);
        
        ConnectionConfig cc = new ConnectionConfig();
        reliableChannelId = cc.AddChannel(QosType.Reliable);
        HostTopology topo = new HostTopology(cc, MAX_CONNECTIONS);

        hostId = NetworkTransport.AddHost(topo,0);

#if UNITY_WEBGL

        connectionId = NetworkTransport.Connect(hostId, SERVER_IP, SERVER_WEB_PORT, 0, out error);
        
#else

        connectionId = NetworkTransport.Connect(hostId, SERVER_IP, SERVER_PORT, 0, out error);
#endif
        
    }
    
    private void Update()
    {
        
        int outHostId, outConnectionId, outChannelId;
        int recievedSize;
        
        buffer = new byte[BUFFER_SIZE];
        NetworkEventType e =  NetworkTransport.Receive(out outHostId, out outConnectionId, out outChannelId,buffer, buffer.Length, out recievedSize,
            out error);

        if (e == NetworkEventType.Nothing)
        {
            return;
        }
        
        switch (e)
        {
            case NetworkEventType.ConnectEvent:
                isConnected = true;
                break;
            case NetworkEventType.DataEvent:
                IFormatter formatter = new BinaryFormatter();
                Stream destream = new MemoryStream(buffer);
                object back = formatter.Deserialize(destream);
                if (back.GetType().Name == "Message")
                {
                    RecieveMessage(outHostId, outConnectionId, outChannelId, (Message)back);
                }

                if (back.GetType().Name == "AdminMessage")
                {
                    RecieveAdminMessage((AdminMessage)back);
                }

                break;
            case NetworkEventType.DisconnectEvent:
                Debug.Log("Disconnect from " + outConnectionId + " through the channel " + outChannelId);
                break;
            case NetworkEventType.BroadcastEvent:
                Debug.Log("Broadcast from " + outConnectionId + " through the channel " + outChannelId);
                break;
            default:
                Debug.Log("Unknown type of message");
                break;
        }
    }
    
    private void RecieveMessage(int outHostId, int outConnectionId, int outChannelId, Message back)
    {
        messages.Add(back);
        uiController.AddMessage(back);
        mylog.text += back.ToString() + "\n";
    }

    private Message GetMessageById(int id)
    {
        for (int i = 0; i < messages.Count; i++)
        {
            if (messages[i].Id == id)
            {
                return messages[i];
            }
        }

        return null;
    }
    
    private void RecieveAdminMessage( AdminMessage back)
    {
        Message messageToRemove = GetMessageById(back.IdMessage);
        messages.Remove(messageToRemove);
        uiController.RemoveMessage(messageToRemove);
        
    }
}
