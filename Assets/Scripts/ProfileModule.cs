using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Vivestudios.UI;

public class ProfileModule : SingletonBehaviour<ProfileModule>
{
    public void GetProfileImages(Texture2D origin, PROFILE_TYPE type, Action<List<Texture2D>> OnEnd)
    {
        string targetEncodeTexture = Convert.ToBase64String(origin.EncodeToPNG());
        List<int> indexes = GetRandomIndexes(_profileSampleImagesDic[type].Count, CONVERT_IMAGE_NUM);

        List<string> convertedJsons = new List<string>();

        for (int i = 0; i < indexes.Count; i++)
        {
            convertedJsons.Add(_profileJson.Replace(SOURCE_IMAGE_REPLACE_STRING, targetEncodeTexture).Replace(TARGET_IMAGE_REPLACE_STRING, _profileSampleImagesDic[type][i]));
        }

        StartCoroutine(PostProfile(convertedJsons, OnEnd));
    }

    public void GetWhatIfImages(Texture2D origin, PROFILE_TYPE type, Action<List<Texture2D>> OnEnd)
    {
        string targetEncodeTexture = Convert.ToBase64String(origin.EncodeToPNG());
        List<int> indexes = GetRandomIndexes(_profileSampleImagesDic[type].Count, CONVERT_IMAGE_NUM);
        _profileReorderName = new List<string>();

        List<string> convertedJsons = new List<string>();

        for (int i = 0; i < indexes.Count; i++)
        {
            convertedJsons.Add(_profileJson.Replace(SOURCE_IMAGE_REPLACE_STRING, targetEncodeTexture).Replace(TARGET_IMAGE_REPLACE_STRING, _profileSampleImagesDic[type][indexes[i]]));
            _profileReorderName.Add(StringCacheManager.inst.ProfileWhatIfName[indexes[i]]);
        }

        StartCoroutine(PostProfile(convertedJsons, OnEnd));
    }

    #region Process
    private Dictionary<PROFILE_TYPE, List<string>> _profileSampleImagesDic = new Dictionary<PROFILE_TYPE, List<string>>();
    private string _profileJson;
    private List<string> _profileReorderName;

    private const string SOURCE_IMAGE_REPLACE_STRING = "base64_encoded_source_image";
    private const string TARGET_IMAGE_REPLACE_STRING = "base64_encoded_target_image";
    private const int CONVERT_IMAGE_NUM = 8;

    public List<string> ProfileReorderName => _profileReorderName;

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

    private IEnumerator PostProfile(List<string> jsons, Action<List<Texture2D>> OnEnd)
    {
        List<Texture2D> resultTextures = new List<Texture2D>();
        int curRequestNum = 0;
        int maxRequestNum = 3;
        bool isRequestSuccessed = false;

        for (int i = 0; i < jsons.Count; i++)
        {
            //Debug.Log(jsons[i]);

            using (UnityWebRequest www = new UnityWebRequest(ApiCall.inst.profileAPI))
            {
                curRequestNum = 0; 
                isRequestSuccessed = false;

                while (curRequestNum < maxRequestNum)
                {
                    www.method = "POST";
                    DownloadHandlerBuffer dh = new DownloadHandlerBuffer();

                    www.SetRequestHeader("Content-Type", "application/json");
                    byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsons[i]);

                    www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    www.downloadHandler = dh;

                    yield return www.SendWebRequest();

                    if (www.result != UnityWebRequest.Result.Success)
                    {
                        Debug.Log(www.error);
                        Debug.Log(www.downloadHandler.text);
                        curRequestNum++;
                    }
                    else
                    {
                        string response = (www.downloadHandler.text);
                        //Debug.Log(response);
                        JObject root = JObject.Parse(response);
                        //Debug.Log(root);
                        JToken image = root.GetValue("images");
                        //Debug.Log(image);
                        byte[] bytes = Convert.FromBase64String(image[0].ToString());

                        Texture2D resultTexture = new Texture2D(0, 0);
                        resultTexture.LoadImage(bytes);
                        resultTextures.Add(resultTexture);

                        Debug.Log($"get image [{i}]");
                        isRequestSuccessed = true;
                        break;
                    }
                }

                if (!isRequestSuccessed)
                {
                    Debug.LogFormat("[AIProfile / request count {0}] Fail to Send!", curRequestNum);
                    GameManager.inst.SetDiffusionState(false);
                    break;
                }
            }
        }

        OnEnd?.Invoke(resultTextures);
    }
    #endregion
}
