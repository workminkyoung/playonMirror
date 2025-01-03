using Newtonsoft.Json.Linq;
using ServiceData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Vivestudios.UI;

public class ProfileModule : SingletonBehaviour<ProfileModule>
{
    #region Process
    private Dictionary<PROFILE_TYPE, List<string>> _profileSampleImagesDic = new Dictionary<PROFILE_TYPE, List<string>>();
    private string _profileJson;
    private List<string> _profileReorderName;
    private List<string> _caricatureCodes;
    private Dictionary<int, ContentsDetailEntry> _caricatureDetail = null;

    private const string SOURCE_IMAGE_REPLACE_STRING = "base64_encoded_source_image";
    private const string TARGET_IMAGE_REPLACE_STRING = "base64_encoded_target_image";
    private const int CONVERT_IMAGE_NUM = 8;

    public List<string> ProfileReorderName => _profileReorderName;
    public Dictionary<int, ContentsDetailEntry> caricatureDetail => _caricatureDetail;

    public void GetProfileImages(Texture2D origin, Action<List<Texture2D>> OnEnd)
    {
        string targetEncodeTexture = Convert.ToBase64String(origin.EncodeToPNG());
        List<int> indexes = GetRandomIndexes(CONVERT_IMAGE_NUM);

        List<string> convertedJsons = new List<string>();

        for (int i = 0; i < indexes.Count; i++)
        {
            ProfileRequestData requestData = new ProfileRequestData();
            requestData.menu_code = UserDataManager.inst.selectedSubContentKey;
            requestData.encoded_source_image = targetEncodeTexture;
            requestData.image_index = indexes[i];

            convertedJsons.Add(JsonUtility.ToJson(requestData));

            //convertedJsons.Add(_profileJson.Replace(SOURCE_IMAGE_REPLACE_STRING, targetEncodeTexture).Replace(TARGET_IMAGE_REPLACE_STRING, _profileSampleImagesDic[type][i]));
        }

        StartCoroutine(PostInSequence(ApiCall.inst.profileAPI, convertedJsons, OnEnd));
    }
    public void GetProfileImages_tirtir(Texture2D origin, Action<List<Texture2D>> OnEnd)
    {
        string targetEncodeTexture = Convert.ToBase64String(origin.EncodeToPNG());
        List<int> indexes = GetRandomIndexes(CONVERT_IMAGE_NUM);

        List<string> convertedJsons = new List<string>();

        ProfileRequestData requestData = new ProfileRequestData();
        requestData.menu_code = UserDataManager.inst.selectedSubContentKey;
        requestData.encoded_source_image = targetEncodeTexture;
        requestData.image_index = indexes[0];
        convertedJsons.Add(JsonUtility.ToJson(requestData));

        StartCoroutine(PostInSequence(ApiCall.inst.profileAPI, convertedJsons, OnEnd));
    }

    public void GetWhatIfImages(Texture2D origin, Action<List<Texture2D>> OnEnd)
    {
        string targetEncodeTexture = Convert.ToBase64String(origin.EncodeToPNG());
        //List<int> indexes = new List<int>();
        //for (int i = 0; i < CONVERT_IMAGE_NUM; i++)
        //{
        //    indexes.Add(i);
        //}
        List<int> indexes = GetRandomIndexes(CONVERT_IMAGE_NUM);
        _profileReorderName = new List<string>();

        List<string> convertedJsons = new List<string>();

        for (int i = 0; i < indexes.Count; i++)
        {
            ProfileRequestData requestData = new ProfileRequestData();
            requestData.menu_code = UserDataManager.inst.selectedSubContentKey;
            requestData.encoded_source_image = targetEncodeTexture;
            requestData.image_index = indexes[i];

            convertedJsons.Add(JsonUtility.ToJson(requestData));
            //convertedJsons.Add(_profileJson.Replace(SOURCE_IMAGE_REPLACE_STRING, targetEncodeTexture).Replace(TARGET_IMAGE_REPLACE_STRING, _profileSampleImagesDic[type][indexes[i]]));
            _profileReorderName.Add(StringCacheManager.inst.ProfileWhatIfName[indexes[i]]);
        }

        StartCoroutine(PostInSequence(ApiCall.inst.profileAPI, convertedJsons, OnEnd));
    }

