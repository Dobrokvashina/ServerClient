using System;

[Serializable]
public class LoginMessage
{
    private string date; // дата входа\выхода
    private string time; // время входа\выхода
    private string userlogin; // логин пользователя
    private string pavilion; // павильон, в который пользователь вошел\вышел
    private bool isLogOut = false; // вход(false) или выход(true) в павильон осуществляется

    /// <summary>
    /// Конструктор сообщения с данными о входе\выходе пользователя в чат павильона
    /// </summary>
    /// <param name="userlogin"> логин пользователя</param>
    /// <param name="exhibition"> название выставки</param>
    /// <param name="pavilion"> название павильона</param>
    /// <param name="isLogOut"> вход(false - по умолчанию) или выход(true) в павильон осуществляется</param>
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