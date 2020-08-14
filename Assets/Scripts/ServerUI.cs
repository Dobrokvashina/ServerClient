using System;
using UnityEngine;
using UnityEngine.UI;

public class ServerUI : MonoBehaviour
{
    [SerializeField]private Text log;

    [SerializeField]private int MAX_ROW_LOG = 30;

    private int currentRow = 0;

    void Start()
    {
        log.text = "";
    }

    public void AddLog(string text)
    {
        if (text.Contains("\n"))
            currentRow += 1;
        log.text += text + "\n";
        currentRow += 1;
        if (currentRow > MAX_ROW_LOG)
        {
            DeleteFirstRow();
        }
    }

    private void DeleteFirstRow()
    {
        int pos = log.text.IndexOf("\n", StringComparison.Ordinal);
        log.text = log.text.Substring(pos+1);
        currentRow -= 1;
    }
    
}
