public class ChatUser
{
    private int hostId;
    private int connectionId;
    private int channelId;
    private string login;
    private string pavilion;

    public int HostId => hostId;

    public int ConnectionId => connectionId;

    public int ChannelId => channelId;

    public string Login => login;

    public string Pavilion => pavilion;


    public ChatUser(int hostId, int connectionId, int channelId)
    {
        this.hostId = hostId;
        this.connectionId = connectionId;
        this.channelId = channelId;
    }

    public void LogIn(string login, string pavilion)
    {
        this.login = login;
        this.pavilion = pavilion;
    }

    public bool IsLoggedIn()
    {
        return login != null;
    }

    public bool IsIt(int hostId, int connectionId, int channelId)
    {
        return this.hostId == hostId && this.connectionId == connectionId && this.channelId == channelId;
    }
}