using FrameData;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System;
using static UnityEditor.Progress;
using System.Security.Policy;
using ShootingScreenData;

public class AdminManager : SingletonBehaviour<AdminManager>
{
    [SerializeField]
    private ConfigDefaultData _configDefaultData;
    [SerializeField]
    private BubbleData.BubbleData _bubbleData;
    [SerializeField]
    private FilterData.FilterData _filterData;
    [SerializeField]
    private ServiceData.ServiceData _serviceData;
    [SerializeField]
    private BasicData.BasicSetting _basicSetting;
    [SerializeField]
    private ChromakeyFrameData.ChromakeyFrame _chromakeyFrame;
    [SerializeField]
    private ShootingScreenData.ShootScreenDic _shootScreen;
    [SerializeField]
    private FrameData.FrameEntryDic _frameData;

    [SerializeField]
    private LANGUAGE_TYPE _language = LANGUAGE_TYPE.KOR;
    private string _configDefaultAPI = "http://api.playon-vive.com/config/default/latest";

    public ConfigDefaultData ConfigDefaultData => _configDefaultData;
    public BubbleData.BubbleData BubbleData => _bubbleData;
    public FilterData.FilterData FilterData => _filterData;
    public ServiceData.ServiceData ServiceData => _serviceData;
    public BasicData.BasicSetting BasicSetting => _basicSetting;
    public ChromakeyFrameData.ChromakeyFrame ChromakeyFrame => _chromakeyFrame;
    public ShootingScreenData.ShootScreenDic ShootScreen => _shootScreen;
    public FrameData.FrameEntryDic FrameData => _frameData;
    public LANGUAGE_TYPE Language => _language;

    protected override void Init()
    {
        GameManager.OnGameResetAction += ResetAdminData;
        ApiCall.Instance.Get<string>(_configDefaultAPI, GetResponse);
    }

    public void ResetAdminData()
    {

    }

    public void GetResponse(string result)
    {
        GameManager.Instance.globalPage.OpenDownloadLoading();
        Debug.Log(result);

        _configDefaultData = JsonConvert.DeserializeObject<ConfigDefaultData>(result);
        Debug.Log(_configDefaultData.id);

        SetBubbleData();
        SetFilterData();
        SetServiceData();
        SetBasicData();
        SetChromakeyFrameData();
        SetShootScreenData();
        //SetFrameData();
    }

    private void SetBubbleData()
    {
        string result = _configDefaultData.config_default_set.result.BubbleData.ToString();
        _bubbleData = JsonConvert.DeserializeObject<BubbleData.BubbleData>(result);
    }

    private void SetFilterData()
    {
        string result = _configDefaultData.config_default_set.result.FilterData.ToString();
        _filterData = JsonConvert.DeserializeObject<FilterData.FilterData>(result);
    }

    private void SetServiceData()
    {
        string result = _configDefaultData.config_default_set.result.ServiceData.ToString();
        _serviceData = JsonConvert.DeserializeObject<ServiceData.ServiceData>(result);
        DownloadServiceData();
    }

    private void SetBasicData()
    {
        string result = _configDefaultData.config_default_set.result.BasicSetting.ToString();
        _basicSetting = JsonConvert.DeserializeObject<BasicData.BasicSetting>(result);
        DownloadBasicData();
    }

    private void SetChromakeyFrameData()
    {
        string result = _configDefaultData.config_default_set.result.ChromakeyFrame.ToString();
        _chromakeyFrame = JsonConvert.DeserializeObject<ChromakeyFrameData.ChromakeyFrame>(result);
    }

