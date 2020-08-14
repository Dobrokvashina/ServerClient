using System;

[Serializable]
public class AdminMessage
{
    private int idMessage;

    public AdminMessage(int idMessage)
    {
        this.idMessage = idMessage;
    }

    public int IdMessage => idMessage;
}