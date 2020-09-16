using System;

[Serializable]
public class AdminMessage
{
    private int idMessage; // идентефикатор сообщения
    private bool delete; // переменная с информацией об удалении
    private string edit; // измененный текст
    
    /// <summary>
    /// Ккнтейнер для информации об удалении или редактировании сообщения
    /// </summary>
    /// <param name="idMessage"> идентификатор изменяемого сообщения</param>
    /// <param name="delete"> удаляется ли сообщение</param>
    /// <param name="edit"> новый текст сообщения</param>
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