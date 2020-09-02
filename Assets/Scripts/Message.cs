using System;

[Serializable]
public class Message
{
    private int id;
    private string text;
    private string date;
    private string time;
    private string userlogin;
    private bool isPrivate;
    private string recipient;

    public Message(string text, string userlogin, bool isPrivate, string recipient)
    {
        this.text = text;
        date = DateTime.Now.ToShortDateString();
        time = DateTime.Now.ToShortTimeString();
        this.userlogin = userlogin;
        this.isPrivate = isPrivate;
        this.recipient = recipient;
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

    public string Date => date;

    public string Time => time;

    public string Userlogin => userlogin;

    public bool IsPrivate => isPrivate;

    public string Recipient => recipient;
}