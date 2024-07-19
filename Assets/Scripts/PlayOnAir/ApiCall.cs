using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using Vivestudios.UI;
using Newtonsoft.Json;

public partial class ApiCall : SingletonBehaviour<ApiCall>
{
    private List<CartoonRequestBody> cartoonTemplate = new List<CartoonRequestBody>();
    private List<string> cartoonJsons = new List<string>();
    private List<string> beautyJsons = new List<string>();

    private Coroutine _setPostCoroutine;

    private int _requestNum = 0;
    private const int _requestMaxNum = 3;

    protected string _cartoonAPI = "http://api.playon-vive.com/ai-cartoon?api_key=1ef5ba12-5773-4fc0-837c-9af7a926e2db";
    protected string _profileAPI = "http://api.playon-vive.com/ai-profile?api_key=1ef5ba12-5773-4fc0-837c-9af7a926e2db";

    public string profileAPI => _profileAPI;

    protected override void Init()
    {
        //videoPath = Path.Combine(Application.persistentDataPath, videoName);
        //throw new NotImplementedException();
        //유저데이터 서버사용인지 아닌지 확인해서 적용
        //UserDataManager.inst.url_setmodel;
        OnEndRequest = () =>
        {
            _curRequestCount++;
            _requestDone = true;
        };
    }

    public void GetCartoonTemplate()
    {
        string cartoonPath = Path.Combine(Application.streamingAssetsPath, "Cartoon");
        cartoonJsons = GetJsonFiles(Path.GetFullPath(cartoonPath));

        string beautyPath = Path.Combine(Application.streamingAssetsPath, "Beauty");
        beautyJsons = GetJsonFiles(Path.GetFullPath(beautyPath));
        //cartoonTemplate = new List<CartoonRequestBody>(GetBodyFromJson(jsonNames));
    }

    List<string> GetJsonFiles(string folderPath)
    {
        List<string> jsonFiles = new List<string>();

        if (Directory.Exists(folderPath))
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
            FileInfo[] fileInfos = directoryInfo.GetFiles("*.json");

            string[] files = Directory.GetFiles(folderPath, "*.json", SearchOption.AllDirectories);
            Array.Sort(files);
            foreach (string file in files)
            {
                string fileData = File.ReadAllText(file);
                jsonFiles.Add(fileData);
            }
        }
        else
        {
            Debug.LogError("Folder does not exist: " + folderPath);
        }

        return jsonFiles;
    }

    public IEnumerator PostRequest(string url, string json, Action<string> response = null, bool isReRequest = false)
    {
        if (!isReRequest)
        {
            _requestNum = 0;
        }

        _requestNum++;
        UnityWebRequest www = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
        DownloadHandlerBuffer dh = new DownloadHandlerBuffer();

        www.SetRequestHeader("Content-Type", "application/json");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = dh;

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            Debug.Log(www.downloadHandler.text);

            if (_requestNum < _requestMaxNum)
            {
                www.Dispose();
                _setPostCoroutine = StartCoroutine(PostRequest(url, json, response, true));
                yield break;
            }
            else
            {
                Debug.LogFormat("[POST / request count {0}] Fail to Send!", _requestNum);
                GameManager.inst.SetDiffusionState(false);
            }
        }
        else
        {
            Debug.LogFormat("[POST / request count {0}] Successed to Send!", _requestNum);
            response?.Invoke(www.downloadHandler.text);
        }
        www.Dispose();
    }

    public void Post(string url, string json, Action<string> response = null)
    {
        if(_setPostCoroutine != null)
        {
            StopCoroutine(_setPostCoroutine);
            _setPostCoroutine = null;
        }
        _setPostCoroutine = StartCoroutine(PostRequest(url, json, response));
    }

    public void StopActiveCoroutine()
    {
        if (_setPostCoroutine != null)
        {
            StopCoroutine(_setPostCoroutine);
            _setPostCoroutine = null;
        }
    }
}