using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Vivestudios.UI;

public partial class ApiCall : SingletonBehaviour<ApiCall>
{
    private string _sourceEncodeText;// reference
    private string _targetEncodeText;// photo
    public Action<Texture2D> SendResult;
    public Action OnEndRequest;

    public List<Texture2D> _requestTextures = new List<Texture2D>();
    public int _requestCountMax = 0;
    public int _curRequestCount = 0;
    bool _requestDone = false;
    CARTOON_TYPE _cartoonType;
    Coroutine _requestCoroutine = null;

    const int reSizeWidth = 768;
    const int reSizeHeight = 576;

    //TODO : RESET
    public void ResetRequest(int photoMax)
    {
        _requestTextures.Clear();
        _requestTextures = new List<Texture2D>();
        _requestCountMax = photoMax;
        _curRequestCount = 0;
    }

    public void InRequestList(Texture2D texture)
    {
        _requestTextures.Add(texture);
    }

    public void StartCheckRequest()
    {
        if(_requestCoroutine != null)
        {
            StopCoroutine(_requestCoroutine);
            _requestCoroutine = null;
        }
        _requestCoroutine = StartCoroutine(CheckRequestDone());
    }

    public void StopCheckRequest()
    {
        if (_requestCoroutine != null)
        {
            StopCoroutine(_requestCoroutine);
            _requestCoroutine = null;
        }
    }

    IEnumerator CheckRequestDone()
    {
        while(_curRequestCount < _requestCountMax)
        {
            //wait until photo is exist
            yield return new WaitUntil(() => _requestTextures.Count - 1 >= _curRequestCount);

            byte[] targetBytes = _requestTextures[_curRequestCount].EncodeToPNG();
            _targetEncodeText = Convert.ToBase64String(targetBytes);
            _requestDone = false;

            if (UserDataManager.Instance.selectedContent == CONTENT_TYPE.AI_CARTOON)
            {
                APICartoonRequest();
            }

            yield return new WaitUntil(() => _requestDone);
        }
    }

    public void APICartoonRequest()
    {
        CartoonRequestData data = new CartoonRequestData();

        data.menu_code = UserDataManager.Instance.selectedContentCode;
        data.encoded_source_image = _targetEncodeText;

        string json = JsonUtility.ToJson(data);
        Post(_cartoonAPI, json, GetResponse);
    } 

    void GetResponse(string result)
    {
        APIResponse response = new APIResponse();
        response = JsonUtility.FromJson<APIResponse>(result);
        byte[] bytes = Convert.FromBase64String(response.images);

        Texture2D cartoonTex = new Texture2D(0, 0);
        cartoonTex.LoadImage(bytes);

        if(Debug.isDebugBuild)
        {
            //사진저장
            StorageManager.Instance.SavePicture("resultimg", cartoonTex);
        }

        SendResult(cartoonTex);
        OnEndRequest();
    }
}