    private void SetShootScreenData()
    {
        Dictionary<string, object> shootScreenPair = new Dictionary<string, object>();
        string result = _configDefaultData.config_default_set.result.ShootingScreen.ToString();
        shootScreenPair = JsonConvert.DeserializeObject<Dictionary<string, object>>(result);

        _shootScreen = new ShootingScreenData.ShootScreenDic();

        foreach (var item in shootScreenPair)
        {
            Dictionary<string, string> shootScreenEntryDic = new Dictionary<string, string>();
            shootScreenEntryDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(item.Value.ToString());

            ShootingScreenData.ShootScreenEntry shootScreen = new ShootingScreenData.ShootScreenEntry();
            shootScreen.url = new ShootingScreenData.ShootScreenEntryDic();
            shootScreen.ratio = new List<string>();
            shootScreen.korean = new List<string>();
            shootScreen.chinese = new List<string>();
            shootScreen.english = new List<string>();
            foreach (var entry in shootScreenEntryDic)
            {
                if (entry.Key.Contains("Key"))
                {
                    shootScreen.Key = entry.Value.ToString();
                }
                else if (entry.Key.Contains("url"))
                {
                    shootScreen.url.Add(entry.Key, entry.Value);
                    if(shootScreen.url_orderdKey == null)
                    {
                        shootScreen.url_orderdKey = new List<string>();
                    }
                    shootScreen.url_orderdKey.Add(entry.Key);
                }
                else if (entry.Key.Contains("ratio"))
                {
                    shootScreen.ratio.Add(entry.Value);
                }
                else if (entry.Key.Contains("Korean"))
                {
                    shootScreen.korean.Add(entry.Value);
                }
                else if (entry.Key.Contains("Chinese"))
                {
                    shootScreen.chinese.Add(entry.Value);
                }
                else if (entry.Key.Contains("English"))
                {
                    shootScreen.english.Add(entry.Value);
                }
                else if (entry.Key.Contains("ConversionTime"))
                {
                    shootScreen.ConversionTime = entry.Value.ToString();
                }
                else if (entry.Key.Contains("ConversionVideo"))
                {
                    shootScreen.ConversionVideo = entry.Value.ToString();
                }
            }

            _shootScreen.Add(shootScreen.Key, shootScreen);
        }

        DownloadShootData();
    }

    private void SetFrameData()
    {
        string result = _configDefaultData.config_default_set.result.FrameData.ToString();
        _frameData = JsonConvert.DeserializeObject<FrameData.FrameEntryDic>(result);
        
        foreach (var entry in _frameData)
        {
            if (!string.IsNullOrEmpty(entry.Value.data))
            {
                SplitFrameDefinition(entry.Value);

            }
        }
        Debug.Log("e");
    }

