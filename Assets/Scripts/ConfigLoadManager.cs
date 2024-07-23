using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

public class ConfigLoadManager : SingletonBehaviour<ConfigLoadManager>
{
    protected override void Init()
    {
        UpdateConfig();
    }

    public void UpdateConfig()
    {
#if UNITY_EDITOR
        string text = File.ReadAllText(Path.Combine(Application.dataPath, "config.json"));
#else
        string text = File.ReadAllText(Path.Combine(Directory.GetParent(Application.dataPath).FullName, "config.json"));
#endif
        ConfigData.config = JsonUtility.FromJson<Config>(text);
    }

    public void SaveConfig()
    {
#if UNITY_EDITOR
        string path = Path.Combine(Application.dataPath, "config.json");
#else
        string path = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "config.json");
#endif
        string text = JsonUtility.ToJson(ConfigData.config, true);
        File.WriteAllText(path, text);
    }
}
