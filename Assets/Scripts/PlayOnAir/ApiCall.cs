using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;

public partial class ApiCall : SingletonBehaviour<ApiCall>
{
    private Coroutine _postCoroutine;
    private Coroutine _getCoroutine;
    //[SerializeField]
    //private List<Coroutine> _getCoroutines = new List<Coroutine>();
    private int _requestNum = 0;
    private const int _requestMaxNum = 3;
    private const string _googleDownUrl = "https://drive.google.com/uc?export=download&id=";
    private string _downloadPath;

    protected override void Init()
    {
        OnEndRequest = () =>
        {
            _curRequestCount++;
            _requestDone = true;
        };
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
            CustomLogger.Log(www.error);
            CustomLogger.Log(www.downloadHandler.text);

            if (_requestNum < _requestMaxNum)
            {
                www.Dispose();
                _postCoroutine = StartCoroutine(PostRequest(url, json, response, true));
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

    public IEnumerator GetRequest<T>(string url, Action<T> response = null, bool isReRequest = false)
    {
        if (!isReRequest)
        {
            _requestNum = 0;
        }

        _requestNum++;
        UnityWebRequest www = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
        DownloadHandlerBuffer dh = new DownloadHandlerBuffer();

        www.SetRequestHeader("Content-Type", "application/json");
        www.downloadHandler = dh;

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            CustomLogger.Log(www.error);
            CustomLogger.Log(www.downloadHandler.text);

            if (_requestNum < _requestMaxNum)
            {
                www.Dispose();
                _getCoroutine = StartCoroutine(GetRequest(url, response, true));
                yield break;
            }
            else
            {
                Debug.LogFormat("[Get / request count {0}] Fail to Send!", _requestNum);
            }
        }
        else
        {
            object result = null;

            if (typeof(T) == typeof(string))
            {
                result = www.downloadHandler.text;
            }
            else if (typeof(T) == typeof(Texture2D))
            {
                result = DownloadHandlerTexture.GetContent(www);
            }

            Debug.LogFormat("[Get / request count {0}] Successed to Send!", _requestNum);
            response?.Invoke((T)result);
        }
        www.Dispose();
    }

    public IEnumerator GetRequestGoogleLink<T>(string url, Action<T> response = null, bool isReRequest = false, bool isSequential = false)
    {
        if (!isReRequest)
        {
            _requestNum = 0;
        }

        string key = ExtractGoogleDownKey(url);
        url = _googleDownUrl + key;

        _requestNum++;
        UnityWebRequest www = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
        DownloadHandlerBuffer dh = new DownloadHandlerBuffer();

        www.SetRequestHeader("Content-Type", "application/json");
        www.downloadHandler = dh;

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            CustomLogger.Log(www.error);
            CustomLogger.Log(www.downloadHandler.text);

            if (_requestNum < _requestMaxNum)
            {
                www.Dispose();
                if(!isSequential)
                {
                    _getCoroutine = StartCoroutine(GetRequestGoogleLink(url, response, true));
                }
                yield break;
            }
            else
            {
                Debug.LogFormat("[Get / request count {0}] Fail to Send!", _requestNum);
            }
        }
        else
        {
            Debug.LogFormat("[Get / request count {0}] Successed to Send!", _requestNum);

            object result = null;
            string contentType = www.GetResponseHeader("Content-Type");
            if (contentType != null)
            {
                if (contentType.StartsWith("image/"))
                {
                    byte[] data = www.downloadHandler.data;
                    Texture2D texture = new Texture2D(0, 0, TextureFormat.ARGB32, false);
                    texture.LoadImage(data);
                    result = texture;
                }
                else if (contentType.StartsWith("video/"))
                {
                    _downloadPath = Path.Combine(Application.streamingAssetsPath, $"Video/{key}.mp4");
                    byte[] data = www.downloadHandler.data;
                    File.WriteAllBytes(_downloadPath, data);
                    result = _downloadPath;
                }
                else if(contentType.Contains("application/octet-stream")) // SVG 인 경우 혹은 data인 경우
                {
                    byte[] data = www.downloadHandler.data;
                    result = data;
                }
                else
                {
                    Debug.Log($"Unknown file type. : {contentType}");
                }
            }

            response?.Invoke((T)result);
        }
        www.Dispose();
    }

    public void Post(string url, string json, Action<string> response = null)
    {
        if(_postCoroutine != null)
        {
            StopCoroutine(_postCoroutine);
            _postCoroutine = null;
        }
        _postCoroutine = StartCoroutine(PostRequest(url, json, response));
    }

    public void Get<T>(string url, Action<T> response = null, bool isGoogleDownload = false)
    {
        if (_getCoroutine != null)
        {
            StopCoroutine(_getCoroutine);
            _getCoroutine = null;
        }

        if (isGoogleDownload)
        {
            _getCoroutine = StartCoroutine(GetRequestGoogleLink(url, response));
        }
        else
        {
            _getCoroutine = StartCoroutine(GetRequest(url, response));
        }
    }

    public void GetSequently<T>(string url, Action<T> response = null, bool isGoogleDownload = false)
    {
        if (isGoogleDownload)
        {
            StartCoroutine(GetRequestGoogleLink(url, response, false, true));
        }
        else
        {
            //sequential 대응필요
            StartCoroutine(GetRequest(url, response));
        }
    }

    public void StopActiveCoroutine()
    {
        if (_postCoroutine != null)
        {
            StopCoroutine(_postCoroutine);
            _postCoroutine = null;
        }
    }

    private string ExtractGoogleDownKey(string url)
    {
        try
        {
            string startPattern = "file/d/";
            string endPattern = "/view";

            var startindex = url.IndexOf(startPattern) + startPattern.Length;
            var endindex = url.IndexOf(endPattern);

            return url.Substring(startindex, endindex - startindex);
        }
        catch (Exception ex)
        {
            CustomLogger.LogError(ex);
            return string.Empty;
        }
    }
}