    private void SplitFrameDefinition(FrameEntry entry)
    {
        //List<FrameDefinitionEntry> listEntry = new List<FrameDefinitionEntry>();
        //FrameDefinitionEntryDic 
        // 정규 표현식 패턴
        string pattern = @"\{.*?\}";
        entry.FrameDefinitions = new FrameDefinitionEntryDic();

        // 정규 표현식으로 일치하는 부분을 찾습니다.
        MatchCollection matches = Regex.Matches(entry.data, pattern);

        // 찾은 모든 매치를 출력
        foreach (Match match in matches)
        {
            FrameDefinitionEntry definition = JsonConvert.DeserializeObject<FrameDefinitionEntry>(match.Value);
            definition.picRects = new List<FrameRectTransform>();

            if (!string.IsNullOrEmpty(definition.PicConvert1))
            {
                definition.picRects.Add(ParseRectData(definition.PicConvert1));
            }
            if (!string.IsNullOrEmpty(definition.PicConvert2))
            {
                definition.picRects.Add(ParseRectData(definition.PicConvert2));
            }
            if (!string.IsNullOrEmpty(definition.PicConvert3))
            {
                definition.picRects.Add(ParseRectData(definition.PicConvert3));
            }
            if (!string.IsNullOrEmpty(definition.PicConvert4))
            {
                definition.picRects.Add(ParseRectData(definition.PicConvert4));
            }
            if (!string.IsNullOrEmpty(definition.PicConvert5))
            {
                definition.picRects.Add(ParseRectData(definition.PicConvert5));
            }
            if (!string.IsNullOrEmpty(definition.PicConvert6))
            {
                definition.picRects.Add(ParseRectData(definition.PicConvert6));
            }
            if (!string.IsNullOrEmpty(definition.PicConvert7))
            {
                definition.picRects.Add(ParseRectData(definition.PicConvert7));
            }
            if (!string.IsNullOrEmpty(definition.PicConvert8))
            {
                definition.picRects.Add(ParseRectData(definition.PicConvert8));
            }


            if (!string.IsNullOrEmpty(definition.DateRect))
            {
                definition.dateRect = ParseRectData(definition.DateRect);
            }
            if (!string.IsNullOrEmpty(definition.QRRect))
            {
                definition.qrRect = ParseRectData(definition.QRRect);
            }

            entry.FrameDefinitions[Tuple.Create(definition.Service, definition.ColorCode)] = definition;

            //Find Thumbnail tlqkf
            if (entry.ThumbnailUnselect == null)
            {
                if (!string.IsNullOrEmpty(definition.Thumbnaillink))
                {
                    ApiCall.Instance.GetSequently<Sprite>
                        (definition.Thumbnaillink, (texture) => { entry.ThumbnailUnselect = texture; }, true);
                }
            }
            if (entry.ThumbnailSelect == null)
            {
                if (!string.IsNullOrEmpty(definition.ThumbnailSliced))
                {
                    ApiCall.Instance.GetSequently<Sprite>
                        (definition.ThumbnailSliced, (texture) => { entry.ThumbnailSelect = texture; }, true);
                }
            }
        }
    }

