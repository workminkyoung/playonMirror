using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
                FindTagger();
            else if (UserDataManager.Instance.selectedContent == CONTENT_TYPE.AI_BEAUTY)
                CheckImg2ImgJson(string.Empty);

            yield return new WaitUntil(() => _requestDone);
        }
    }

    //set current model
    public void RequestModel(Cartoon cartoon)
    {
        ModelRequest request = new ModelRequest();
        request.sd_model_checkpoint = cartoon.model;
        string json = JsonUtility.ToJson(request);

        if(cartoon.reference != null)
        {
            byte[] sources = cartoon.reference.EncodeToPNG();
            _sourceEncodeText = Convert.ToBase64String(sources);
        }
        else
        {
            _sourceEncodeText = string.Empty;
        }
        _cartoonType = cartoon.type;

        SetModel(json);
    }

    //set current model
    public void RequestModel(string model)
    {
        ModelRequest request = new ModelRequest();
        request.sd_model_checkpoint = model;
        string json = JsonUtility.ToJson(request);

        _sourceEncodeText = string.Empty;

        SetModel(json);
    }

    //find image tagger and Run change img
    public void FindTagger()
    {
        TaggerRequest tag = new TaggerRequest();
        tag.image = _targetEncodeText;
        string json = JsonUtility.ToJson(tag);

        PostSelect(_url_tagger, json, (result) =>
        {
            string[] splitData = result.Split('\"');
            string captions = "";

            for (int i = 1; i < splitData.Length; i += 2)
            {
                if (splitData[i].Contains("caption") || splitData[i].Contains("realistic") || splitData[i].Contains("freckles"))
                    continue;

                if (i < splitData.Length - 2)
                    captions += splitData[i] + ",";
                else
                    captions += splitData[i];
            }

            CheckImg2ImgJson(captions);
        });
    }

    void CheckImg2ImgJson(string tagger)
    {
        string json = string.Empty;
        if(UserDataManager.Instance.selectedContent == CONTENT_TYPE.AI_CARTOON)
            json = cartoonJsons[(int)_cartoonType];
        else if(UserDataManager.Instance.selectedContent == CONTENT_TYPE.AI_BEAUTY)
            json = beautyJsons[0];

        json = json.Replace(TextData.replace_width, reSizeWidth.ToString());
        json = json.Replace(TextData.replace_height, reSizeHeight.ToString());

        if(json.Contains(TextData.replace_target) && _targetEncodeText.Length > 0)
            json = json.Replace(TextData.replace_target, _targetEncodeText);
        if(json.Contains(TextData.replace_source) && _sourceEncodeText.Length > 0)
            json = json.Replace(TextData.replace_source, _sourceEncodeText);
        if(json.Contains(TextData.replace_tagger) && tagger.Length > 0)
            json = json.Replace(TextData.replace_tagger, tagger);

        Img2Img(json, result => ShowResult(result));
    }

    void ShowResult(string result)
    {
        Img2ImgResponse response = new Img2ImgResponse();
        response = JsonUtility.FromJson<Img2ImgResponse>(result);
        byte[] bytes = Convert.FromBase64String(response.images[0]);

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
