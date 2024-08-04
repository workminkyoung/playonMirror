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
    private Coroutine _postCoroutine;
    private Coroutine _getCoroutine;
    private int _requestNum = 0;
    private const int _requestMaxNum = 3;

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

    public IEnumerator GetRequest(string url, Action<string> response = null, bool isReRequest = false)
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
        if(_postCoroutine != null)
        {
            StopCoroutine(_postCoroutine);
            _postCoroutine = null;
        }
        _postCoroutine = StartCoroutine(PostRequest(url, json, response));
    }

    public void Get(string url, Action<string> response = null)
    {
        if (_getCoroutine != null)
        {
            StopCoroutine(_getCoroutine);
            _getCoroutine = null;
        }

        _getCoroutine = StartCoroutine(GetRequest(url, response));
    }

    public void StopActiveCoroutine()
    {
        if (_postCoroutine != null)
        {
            StopCoroutine(_postCoroutine);
            _postCoroutine = null;
        }
    }
}