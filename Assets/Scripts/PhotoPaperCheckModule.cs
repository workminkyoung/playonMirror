using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class PhotoPaperCheckModule
{
    public static string PATH = Path.Combine(Application.persistentDataPath, "PhotoPaper.remain");

    public static int GetRemainPhotoPaper()
    {
        if (!File.Exists(PATH))
        {
            File.WriteAllText(PATH, "400");
        }
        return int.Parse(File.ReadAllText(PATH));
    }

    public static void SetRemainPhotoPaper(int remainNum)
    {
        if (!File.Exists(PATH))
        {
            File.WriteAllText(PATH, $"{remainNum}");
        }
        File.WriteAllLines(PATH, new string[] { $"{remainNum}" });
    }
}
