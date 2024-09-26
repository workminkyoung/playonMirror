using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class PhotoPaperCheckModule
{
    public static string PATH = Path.Combine(Application.persistentDataPath, "PhotoPaper.remain");

    public static int GetRemainPhotoPaper()
    {
        int remainNum = 0;
        try
        {
            // 파일이 없다면 생성 후 초기화
            if (!File.Exists(PATH))
            {
                File.WriteAllText(PATH, "400");
            }

            string fileContent = File.ReadAllText(PATH);
            // 파일 null check
            if (string.IsNullOrEmpty(fileContent))
            {
                CustomLogger.LogWarning("Photo Paper Remain Nothing, Reset To 0");
                remainNum = 0;
                fileContent = remainNum.ToString();
                File.WriteAllText(PATH, fileContent);
            }

            if (!int.TryParse(fileContent, out remainNum))
            {
                CustomLogger.LogWarning("Photo Paper Remain Nothing, Reset To 0");
                remainNum = 0;
                fileContent = remainNum.ToString();
                File.WriteAllText(PATH, fileContent);
            }

            CustomLogger.Log($"Photo Remain {remainNum}");
        }
        catch (IOException ioEx)
        {
            CustomLogger.LogException(ioEx);
            remainNum = 0;
        }
        catch (UnauthorizedAccessException authEx)
        {
            CustomLogger.LogException(authEx);
            remainNum = 0;
        }
        catch (Exception ex)
        {
            // 그외모든 예외 에러 케이스
            CustomLogger.LogException(ex);
            remainNum = 400;
        }

        return remainNum;
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
