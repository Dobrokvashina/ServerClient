using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageUI : MonoBehaviour
{
    private int id;
    [SerializeField] private Text testOfMessage;
    [SerializeField] private Text timeOfMessage;

    public void SetMessage(Message message)
    {
        testOfMessage.text = message.Text;
        timeOfMessage.text = message.Time;
        // this.id = message.Id;
    }

    public int Id => id;
}
