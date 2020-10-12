using System;
using System.Collections.Generic;

[Serializable]
public class Message
{
    private int id; // идентефикатор сообщения
    private string text; // текст сообщения
    private string date; // дата отправки сообщения
    private string time; // время отправки сообщения
    private string userlogin; // логин отправителя
    private bool isPrivate; // переменная, значение которой описывает, является ли сообщение личным или оно было отправлено в общий чат(комнату)
    private string recipient; // получатель сообщения(логин получателя или название чата(комнаты))
    private List<string> readen; // прочитано ли сообщение

    /// <summary>
    /// Контейнер для информации о сообщении для пересылки между сервером и клиентом
    /// </summary>
    /// <param name="text"> Текст сообщения</param>
    /// <param name="userlogin"> Логин отправителя</param>
    /// <param name="isPrivate"> Является ли сообщение личным или оно было отправлено в общий чат(комнату)</param>
    /// <param name="recipient"> Логин получателя или название чата (комнаты), куда отправлено сообщение</param>
    public Message(string text, string userlogin, bool isPrivate, string recipient)
    {
        this.text = text;
        date = DateTime.Now.ToShortDateString();
        time = DateTime.Now.ToShortTimeString();
        this.userlogin = userlogin;
        this.isPrivate = isPrivate;
        this.recipient = recipient;
        readen = new List<string>() {userlogin};
    }

    public string ToString()
    {
        return userlogin + " at " + time + " " + date + " said to " + recipient + " next: "+ text;
    }

    public void SetId(int id)
    {
        this.id = id;
    }

    public int Id => id;

    public string Text
    {
        get => text;
        set => text = value;
    }

    public List<string> Readen
    {
        get => readen;
        set => readen = value;
    }

    public string Date => date;

    public string Time => time;

    public string Userlogin => userlogin;

    public bool IsPrivate => isPrivate;

    public string Recipient => recipient;
}