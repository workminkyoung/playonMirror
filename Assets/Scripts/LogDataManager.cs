using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Vivestudios.UI;

public class LogDataManager : SingletonBehaviour<LogDataManager>
{
    private string url = "";
    private string fileUrl = "";
    private string guidFilePath;
    private string guid = null;

    public LogFormat logFormat = new LogFormat();

    public string GetGuid => guid;

    protected override void Init()
    {
        GenerateGUID();
        logFormat.pc_uuid = guid;// SystemInfo.deviceUniqueIdentifier;

        if (Debug.isDebugBuild)
        {
            url = "http://3.35.3.44:1996/logs";
            fileUrl = "http://3.35.3.44:1996/data";
        }
        else
        {
            url = "http://43.200.46.181:1996/logs";
            fileUrl = "http://43.200.46.181:1996/data";
        }

#if UNITY_EDITOR
        url = "http://3.35.3.44:1996/logs";
        fileUrl = "http://3.35.3.44:1996/data";
#endif
    }

    public void GenerateGUID()
    {
        guidFilePath = Application.persistentDataPath + "/" + "guid.txt";
        guid = LoadGUIDFromFile();

        if (guid == null)
        {
            Guid newGuid = Guid.NewGuid();
            Debug.Log("Create New GUID: " + newGuid.ToString());
            File.WriteAllText(guidFilePath, newGuid.ToString());
        }
    }

    string LoadGUIDFromFile()
    {
        try
        {
            if (File.Exists(guidFilePath))
            {
                string loadedGuid = File.ReadAllText(guidFilePath);
                string cleanGuid = loadedGuid.Replace(" ", string.Empty).Replace("\r", string.Empty).Replace("\t", string.Empty);

                Debug.Log("Load GUID: " + cleanGuid);
                return cleanGuid;
            }
            else
            {
                Debug.Log("GUID file does not exist.");
                return null;
            }
        }
        catch (Exception e)
        {
            Debug.Log("Failed to load GUID: " + e.Message);
            return null;
        }
    }

    public void SendLog()
    {
        logFormat.print_count = UserDataManager.inst.selectedFrameType == FRAME_TYPE.FRAME_8 ? UserDataManager.inst.curPicAmount / 2 : UserDataManager.inst.curPicAmount;
        logFormat.payment_amount = UserDataManager.inst.curPrice;

        switch (UserDataManager.inst.selectedContent)
        {
            case CONTENT_TYPE.AI_CARTOON:
                logFormat.played_menu_categories = UserDataManager.inst.selectedSubContentKey;
                logFormat.profile_index = -1;
                break;
            case CONTENT_TYPE.AI_PROFILE:
                logFormat.played_menu_categories = UserDataManager.inst.selectedSubContentKey;
                logFormat.profile_index = UserDataManager.inst.selectedProfilePicNum;
                break;
            case CONTENT_TYPE.AI_BEAUTY:
                logFormat.played_menu_categories = "BT";
                logFormat.profile_index = -1;
                break;
            case CONTENT_TYPE.WHAT_IF:
                logFormat.played_menu_categories = UserDataManager.inst.selectedSubContentKey;
                logFormat.profile_index = UserDataManager.inst.selectedProfilePicNum;
                break;
        }

        logFormat.frame = UserDataManager.inst.selectedFrameKey;
        logFormat.frame_shape = null;

        logFormat.color_filter = UserDataManager.inst.selectedLutKey;

        logFormat.frame_color = UserDataManager.inst.selectedFrameColor;


#if UNITY_EDITOR
        url = TextData.testLog_url;
        fileUrl = TextData.testLog_fileUrl;
#else
        if (!GameManager.inst.isPaymentOn || Debug.isDebugBuild)
        {
            //service mode -> test log
            url = TextData.testLog_url;
            fileUrl = TextData.testLog_fileUrl;
        }
        else
        {
            url = TextData.releaseLog_url;
            fileUrl = TextData.releaseLog_fileUrl;
        }
#endif
        // 재시작 코드 뺄때 수정해야함

        logFormat.timestamp_utc = DateTime.UtcNow.ToString("yyyy-MM-dd") + "T" + DateTime.UtcNow.ToString("HH_mm_ss_ff");
        logFormat.timestamp_kst = DateTime.Now.ToString("yyyy-MM-dd") + "T" + DateTime.Now.ToString("HH_mm_ss_ff");

        CustomLogger.Log(JsonUtility.ToJson(logFormat, true));

        StartCoroutine(SendLogRoutine(JsonUtility.ToJson(logFormat), GameManager.inst.isChildPlaying));
    }

    private IEnumerator SendLogRoutine(string data, bool isChlidPlaying)
    {
        WWWForm form = new WWWForm();
        form.AddField("timestamp", logFormat.timestamp_utc);
        form.AddField("uuid", logFormat.pc_uuid);
        form.AddField("logs", data);

        using (UnityWebRequest req = UnityWebRequest.Post(url, form))
        {
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                CustomLogger.Log("LOG_SUCCESS : " + req.downloadHandler.text);
            }
            else
            {
                CustomLogger.Log("LOG_ERROR : " + req.error);
            }
        }

        if (!isChlidPlaying)
        {
            form = new WWWForm();
            form.AddField("timestamp", logFormat.timestamp_utc);
            form.AddField("uuid", logFormat.pc_uuid);
            form.AddBinaryData("files", File.ReadAllBytes(PhotoDataManager.inst.imagePath), "Image.png");// imagePath));
            form.AddBinaryData("files", File.ReadAllBytes(PhotoDataManager.inst.videoPath), "Video.mp4");

            for (int i = 0; i < PhotoDataManager.inst.recordPaths.Count; i++)
            {
                form.AddBinaryData("files", File.ReadAllBytes(PhotoDataManager.inst.recordPaths[i]), $"Videos{i}.mp4");
            }

            for (int i = 0; i < PhotoDataManager.inst.photoOrigin.Count; i++)
            {
                form.AddBinaryData("files", PhotoDataManager.inst.photoOrigin[i].EncodeToPNG(), "OriginPhoto_" + i + ".png", "images/png");
            }

            for (int i = 0; i < PhotoDataManager.inst.photoConverted.Count; i++)
            {
                form.AddBinaryData("files", PhotoDataManager.inst.photoConverted[i].EncodeToPNG(), "ConvertedPhoto_" + i + ".png", "images/png");
            }

            if (ConfigData.config.camType == 2)
            {
                for (int i = 0; i < PhotoDataManager.inst.dslrPhotos.Count; i++)
                {
                    form.AddBinaryData("files", PhotoDataManager.inst.dslrPhotos[i], "DSLR_Photo_" + i + ".jpg");
                }
            }

            using (UnityWebRequest req = UnityWebRequest.Post(fileUrl, form))
            {
                yield return req.SendWebRequest();

                if (req.result == UnityWebRequest.Result.Success)
                {
                    CustomLogger.Log("FILE_SUCCESS : " + req.downloadHandler.text);
                }
                else
                {
                    CustomLogger.Log("FILE_ERROR : " + req.error);
                }
            }
        }
    }
}

[Serializable]
public class LogFormat
{
    public string pc_uuid;
    public string timestamp_utc;
    public string timestamp_kst;
    public int print_count;
    public int payment_amount;
    public string played_menu_categories;
    public int profile_index;
    public string frame;
    public string color_filter;
    public string frame_color;
    public string frame_shape;
}