    public void DownloadServiceData()
    {
        foreach (var item in _serviceData.Contents)
        {
            if( !string.IsNullOrEmpty(item.Value.Key))
            {
                switch (item.Value.Key)
                {
                    case "BT":
                        item.Value.ContentType = CONTENT_TYPE.AI_BEAUTY;
                        break;
                    case "CA":
                        item.Value.ContentType = CONTENT_TYPE.AI_CARTOON;
                        break;
                    case "PR":
                        item.Value.ContentType = CONTENT_TYPE.AI_PROFILE;
                        break;
                    case "TM":
                        item.Value.ContentType = CONTENT_TYPE.AI_TIME_MACHINE;
                        break;
                    case "WF":
                        item.Value.ContentType = CONTENT_TYPE.WHAT_IF;
                        break;
                    default:
                        break;
                }
            }

            if(!string.IsNullOrEmpty(item.Value.ImageThumbnail))
            {
                ApiCall.Instance.GetSequently<Sprite>
                    (item.Value.ImageThumbnail, (texture) => {item.Value.ImageThumbnail_data = texture;}, true);
            }

            if (!string.IsNullOrEmpty(item.Value.VideoThumbnail))
            {
                ApiCall.Instance.GetSequently<string>
                    (item.Value.VideoThumbnail, (path) => { item.Value.VideoThumbnail_path = path; }, true);
            }

            if (!string.IsNullOrEmpty(item.Value.GuideImage))
            {
                ApiCall.Instance.GetSequently<Sprite>
                    (item.Value.GuideImage, (texture) => { item.Value.GuideImage_data = texture; }, true);
            }

            if (!string.IsNullOrEmpty(item.Value.PopupGuideImage))
            {
                ApiCall.Instance.GetSequently<Sprite>
                    (item.Value.PopupGuideImage, (texture) => { item.Value.PopupGuideImage_data = texture; }, true);
            }

            if (!string.IsNullOrEmpty(item.Value.BGGuideImage))
            {
                ApiCall.Instance.GetSequently<Sprite>
                    (item.Value.BGGuideImage, (texture) => { item.Value.BGGuideImage_data = texture; }, true);
            }

            if (!string.IsNullOrEmpty(item.Value.ShootGuideImage))
            {
                ApiCall.Instance.GetSequently<Sprite>
                    (item.Value.ShootGuideImage, (texture) => { item.Value.ShootGuideImage_data = texture; }, true);
            }
        }

        foreach (var item in _serviceData.ContentsDetail)
        {
            if (!string.IsNullOrEmpty(item.Value.Thumbnail))
            {
                ApiCall.Instance.GetSequently<Sprite>
                    (item.Value.Thumbnail, (texture) => { item.Value.Thumbnail_data = texture; }, true);
            }

            if (!string.IsNullOrEmpty(item.Value.Gender))
            {
                switch (item.Value.Gender.ToLower())
                {
                    case "male" :
                        item.Value.Gender_type = GENDER_TYPE.MALE;
                        break;
                    case "female" :
                        item.Value.Gender_type = GENDER_TYPE.FEMALE;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public void DownloadBasicData()
    {
        if(!string.IsNullOrEmpty(_basicSetting.Config.BGImage))
        {
            ApiCall.Instance.GetSequently<Sprite>
                (_basicSetting.Config.BGImage, (texture) => { _basicSetting.Config.BGImage_data = texture; }, true);
        }

        if (!string.IsNullOrEmpty(_basicSetting.Config.EndImage))
        {
            ApiCall.Instance.GetSequently<Sprite>
                (_basicSetting.Config.EndImage, (texture) => { _basicSetting.Config.EndImage_data = texture; }, true);
        }

        if (!string.IsNullOrEmpty(_basicSetting.Config.StartImage))
        {
            ApiCall.Instance.GetSequently<Sprite>
                (_basicSetting.Config.StartImage, (texture) => { _basicSetting.Config.StartImage_data = texture; }, true);
        }

        if (!string.IsNullOrEmpty(_basicSetting.Config.PrintErrorImage))
        {
            ApiCall.Instance.GetSequently<Sprite>
                (_basicSetting.Config.PrintErrorImage, (texture) => { _basicSetting.Config.PrintErrorImage_data = texture; }, true);
        }

        if (!string.IsNullOrEmpty(_basicSetting.Config.StartMediaVideo))
        {
            ApiCall.Instance.GetSequently<string>
                (_basicSetting.Config.StartMediaVideo, (path) => { _basicSetting.Config.StartMediaVideo_path = path; }, true);
        }

        if (!string.IsNullOrEmpty(_basicSetting.Config.PersonalPolicyImage))
        {
            ApiCall.Instance.GetSequently<Sprite>
                (_basicSetting.Config.PersonalPolicyImage, (texture) => { _basicSetting.Config.PersonalPolicyImage_data = texture; }, true);
        }

        if (!string.IsNullOrEmpty(_basicSetting.Config.ServiceTermImage))
        {
            ApiCall.Instance.GetSequently<Sprite>
                (_basicSetting.Config.ServiceTermImage, (texture) => { _basicSetting.Config.ServiceTermImage_data = texture; }, true);
        }

        if (!string.IsNullOrEmpty(_basicSetting.Config.PaymentTermImage))
        {
            ApiCall.Instance.GetSequently<Sprite>
                (_basicSetting.Config.PaymentTermImage, (texture) => { _basicSetting.Config.PaymentTermImage_data = texture; }, true);
        }

        if (!string.IsNullOrEmpty(_basicSetting.Config.MarketingTermImage))
        {
            ApiCall.Instance.GetSequently<Sprite>
                (_basicSetting.Config.MarketingTermImage, (texture) => { _basicSetting.Config.MarketingTermImage_data = texture; }, true);
        }

        if (!string.IsNullOrEmpty(_basicSetting.Config.ServieErrorImage))
        {
            ApiCall.Instance.GetSequently<Sprite>
                (_basicSetting.Config.ServieErrorImage, (texture) => { _basicSetting.Config.ServieErrorImage_data = texture; }, true);
        }
    }

    public void DownloadFrameData()
    {

    }

    public void DownloadShootData()
    {
        foreach (var item in _shootScreen)
        {
            foreach (var urlItem in item.Value.url)
            {
                if (!string.IsNullOrEmpty(urlItem.Value))
                {
                    if(item.Value.url_datas == null)
                    {
                        item.Value.url_datas = new URLDataDic();
                    }
                    ApiCall.Instance.GetSequently<Sprite>
                        (urlItem.Value, (texture) => { item.Value.url_datas[urlItem.Key] = texture; }, true);
                }
            }

            if (!string.IsNullOrEmpty(item.Value.ConversionImage))
            {
                ApiCall.Instance.GetSequently<Texture2D>
                    (item.Value.ConversionImage, (texture) => { item.Value.ConversionImage_data = texture; }, true);
            }

            if (!string.IsNullOrEmpty(item.Value.ConversionVideo))
            {
                ApiCall.Instance.GetSequently<string>
                    (item.Value.ConversionVideo, (path) => { item.Value.ConversionVideo_path = path; }, true);
            }
        }
    }

    FrameRectTransform ParseRectData(string data)
    {
        FrameRectTransform rectData = new FrameRectTransform();

        // Extract Position
        var posMatch = Regex.Match(data, @"Pos\.X:(?<x>[\-0-9]+)\tPos\.Y:(?<y>[\-0-9]+)");
        if (posMatch.Success)
        {
            rectData.anchoredPosition = new Vector2(
                float.Parse(posMatch.Groups["x"].Value),
                float.Parse(posMatch.Groups["y"].Value)
            );
        }

        // Extract Size (Width and Height)
        var sizeMatch = Regex.Match(data, @"Width:(?<width>[\-0-9]+)\tHeight:(?<height>[\-0-9]+)");
        if (sizeMatch.Success)
        {
            rectData.sizeDelta = new Vector2(
                float.Parse(sizeMatch.Groups["width"].Value),
                float.Parse(sizeMatch.Groups["height"].Value)
            );
        }

        // Extract Anchor Min
        var anchorMinMatch = Regex.Match(data, @"Min:\t\[X:(?<minX>[\-0-9\.]+)\tY:(?<minY>[\-0-9\.]+)\]");
        if (anchorMinMatch.Success)
        {
            rectData.anchorMin = new Vector2(
                float.Parse(anchorMinMatch.Groups["minX"].Value),
                float.Parse(anchorMinMatch.Groups["minY"].Value)
            );
        }

        // Extract Anchor Max
        var anchorMaxMatch = Regex.Match(data, @"Max:\t\[X:(?<maxX>[\-0-9\.]+)\tY:(?<maxY>[\-0-9\.]+)\]");
        if (anchorMaxMatch.Success)
        {
            rectData.anchorMax = new Vector2(
                float.Parse(anchorMaxMatch.Groups["maxX"].Value),
                float.Parse(anchorMaxMatch.Groups["maxY"].Value)
            );
        }

        // Extract Pivot
        var pivotMatch = Regex.Match(data, @"Pivot:\t\[X:(?<pivotX>[\-0-9\.]+)\tY:(?<pivotY>[\-0-9\.]+)\]");
        if (pivotMatch.Success)
        {
            rectData.pivot = new Vector2(
                float.Parse(pivotMatch.Groups["pivotX"].Value),
                float.Parse(pivotMatch.Groups["pivotY"].Value)
            );
        }

        // Extract Rotation
        var rotationMatch = Regex.Match(data, @"Rotation:\t\[X:(?<rotX>[\-0-9\.]+)\tY:(?<rotY>[\-0-9\.]+)\tZ:(?<rotZ>[\-0-9\.]+)\]");
        if (rotationMatch.Success)
        {
            rectData.rotation = new Vector3(
                float.Parse(rotationMatch.Groups["rotX"].Value),
                float.Parse(rotationMatch.Groups["rotY"].Value),
                float.Parse(rotationMatch.Groups["rotZ"].Value)
                );
        }

        return rectData;
    }
}
