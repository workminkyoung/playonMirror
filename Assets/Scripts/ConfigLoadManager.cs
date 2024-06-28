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
#if UNITY_EDITOR
        string text = File.ReadAllText(Path.Combine(Application.dataPath, "config.json"));
#else
        string text = File.ReadAllText(Path.Combine(Directory.GetParent(Application.dataPath).FullName, "config.json"));
#endif
        ConfigData.config = JsonUtility.FromJson<Config>(text);

    }
}
