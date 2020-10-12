using System;

[Serializable]
public class ReadMessage
{
    private string login;
    private int[] messagesId;

    /// <summary>
    /// Конструктор сообщения о прочитанном
    /// </summary>
    /// <param name="messagesId"> массив идентификаторов прочитанного</param>
    public ReadMessage(int[] messagesId, string login)
    {
        this.messagesId = messagesId;
        this.login = login;
    }

    public int[] MessagesId => messagesId;

    public string Login => login;
}