    public void GetCaricatureImages(Texture2D origin, Action<List<Texture2D>> OnEnd)
    {
        string targetEncodeTexture = Convert.ToBase64String(origin.EncodeToPNG());
        List<string> convertedJsons = new List<string>();

        if(_caricatureDetail == null)
        {
            _caricatureDetail = new Dictionary<int, ContentsDetailEntry>();
            foreach (var item in AdminManager.inst.ServiceData.ContentsDetail)
            {
                if(item.Value.Use.ToLower() == "true" && item.Value.Category == "CC")
                {
                    _caricatureDetail.Add(int.Parse(item.Value.Sequence), item.Value);
                }
            }
        }

        for (int i = 0; i < _caricatureDetail.Count; i++)
        {
            CaricatureRequestData requestData = new CaricatureRequestData();
            requestData.menu_code = _caricatureDetail[i+1].Property;
            requestData.encoded_source_image = targetEncodeTexture;
            requestData.gender_index = (int)UserDataManager.inst.selectedGender;
            convertedJsons.Add(JsonUtility.ToJson(requestData));
        }

        StartCoroutine(PostInSequence(ApiCall.inst.caricatureAPI, convertedJsons, OnEnd));
    }

    protected override void Init()
    {
        LoadImages();
        LoadJson();
    }

    private void LoadImages()
    {
        foreach (PROFILE_TYPE elem in Enum.GetValues(typeof(PROFILE_TYPE)))
        {
            string path = Path.Combine(Application.streamingAssetsPath, "Profile", elem.ToString());
            if (Directory.Exists(path))
            {
                _profileSampleImagesDic.Add(elem, new List<string>());

                foreach (string imagePath in Directory.GetFiles(path, "*.png"))
                {
                    _profileSampleImagesDic[elem].Add(Convert.ToBase64String(File.ReadAllBytes(imagePath)));
                }
            }
        }
    }

    private void LoadJson()
    {
        _profileJson = File.ReadAllText( Path.Combine(Application.streamingAssetsPath, "Profile", "Profile.json"));
    }


    private List<int> GetRandomIndexes(int length, int n)
    {
        List<int> result = new List<int>();
        int randomNum = 0;
        while (result.Count < n)
        {
            randomNum = UnityEngine.Random.Range(0, length);
            if (!result.Contains(randomNum))
            {
                result.Add(randomNum);
            }
        }

        return result;
    }

    private List<int> GetRandomIndexes(int length)
    {
        List<int> result = new List<int>();
        int randomNum = 0;
        while (result.Count < length)
        {
            randomNum = UnityEngine.Random.Range(0, length);
            if (!result.Contains(randomNum))
            {
                result.Add(randomNum);
            }
        }

        return result;
    }

    public void ParseCaricatureCodes()
    {
        
    }

    private IEnumerator PostInSequence(string url,  List<string> jsons, Action<List<Texture2D>> OnEnd)
    {
        List<Texture2D> resultTextures = new List<Texture2D>();
        int curRequestNum = 0;
        int maxRequestNum = 3;
        bool isRequestSuccessed = false;

        for (int i = 0; i < jsons.Count; i++)
        {
            //CustomLogger.Log(jsons[i]);

            using (UnityWebRequest www = new UnityWebRequest(url))
            {
                curRequestNum = 0; 
                isRequestSuccessed = false;

                www.method = "POST";
                DownloadHandlerBuffer dh = new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsons[i]);
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                www.downloadHandler = dh;

                while (curRequestNum < maxRequestNum)
                {
                    yield return www.SendWebRequest();

                    if (www.result != UnityWebRequest.Result.Success)
                    {
                        CustomLogger.Log(www.error);
                        CustomLogger.Log(www.downloadHandler.text);
                        curRequestNum++;
                    }
                    else
                    {
                        string response = (www.downloadHandler.text);
                        JObject root = JObject.Parse(response);
                        JToken image = root.GetValue("images");
                        byte[] bytes = Convert.FromBase64String(image.ToString());

                        Texture2D resultTexture = new Texture2D(0, 0);
                        resultTexture.LoadImage(bytes);
                        resultTextures.Add(resultTexture);

                        CustomLogger.Log($"get image [{i}]");
                        isRequestSuccessed = true;
                        break;
                    }
                }

                if (!isRequestSuccessed)
                {
                    CustomLogger.Log($"[PostInSequence / request count {curRequestNum}] Fail to Send!");
                    GameManager.inst.SetDiffusionState(false);
                    break;
                }
            }
        }

        OnEnd?.Invoke(resultTextures);
    }
    #endregion
}
