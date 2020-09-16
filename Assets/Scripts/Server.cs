using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Server : MonoBehaviour
{
    [SerializeField] private ServerUI uiController;
    
    private const int MAX_CONNECTIONS = 10000;
    private const string SERVER_IP = "127.0.0.1";
    private const int SERVER_PORT = 2019;
    private const int SERVER_WEB_PORT = 2222;
    private const int BUFFER_SIZE = 4096;

    private int reliableChannelId;

    private int hostId;
    private int webHostId;
    byte error;

    private byte[] buffer = new byte[BUFFER_SIZE];
    private bool isInit;
    
    private Dictionary<string, List<ChatUser>> users = new Dictionary<string, List<ChatUser>>();
    private List<ChatUser> unlogedUsers = new List<ChatUser>();
    private List<Message> messages = new List<Message>();
    private int messageID = 0;

    // Создание сервера и его включение
    private void Start()
    {
        GlobalConfig config = new GlobalConfig();
        NetworkTransport.Init(config);
        
        ConnectionConfig cc = new ConnectionConfig();
        reliableChannelId = cc.AddChannel(QosType.Reliable);
        HostTopology topo = new HostTopology(cc, MAX_CONNECTIONS);

        hostId = NetworkTransport.AddHost(topo, SERVER_PORT);
        webHostId = NetworkTransport.AddWebsocketHost(topo, SERVER_WEB_PORT);

        isInit = true;
    }

    // получение сообщений и их обработка
    private void Update()
    {
        if (!isInit) 
            return;

        int outHostId, outConnectionId, outChannelId;
        int recievedSize;
        
        buffer = new byte[BUFFER_SIZE];

        NetworkEventType e =  NetworkTransport.Receive(out outHostId, out outConnectionId, out outChannelId,buffer, buffer.Length, out recievedSize,
            out error);

        if (e == NetworkEventType.Nothing)
        {
            return;
        }
        
        Debug.Log("Message send error: " + (NetworkError)error);
        switch (e)
        {
            case NetworkEventType.ConnectEvent:
                Debug.Log("Connect from " + outConnectionId + " through the channel " + outChannelId);
                uiController.AddLog("Connect from " + outConnectionId + " through the channel " + outChannelId);
                unlogedUsers.Add(new ChatUser(outHostId, outConnectionId, outChannelId));
                break;
            case NetworkEventType.DataEvent:
                if (GetUnloggedUser(outHostId, outConnectionId, outChannelId) == null && GetUser(outHostId, outConnectionId, outChannelId) == null)
                    return;

                IFormatter formatter = new BinaryFormatter();
                Stream destream = new MemoryStream(buffer);
                object message = formatter.Deserialize(destream);
                if (message.GetType().Name.Equals("LoginMessage") && (GetUnloggedUser(outHostId, outConnectionId, outChannelId) != null || !GetUser(outHostId, outConnectionId, outChannelId).IsLoggedIn()))
                {
                    RecieveLogInMessage(outHostId, outConnectionId, outChannelId, (LoginMessage)message);
                }
                if (GetUser(outHostId, outConnectionId, outChannelId).IsLoggedIn())
                {
                    if (message.GetType().Name.Equals("Message"))
                    {
                        RecieveMessage(outHostId, outConnectionId, outChannelId, (Message) message);
                    }
                    if (message.GetType().Name.Equals("AdminMessage"))
                    {
                        RecieveAdminMessage(outHostId, outConnectionId, outChannelId, (AdminMessage) message);
                    }
                    if (message.GetType().Name.Equals("LoginMessage"))
                    {
                        RecieveLogInMessage(outHostId, outConnectionId, outChannelId, (LoginMessage) message);
                    }
                    
                }
                
                break;
            case NetworkEventType.DisconnectEvent:
                Debug.Log("Disconnect from " + outConnectionId + " through the channel " + outChannelId);
                uiController.AddLog("Disconnect from " + outConnectionId + " through the channel " + outChannelId);
                RemoveUser(outHostId, outConnectionId, outChannelId);
                break;
            case NetworkEventType.BroadcastEvent:
                Debug.Log("Broadcast from " + outConnectionId + " through the channel " + outChannelId);
                uiController.AddLog("Broadcast from " + outConnectionId + " through the channel " + outChannelId);
                break;
            default:
                Debug.Log("Unknown type of message");
                break;
        }
    }

    /// <summary>
    /// Удаление пользователя из списка подключений при его отключении
    /// </summary>
    /// <param name="outHostId"> идентификатор хоста</param>
    /// <param name="outConnectionId"> идентификатор соединения</param>
    /// <param name="outChannelId"> идентификатор канала</param>
    private void RemoveUser(int outHostId, int outConnectionId, int outChannelId)
    {
        ChatUser user = GetUser(outHostId, outConnectionId, outChannelId);
        if (user == null)
        {
            ChatUser unUser = GetUnloggedUser(outHostId, outConnectionId, outChannelId);
            if (unUser==null)
                return;
            unlogedUsers.Remove(unUser);
        } 
        users[user.Pavilion].Remove(user);
    }
   
    /// <summary>
    ///  Обработка сообщения LoginMessage с данными о входе\выходе пользователя в павильон
    /// </summary>
    /// <param name="outHostId"> идентификатор хоста</param>
    /// <param name="outConnectionId"> идентификатор соединения</param>
    /// <param name="outChannelId"> идентификатор канала</param>
    /// <param name="logMess"> сообщение с данными о входе</param>
    private void RecieveLogInMessage(int outHostId, int outConnectionId, int outChannelId, LoginMessage logMess)
    {
        if (GetUnloggedUser(outHostId, outConnectionId, outChannelId) != null ||
            !GetUser(outHostId, outConnectionId, outChannelId).IsLoggedIn())
        {
            ChatUser user = GetUnloggedUser(outHostId, outConnectionId, outChannelId);
            unlogedUsers.Remove(user);
            Debug.Log(logMess.ToString());
            uiController.AddLog(logMess.ToString());
            user.LogIn(logMess.Userlogin, logMess.Pavilion);
            if (!users.ContainsKey(logMess.Pavilion))
            {
                users.Add(logMess.Pavilion, new List<ChatUser>());
            }
            users[logMess.Pavilion].Add(user);
        }
        else
        {
            if (logMess.IsLogOut)
            {
                ChatUser user = GetUser(outHostId, outConnectionId, outChannelId);
                
                users[logMess.Pavilion].Remove(user);
                
                user.LogIn(null, null);
                
                unlogedUsers.Add(user);
                
            }

        }
    }
    
    /// <summary>
    ///  Обработка поступившего текстового сообщения от пользователя
    /// </summary>
    /// <param name="outHostId"> идентификатор хоста</param>
    /// <param name="outConnectionId"> идентификатор соединения</param>
    /// <param name="outChannelId"> идентификатор канала</param>
    /// <param name="mess"> сообщение с данными</param>
    private void RecieveMessage(int outHostId, int outConnectionId, int outChannelId, Message mess)
    {
        mess.SetId(messageID);
        messageID += 1;
        Debug.Log(outHostId + " Data from " + outConnectionId + " through the channel " + outChannelId +
                  " message is " + mess.ToString());
        uiController.AddLog(outHostId + " Data from " + outConnectionId + " through the channel " + outChannelId +
                            " message is " + mess.ToString());
        messages.Add(mess);
        if (mess.IsPrivate)
            SendMessage(mess, outHostId, outConnectionId);
        SendMessageToClients(mess);
    }

    /// <summary>
    /// Обработка сообщения с информацией об удалении\редактировании некоторого текстового сообщения
    /// </summary>
    /// <param name="outHostId"> идентификатор хоста</param>
    /// <param name="outConnectionId"> идентификатор соединения</param>
    /// <param name="outChannelId"> идентификатор канала</param>
    /// <param name="mess"> сообщение с данными</param>
    private void RecieveAdminMessage(int outHostId, int outConnectionId, int outChannelId, AdminMessage mess)
    {
        messageID += 1;
        Debug.Log(outHostId + " Data from " + outConnectionId + " through the channel " + outChannelId +
                  " message is " + mess.ToString());
        uiController.AddLog(outHostId + " Data from " + outConnectionId + " through the channel " + outChannelId +
                            " message is " + mess.ToString());
        Message message = FindMessageById(mess.IdMessage);
        if (message != null && mess.Delete)
        {
            messages.Remove(message);
            ChatUser sender = GetUserByLogin(message.Userlogin);
            SendMessageToClients(mess, sender.Pavilion);
        }
        if (message != null && mess.Delete)
        {
            int ind = messages.IndexOf(message);
            messages.Remove(message);
            message.Text = mess.Edit;
            messages.Insert(ind, message);
            ChatUser sender = GetUserByLogin(message.Userlogin);
            SendMessageToClients(mess, sender.Pavilion);
        }

        if (message != null && message.IsPrivate)
        {
            SendMessage(mess, outHostId, outConnectionId);
        }
    }

    /// <summary>
    /// Поиск сообщения по идентификатору
    /// </summary>
    /// <param name="id"> идентификатор необходимого сообщения</param>
    /// <returns> сообщение с нужным идентификатором</returns>
    private Message FindMessageById(int id)
    {
        foreach (Message message in messages)
        {
            if (message.Id == id)
            {
                return message;
            }
        }

        return null;
    }
    
    /// <summary>
    ///  Распределение сообщения по клиентам в зависимости от того является оно личным или общим
    /// </summary>
    /// <param name="message"> отправляемое сообщение</param>
    private void SendMessageToClients(Message message)
    {
        ChatUser sender = GetUserByLogin(message.Userlogin);
        if (message.IsPrivate)
        {
            ChatUser recepient = GetUserByLogin(message.Recipient);
            if (recepient != null)
            {
                SendMessage(message, recepient.HostId, recepient.ConnectionId);
            }
        }
        else
        {
            for (int i = 0; i < users[sender.Pavilion].Count; i++)
            {
                SendMessage(message, users[sender.Pavilion][i].HostId, users[sender.Pavilion][i].ConnectionId);
            }
        }
    }
    
    /// <summary>
    ///  Отправка сообщения всем пользователям в некотором павильоне
    /// </summary>
    /// <param name="message"> сообщение</param>
    /// <param name="pavilion"> павильон, в котором оно рассылается</param>
    private void SendMessageToClients(AdminMessage message, string pavilion)
    {
        for (int i = 0; i < users[pavilion].Count; i++)
            {
                SendMessage(message, users[pavilion][i].HostId, users[pavilion][i].ConnectionId);
            }
    }

    /// <summary>
    ///  Непосредственная отправка сообщения некоторому клиенту
    /// </summary>
    /// <param name="message"> сообщение</param>
    /// <param name="hostId"> идентификатор хоста клиента</param>
    /// <param name="connectionId"> идентификатор соединения клиента</param>
    private void SendMessage(Message message, int hostId, int connectionId)
    {
        IFormatter formatter = new BinaryFormatter();
        byte[] buffer;
        using (MemoryStream stream = new MemoryStream())
        {
            formatter.Serialize(stream, message);
            buffer = stream.ToArray();
        }

        NetworkTransport.Send(hostId, connectionId, reliableChannelId, buffer, buffer.Length, out error);
    }
    
    /// <summary>
    ///  Непосредственная отправка сообщения некоторому клиенту
    /// </summary>
    /// <param name="message"> сообщение</param>
    /// <param name="hostId"> идентификатор хоста клиента</param>
    /// <param name="connectionId"> идентификатор соединения клиента</param>
    private void SendMessage(AdminMessage message, int hostId, int connectionId)
    {
        IFormatter formatter = new BinaryFormatter();
        byte[] buffer;
        using (MemoryStream stream = new MemoryStream())
        {
            formatter.Serialize(stream, message);
            buffer = stream.ToArray();
        }

        NetworkTransport.Send(hostId, connectionId, reliableChannelId, buffer, buffer.Length, out error);
    }
    
    /// <summary>
    /// Проверка на наличие пользователя по данным подключения
    /// </summary>
    /// <param name="outHostId"> идентификатор хоста</param>
    /// <param name="outConnectionId"> идентификатор соединения</param>
    /// <param name="outChannelId"> идентификатор канала</param>
    /// <returns> есть ли искомый пользователь среди павильонов</returns>
    private bool IfUserExist(int outHostId, int outConnectionId, int outChannelId)
    {
        foreach (string key in users.Keys)
        {
            for (int i = 0; i < users[key].Count; i++)
            {
                if (users[key][i].IsIt(outHostId, outConnectionId, outChannelId))
                {
                    return true;
                }
            }
        }

        return false;
    }
    
    /// <summary>
    /// Поиск пользователя по данным подключения в павильонах
    /// </summary>
    /// <param name="outHostId"> идентификатор хоста</param>
    /// <param name="outConnectionId"> идентификатор соединения</param>
    /// <param name="outChannelId"> идентификатор канала</param>
    /// <returns> Искомый пользователь или null, при отсутствии такого</returns>
    private ChatUser GetUser(int outHostId, int outConnectionId, int outChannelId)
    {
        foreach (string key in users.Keys)
        {
            for (int i = 0; i < users[key].Count; i++)
            {
                if (users[key][i].IsIt(outHostId, outConnectionId, outChannelId))
                {
                    return users[key][i];
                }
            }
        }

        return null;
    }
    
    /// <summary>
    /// Поиск пользователя по логину в павильонах
    /// </summary>
    /// <param name="login"> логин пользователя</param>
    /// <returns>Искомый пользователь или null, при отсутствии такого</returns>
    private ChatUser GetUserByLogin(string login)
    {
        foreach (string key in users.Keys)
        {
            for (int i = 0; i < users[key].Count; i++)
            {
                if (users[key][i].Login.Equals(login))
                {
                    return users[key][i];
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Поиск пользователя среди незалогиненых по данным подключения
    /// </summary>
    /// <param name="outHostId"> идентификатор хоста</param>
    /// <param name="outConnectionId"> идентификатор соединения</param>
    /// <param name="outChannelId"> идентификатор канала</param>
    /// <returns> Искомый пользователь или null, при отсутствии такого</returns>
    private ChatUser GetUnloggedUser(int outHostId, int outConnectionId, int outChannelId)
    {
        for (int i = 0; i < unlogedUsers.Count; i++)
        {
            if (unlogedUsers[i].IsIt(outHostId, outConnectionId, outChannelId))
            {
                return unlogedUsers[i];
            }
        }

        return null;
    }
    
}
