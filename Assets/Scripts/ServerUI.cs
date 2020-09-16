using System;
using UnityEngine;
using UnityEngine.UI;

public class ServerUI : MonoBehaviour
{
    [SerializeField]private Text log; // сслыка на текстовое поле с логом

    [SerializeField]private int MAX_ROW_LOG = 10; // максимальное количество строк в выводе

    private int currentRow = 0; // нынешнее количество строк в выводе

    // обнуление текста в выводе
    void Start()
    {
        log.text = "";
    }

    /// <summary>
    /// Добавление нового лога к выводу
    /// </summary>
    /// <param name="text"> текст лога</param>
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

    /// <summary>
    /// Удаление первой строки в выводе
    /// </summary>
    private void DeleteFirstRow()
    {
        int pos = log.text.IndexOf("\n", StringComparison.Ordinal);
        log.text = log.text.Substring(pos+1);
        currentRow -= 1;
    }
    
}
