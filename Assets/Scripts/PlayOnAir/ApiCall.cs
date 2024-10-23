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
    private Coroutine _patchCoroutine;
    //[SerializeField]
    //private List<Coroutine> _getCoroutines = new List<Coroutine>();
    //private int _requestNum = 0;
    private const int _requestMaxNum = 10;
    private const string _googleDownUrl = "https://drive.google.com/uc?export=download&id=";
    private string _downloadPath;
    private List<bool> _requestCompleted = new List<bool>();
    protected string _couponAPIUrl = "http://api.playon-vive.com/v1/coupon/";

    public string CouponAPIUrl => _couponAPIUrl; 

    protected override void Init ()
    {
        OnEndRequest = () =>
        {
            _curRequestCount++;
            _requestDone = true;
        };
    }

    public IEnumerator PatchRequest(string url, string json, Action<string> response = null, bool isReRequest = false, int ReIndex = 0)
    {
        int _requestNum = 0 + ReIndex;

        _requestNum++;
        UnityWebRequest www = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPUT);
        www.method = "PATCH";
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
                _postCoroutine = StartCoroutine(PatchRequest(url, json, response, true, _requestNum));
                yield break;
            }
            else
            {
                Debug.LogFormat("[PATCH / request count {0}] Fail to Send!", _requestNum);
            }
        }
        else
        {
            Debug.LogFormat("[PATCH / request count {0}] Successed to Send!", _requestNum);
            response?.Invoke(www.downloadHandler.text);
        }
        www.Dispose();
    }

    public IEnumerator PostRequest (string url, string json, Action<string> success_response = null, Action fail_response = null, bool isReRequest = false, int ReIndex = 0)
    {
        int _requestNum = 0 + ReIndex;

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

            if (_requestNum < _requestMaxNum) // 응답 자체가 안온 경우
            {
                www.Dispose();
                _postCoroutine = StartCoroutine(PostRequest(url, json, success_response, fail_response, true, _requestNum));
                yield break;
            }
            else
            {
                Debug.LogFormat("[POST / request count {0}] Fail to Send!", _requestNum);
                fail_response?.Invoke();
            }
        }
        else
        {
            Debug.LogFormat("[POST / request count {0}] Successed to Send!", _requestNum);
            success_response?.Invoke(www.downloadHandler.text);
        }
        www.Dispose();
    }

    public IEnumerator GetRequest<T> (string url, Action<T> response = null, bool isReRequest = false, int ReIndex = 0)
    {
        int _requestNum = 0 + ReIndex;

        _requestNum++;
        UnityWebRequest www = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
        DownloadHandlerBuffer dh = new DownloadHandlerBuffer();

        www.SetRequestHeader("Content-Type", "application/json");
        www.downloadHandler = dh;

        yield return www.SendWebRequest();

        if(www.result != UnityWebRequest.Result.Success)
        {
            CustomLogger.Log(www.error);
            CustomLogger.Log(www.downloadHandler.text);

            if(_requestNum < _requestMaxNum)
            {
                www.Dispose();
                _getCoroutine = StartCoroutine(GetRequest(url, response, true, _requestNum));
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

            if(typeof(T) == typeof(string))
            {
                result = www.downloadHandler.text;
            }
            else if(typeof(T) == typeof(Texture2D))
            {
                result = DownloadHandlerTexture.GetContent(www);
            }

            Debug.LogFormat("[Get / request count {0}] Successed to Send!", _requestNum);
            response?.Invoke((T)result);
        }
        www.Dispose();
    }

    public IEnumerator GetRequestGoogleLink<T>(string url, Action<T> response = null, bool isReRequest = false, bool isSequential = false, int? reRequestedIndex = null, int ReIndex = 0)
    {
        string originUrl = url;

        if (!isReRequest)
        {
            //_requestNum = 0;
            _requestCompleted.Add(false);
        }

        int _requestNum = 0 + ReIndex;
        string key = ExtractGoogleDownKey(url);
        url = _googleDownUrl + key;


        int requestIndex = _requestCompleted.Count - 1;
        if(reRequestedIndex != null)
        {
            requestIndex = (int)reRequestedIndex;
        }

        _requestNum++;
        UnityWebRequest www = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
        DownloadHandlerBuffer dh = new DownloadHandlerBuffer();

        www.SetRequestHeader("Content-Type", "application/json");
        www.downloadHandler = dh;

        yield return www.SendWebRequest();

        if(www.result != UnityWebRequest.Result.Success)
        {
            CustomLogger.Log($"RETRY NOW {_requestNum} : {www.error}");
            //CustomLogger.Log(www.downloadHandler.text);

            if(_requestNum < _requestMaxNum)
            {
                www.Dispose();
                if(!isSequential)
                {
                    _getCoroutine = StartCoroutine(GetRequestGoogleLink(originUrl, response, true));
                }
                else
                {
                    StartCoroutine(GetRequestGoogleLink(originUrl, response, true, true, requestIndex, _requestNum));
                }
                yield break;
            }
            else
            {
                Debug.Log($"[Get / request count {_requestNum}] FAIL! : {originUrl}");
            }
        }
        else
        {
            Debug.LogFormat("[Get / request count {0}] Successed to Send!", _requestNum);

            object result = null;
            string contentType = www.GetResponseHeader("Content-Type");
            if(contentType != null)
            {
                if(contentType.StartsWith("image/"))
                {

                    byte[] data = www.downloadHandler.data;
                    Texture2D texture = new Texture2D(0, 0, TextureFormat.ARGB32, false);
                    texture.LoadImage(data);

                    if (typeof(T) == typeof(Sprite))
                    {
                        Rect rect = new Rect(0, 0, texture.width, texture.height);
                        Vector2 pivot = new Vector2(0.5f, 0.5f);
                        Sprite sprite = Sprite.Create(texture, rect, pivot);

                        result = sprite;
                    }
                    else
                    {
                        result = texture;
                    }
                }
                else if(contentType.StartsWith("video/"))
                {
                    _downloadPath = Path.Combine(Application.streamingAssetsPath, $"Video/{key}.mp4");
                    if(!File.Exists(_downloadPath))
                    {
                        byte[] data = www.downloadHandler.data;
                        File.WriteAllBytes(_downloadPath, data);
                    }
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


            try
            {
                response?.Invoke((T)result);
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
                Debug.Log($"{url} data is nevigative Size");
                //throw;
            }

            //if(result != null)
            //{
            //    response?.Invoke((T)result);
            //}
            //else
            //{
            //    Debug.Log($"{url} data is nevigative Size");
            //}
        }
        www.Dispose();

        _requestCompleted[requestIndex] = true;

        if(_requestCompleted.TrueForAll(x => x))
        {
            CustomLogger.Log("All Google Download completed");
            GameManager.Instance.SetAllDownloaded(true);
            GameManager.Instance.ResetGame();
            GameManager.Instance.globalPage.CloseDownloadLoading();
        }
    }

    public void Patch(string url, string json, Action<string> response = null)
    {
        if (string.IsNullOrEmpty(url) || url.Length <= 3)
        {
            return;
        }

        if (_patchCoroutine != null)
        {
            StopCoroutine(_patchCoroutine);
            _patchCoroutine = null;
        }
        _patchCoroutine = StartCoroutine(PatchRequest(url, json, response));
    }

    public void Post (string url, string json, Action<string> response = null, Action fail_response=null)
    {
        if (string.IsNullOrEmpty(url) || url.Length <= 3)
        {
            return;
        }

        if (_postCoroutine != null)
        {
            StopCoroutine(_postCoroutine);
            _postCoroutine = null;
        }
        _postCoroutine = StartCoroutine(PostRequest(url, json, response, fail_response));
    }

    public void Get<T> (string url, Action<T> response = null, bool isGoogleDownload = false)
    {
        if (string.IsNullOrEmpty(url) || url.Length <= 3)
        {
            return;
        }

        if (_getCoroutine != null)
        {
            StopCoroutine(_getCoroutine);
            _getCoroutine = null;
        }

        if(isGoogleDownload)
        {
            _getCoroutine = StartCoroutine(GetRequestGoogleLink(url, response));
        }
        else
        {
            _getCoroutine = StartCoroutine(GetRequest(url, response));
        }
    }

    public void GetSequently<T> (string url, Action<T> response = null, bool isGoogleDownload = false)
    {
        if(string.IsNullOrEmpty(url) || url.Length <= 3)
        {
            return;
        }

        if(isGoogleDownload)
        {
            StartCoroutine(GetRequestGoogleLink(url, response, false, true));
        }
        else
        {
            //sequential 대응필요
            StartCoroutine(GetRequest(url, response));
        }
    }

    public void StopActiveCoroutine ()
    {
        if(_postCoroutine != null)
        {
            StopCoroutine(_postCoroutine);
            _postCoroutine = null;
        }
    }

    private string ExtractGoogleDownKey (string url)
    {
        try
        {
            string startPattern = "file/d/";
            string endPattern = "/view";

            var startindex = url.IndexOf(startPattern) + startPattern.Length;
            var endindex = url.IndexOf(endPattern);

            return url.Substring(startindex, endindex - startindex);
        }
        catch(Exception ex)
        {
            CustomLogger.LogError($"Ex) {ex}, FROM [{url}]");
            return string.Empty;
        }
    }
}