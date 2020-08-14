using System;

[Serializable]
public class LoginMessage
{
    private string date;
    private string time;
    private string userlogin;
    private string pavilion;
    private bool isLogOut = false;

    public LoginMessage(string userlogin, string exhibition, string pavilion, bool isLogOut = false)
    {
        this.pavilion = exhibition + ":" + pavilion;
        date = DateTime.Now.ToShortDateString();
        time = DateTime.Now.ToShortTimeString();
        this.userlogin = userlogin;
        this.isLogOut = isLogOut;
    }

    public string Userlogin => userlogin;

    public bool IsLogOut => isLogOut;

    public string Pavilion => pavilion;

    public string ToString()
    {
        return userlogin + " at " + time + " " + date + " connected to chat server of " + pavilion;
    }
}