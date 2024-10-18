using System.IO;
using UnityEngine;

public class ExceptionLogger : MonoBehaviour
{
    System.IO.StreamWriter sw;
    string createdAt;
    string LogFileName;
    string LogFilePath;
    private int logCount = 0;
    private const int FlushInterval = 5;

    async void Start()
    {
        createdAt = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        LogFileName = $"ErrorLog_{createdAt}.txt";
        LogFilePath = Application.persistentDataPath + $"/{LogFileName}";
        DontDestroyOnLoad(gameObject);

        try
        {
            sw = new StreamWriter(Application.persistentDataPath + "/" + LogFileName);
            await sw.WriteLineAsync($"Log file created at: {createdAt}");
#if UNITY_EDITOR
            print($"Log file path: {LogFilePath}");
#endif
        }
        catch (IOException e)
        {
            Debug.LogError("Failed to create log file: " + e.Message);
        }
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    async void OnDestroy()
    {
        if (sw != null)
        {
            try
            {
                await sw.FlushAsync();
                sw.Close();
            }
            catch (IOException e)
            {
                Debug.LogError("Failed to close log file: " + e.Message);
            }
        }
    }

    async void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type is LogType.Exception or LogType.Error)
        {
            string logEntry = "Logged at: " + System.DateTime.Now.ToString() +
                              " - Log: " + logString +
                              " - Trace: " + stackTrace +
                              " - Type: " + type.ToString();
            if (sw != null)
            {
                try
                {
                    await sw.WriteLineAsync(logEntry);
                    logCount++;

                    if (logCount >= FlushInterval)
                    {
                        await sw.FlushAsync();
                        logCount = 0;
                    }
                }
                catch (IOException e)
                {
                    Debug.LogError("Failed to write log: " + e.Message);
                }
            }
        }
    }
}
