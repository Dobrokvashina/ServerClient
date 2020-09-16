public class ChatUser
{
    
    private int hostId; // идентификатор хоста
    private int connectionId; // идентификатор соединения
    private int channelId; // идентификатор канала
    
    private string login; // логин пользователя
    private string pavilion; // павильон в котором находится пользователь

    public int HostId => hostId;

    public int ConnectionId => connectionId;

    public int ChannelId => channelId;

    public string Login => login;

    public string Pavilion => pavilion;

    /// <summary>
    /// Конструктор для создания экземпляра нового подключившегося пользователя
    /// </summary>
    /// <param name="hostId"> идентификатор хоста</param>
    /// <param name="connectionId"> идентификатор соединения</param>
    /// <param name="channelId"> идентификатор канала</param>
    public ChatUser(int hostId, int connectionId, int channelId)
    {
        this.hostId = hostId;
        this.connectionId = connectionId;
        this.channelId = channelId;
    }

    /// <summary>
    /// Функция для присвоения логина и павильона подключившемуся пользователю
    /// </summary>
    /// <param name="login"> логин пользователя</param>
    /// <param name="pavilion"> павильон в который вошел пользователь</param>
    public void LogIn(string login, string pavilion)
    {
        this.login = login;
        this.pavilion = pavilion;
    }

    
    public bool IsLoggedIn()
    {
        return login != null;
    }
    
    /// <summary>
    /// Функция для проверки соответствия данных подключения
    /// </summary>
    /// <param name="hostId"> идентификатор хоста</param>
    /// <param name="connectionId"> идентификатор соединения</param>
    /// <param name="channelId"> идентификатор канала</param>
    /// <returns> Совпадают ли у экземпляра данные подключения с заданными</returns>
    public bool IsIt(int hostId, int connectionId, int channelId)
    {
        return this.hostId == hostId && this.connectionId == connectionId && this.channelId == channelId;
    }
}