using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogHandler : MonoBehaviour
{

    [SerializeField] private TMP_Text debugWindowText;
    [SerializeField] private ScrollRect debugScrollRect;

    void OnEnable()
    {
        Application.logMessageReceived += DisplayLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= DisplayLog;
    }

    public void DisplayLog(string logString, string stackTrace, LogType type)
    {
        debugWindowText.text += logString + System.Environment.NewLine;
        Canvas.ForceUpdateCanvases();
        debugScrollRect.normalizedPosition = new Vector2(0, 0);
    }
}
