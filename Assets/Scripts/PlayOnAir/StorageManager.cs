using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using Vivestudios.UI;

public class StorageManager : SingletonBehaviour<StorageManager>
{
    string url_qr_upload = "http://qr.snapai-vive.com/upload/";
    string url_qr_download = "http://qr.snapai-vive.com/download-file/";

    //Temp save
    string folderName = "savePic";
    string folderPath;

    protected override void Init()
    {
        folderPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, folderName);
        CreateStorageFolder();
    }

    public void CreateStorageFolder()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(TextData.storageFolderPath);

        if (!directoryInfo.Exists)
            directoryInfo.Create();
    }

    public void SavePicture(string name, Texture2D texture)
    {
        byte[] bytes = texture.EncodeToPNG();
        DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);

        if (!directoryInfo.Exists)
            directoryInfo.Create();

        File.WriteAllBytes(Path.Combine(folderPath, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss_")+name+".png"), bytes);
    }

    public void StartUploadCloud(List<string> names, List<byte[]> datas, Action<string> SendLink)
    {
        StartCoroutine(UploadCloud(names, datas, SendLink));
    }

    public void StartUploadCloud(List<byte[]> datas, Action<string> SendLink)
    {
        StartCoroutine(UploadCloud(datas, SendLink));
    }

    IEnumerator UploadCloud(List<string> names, List<byte[]> datas, Action<string> SendLink)
    {
        WWWForm form = new WWWForm();
        for (int i = 0; i < names.Count; i++)
        {
            form.AddBinaryData("files", datas[i], names[i]+ DateTime.Now.ToString("MM-dd-HH-mm"));
        }

        UnityWebRequest request = UnityWebRequest.Post(url_qr_upload, form);
        request.timeout = 10;
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            CustomLogger.LogError(request.error);
        }
        else
        {
            ResponseData responseData = JsonUtility.FromJson<ResponseData>(request.downloadHandler.text);

            // Get the desired value
            string message = responseData.message;
            SendLink(url_qr_download + message);
            CustomLogger.Log("Form upload complete! : " + url_qr_download + message);
        }

        request.Dispose();
    }

    IEnumerator UploadCloud(List<byte[]> datas, Action<string> SendLink)
    {
        WWWForm form = new WWWForm();
        for (int i = 0; i < TextData.filePaths.Length; i++)
        {
            form.AddBinaryData("files", datas[i], TextData.filePaths[i]);
        }

        UnityWebRequest request = UnityWebRequest.Post(url_qr_upload, form);
        request.timeout = 600;
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            CustomLogger.LogError(request.error);
            GameManager.inst.SetQRUploadState(false);
        }
        else
        {
            CustomLogger.Log(request.downloadHandler.text);
            ResponseData responseData = JsonUtility.FromJson<ResponseData>(request.downloadHandler.text);

            // Get the desired value
            string message = responseData.message;
            SendLink(url_qr_download + message);
            CustomLogger.Log("Form upload complete! : " + url_qr_download + message);
        }

        request.Dispose();
    }
}
public class ResponseData
{
    public string message;
}
