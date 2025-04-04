using UnityEngine;
using TMPro;

public class InGameLogger : MonoBehaviour
{
    public TextMeshProUGUI debugText;
    private string log = "";

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        log = logString + "\n" + log;
        if (debugText != null)
        {
            debugText.text = log;
        }
    }
}
