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

    private string _url_setmodel;
    private string _url_img2img ;
    private string _url_tagger;
    private string _url_samPredict;
    private string _url_txt2img;
    private string _url_roop;

    private Coroutine _setModelCoroutine;
    private Coroutine _setImageCoroutine;
    private Coroutine _setTaggerCoroutine;

    private int _requestNum = 0;
    private const int _requestMaxNum = 3;

    public string url_img2img => _url_img2img;
    //15.164.164.115
    //이거 서버 바꿔서 테스트해야하지롱
    //서버 그냥 config로 빼두기

    #region const url

    #endregion

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

        if (ConfigData.config.localUrlCheck)
        {
            _url_setmodel = "http://127.0.0.1:7860/sdapi/v1/options";
            _url_img2img = "http://127.0.0.1:7860/sdapi/v1/img2img";
            _url_tagger = "http://127.0.0.1:7860/tagger/v1/interrogate";
            _url_samPredict = "http://127.0.0.1:7860/sam/sam-predict";
            _url_txt2img = "http://127.0.0.1:7860/sdapi/v1/txt2img";
            _url_roop = "http://127.0.0.1:7860/roop/image";
        }
        else
        {
            string url = ConfigData.config.serverUrl;

            _url_setmodel = url + "/sdapi/v1/options";
            _url_img2img = url + "/sdapi/v1/img2img";
            _url_tagger = url + "/tagger/v1/interrogate";
            _url_samPredict = url + "/sam/sam-predict";
            _url_txt2img = url + "/sdapi/v1/txt2img";
            _url_roop = url + "/roop/image";
        }
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
            CustomLogger.LogError("Folder does not exist: " + folderPath);
        }

        return jsonFiles;
    }

    public IEnumerator PostModel(string json, Action<string> response = null, Action endEvent = null, bool isReRequest = false)
    {
        if (!isReRequest)
        {
            _requestNum = 0;
        }

        _requestNum++;
        UnityWebRequest www = new UnityWebRequest(_url_setmodel, "POST");
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
                _setModelCoroutine = StartCoroutine(PostModel(json, response, endEvent, true));
                yield break;
            }
            else
            {
                CustomLogger.Log($"[SetModel / request count {_requestNum}] Fail to Send!");
                GameManager.inst.SetDiffusionState(false);
            }
        }
        else
        {
            CustomLogger.Log($"[SetModel / request count {_requestNum}] Successed to Send!");
            response?.Invoke(www.downloadHandler.text);
            endEvent?.Invoke();
        }
        www.Dispose();
    }

    public IEnumerator PostImg2Img(string json, Action<string> response = null, bool isReRequest = false)
    {
        if (!isReRequest)
        {
            _requestNum = 0;
        }
        
        _requestNum++;
        UnityWebRequest www = new UnityWebRequest(_url_img2img, UnityWebRequest.kHttpVerbPOST);
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
                _setImageCoroutine = StartCoroutine(PostImg2Img(json, response, true));
                yield break;
            }
            else
            {
                CustomLogger.Log($"[Img2Img / request count {_requestNum}] Fail to Send!");
                GameManager.inst.SetDiffusionState(false);
            }
        }
        else
        {
            CustomLogger.Log($"[Img2Img / request count {_requestNum}] Successed to Send!");
            response?.Invoke(www.downloadHandler.text);
        }
        www.Dispose();
    }

    public IEnumerator PostTagger(string url, string json, Action<string> response = null, bool isReRequest = false)
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
                _setTaggerCoroutine = StartCoroutine(PostTagger(url, json, response, true));
                yield break;
            }
            else
            {
                CustomLogger.Log($"[Tagger / request count {_requestNum}] Fail to Send!");
                GameManager.inst.SetDiffusionState(false);
            }
        }
        else
        {
            CustomLogger.Log($"[Tagger / request count {_requestNum}] Successed to Send!");
            response?.Invoke(www.downloadHandler.text);
        }
        www.Dispose();
    }

    public void SetModel(string json, Action<string> response = null, Action endEvent = null)
    {
        if(_setModelCoroutine != null)
        {
            StopCoroutine(_setModelCoroutine);
            _setModelCoroutine = null;
        }
        _setModelCoroutine = StartCoroutine(PostModel(json, response, endEvent));
    }

    public void Img2Img(string json, Action<string> response = null)
    {
        if(_setImageCoroutine != null)
        {
            StopCoroutine(_setImageCoroutine);
            _setImageCoroutine = null;
        }
        _setImageCoroutine = StartCoroutine(PostImg2Img(json, response));
    }

    public void PostSelect(string url, string json, Action<string> response = null)
    {
        if(_setTaggerCoroutine != null)
        {
            StopCoroutine (_setTaggerCoroutine);
            _setTaggerCoroutine = null;
        }
        _setTaggerCoroutine = StartCoroutine(PostTagger(url, json, response));
    }

    public void StopActiveCoroutine()
    {
        if (_setModelCoroutine != null)
        {
            StopCoroutine(_setModelCoroutine);
            _setModelCoroutine = null;
        }
        if (_setImageCoroutine != null)
        {
            StopCoroutine(_setImageCoroutine);
            _setImageCoroutine = null;
        }
        if (_setTaggerCoroutine != null)
        {
            StopCoroutine(_setTaggerCoroutine);
            _setTaggerCoroutine = null;
        }
    }
}