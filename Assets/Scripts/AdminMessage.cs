using System;

[Serializable]
public class AdminMessage
{
    private int idMessage;
    private bool delete;
    private string edit;

    public AdminMessage(int idMessage, bool delete, string edit)
    {
        this.idMessage = idMessage;
        this.delete = delete;
        this.edit = edit;
    }

    public int IdMessage => idMessage;

    public bool Delete => delete;

    public string Edit => edit;
}