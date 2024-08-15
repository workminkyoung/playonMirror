using FrameData;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using Unity.VectorGraphics;
using UnityEngine;
using System;
using ShootingScreenData;
using UnityEditor.Build;
using UnityEngine.Playables;
using UnityEditor.UIElements;
using UnityEngine.Experimental.Rendering;
using static UnityEditor.Progress;

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
    private FrameData.FrameData _frameData;

    [SerializeField]
    private LANGUAGE_TYPE _language = LANGUAGE_TYPE.KOR;
    private string _configDefaultAPI = "http://api.playon-vive.com/machine-admin/machine/latest?uuid=";

    public ConfigDefaultData ConfigDefaultData => _configDefaultData;
    public BubbleData.BubbleData BubbleData => _bubbleData;
    public FilterData.FilterData FilterData => _filterData;
    public ServiceData.ServiceData ServiceData => _serviceData;
    public BasicData.BasicSetting BasicSetting => _basicSetting;
    public ChromakeyFrameData.ChromakeyFrame ChromakeyFrame => _chromakeyFrame;
    public ShootingScreenData.ShootScreenDic ShootScreen => _shootScreen;
    public FrameData.FrameData FrameData => _frameData;
    public LANGUAGE_TYPE Language => _language;

    protected override void Init ()
    {
        GameManager.OnGameResetAction += ResetAdminData;

        string uuid = SystemInfo.deviceUniqueIdentifier;
#if UNITY_EDITOR
        uuid = "temp_kway";
#endif
        _configDefaultAPI += uuid;
        ApiCall.Instance.Get<string>(_configDefaultAPI, GetResponse);
    }

    public void ResetAdminData ()
    {

    }

    public void GetResponse (string result)
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
        SetFrameData();
    }

    private void SetBubbleData ()
    {
        string result = _configDefaultData.machine_config.BubbleData.ToString();
        _bubbleData = JsonConvert.DeserializeObject<BubbleData.BubbleData>(result);
        DownloadBubbleData();
    }

    private void SetFilterData ()
    {
        string result = _configDefaultData.machine_config.FilterData.ToString();
        _filterData = JsonConvert.DeserializeObject<FilterData.FilterData>(result);
        DownloadFilterData();
    }

    private void SetServiceData ()
    {
        string result = _configDefaultData.machine_config.ServiceData.ToString();
        _serviceData = JsonConvert.DeserializeObject<ServiceData.ServiceData>(result);
        DownloadServiceData();
    }

    private void SetBasicData ()
    {
        string result = _configDefaultData.machine_config.BasicSetting.ToString();
        _basicSetting = JsonConvert.DeserializeObject<BasicData.BasicSetting>(result);
        DownloadBasicData();
    }

    private void SetChromakeyFrameData ()
    {
        string result = _configDefaultData.machine_config.ChromakeyFrame.ToString();
        _chromakeyFrame = JsonConvert.DeserializeObject<ChromakeyFrameData.ChromakeyFrame>(result);
        DownloadChromaKeyData();
    }

    private void SetShootScreenData ()
    {
        Dictionary<string, object> shootScreenPair = new Dictionary<string, object>();
        string result = _configDefaultData.machine_config.ShootingScreen.ToString();
        shootScreenPair = JsonConvert.DeserializeObject<Dictionary<string, object>>(result);

        _shootScreen = new ShootingScreenData.ShootScreenDic();

        foreach(var item in shootScreenPair)
        {
            Dictionary<string, string> shootScreenEntryDic = new Dictionary<string, string>();
            shootScreenEntryDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(item.Value.ToString());

            ShootingScreenData.ShootScreenEntry shootScreen = new ShootingScreenData.ShootScreenEntry();
            shootScreen.url = new ShootingScreenData.ShootScreenEntryDic();
            shootScreen.ratio = new List<string>();
            shootScreen.korean = new List<string>();
            shootScreen.chinese = new List<string>();
            shootScreen.english = new List<string>();
            foreach(var entry in shootScreenEntryDic)
            {
                if(entry.Key.Contains("Key"))
                {
                    shootScreen.Key = entry.Value.ToString();
                }
                else if(entry.Key.Contains("url"))
                {
                    shootScreen.url.Add(entry.Key, entry.Value);
                    if(shootScreen.url_orderdKey == null)
                    {
                        shootScreen.url_orderdKey = new List<string>();
                    }
                    shootScreen.url_orderdKey.Add(entry.Key);
                }
                else if(entry.Key.Contains("ratio"))
                {
                    shootScreen.ratio.Add(entry.Value);
                }
                else if(entry.Key.Contains("Korean"))
                {
                    shootScreen.korean.Add(entry.Value);
                }
                else if(entry.Key.Contains("Chinese"))
                {
                    shootScreen.chinese.Add(entry.Value);
                }
                else if(entry.Key.Contains("English"))
                {
                    shootScreen.english.Add(entry.Value);
                }
                else if(entry.Key.Contains("ConversionTime"))
                {
                    shootScreen.ConversionTime = entry.Value.ToString();
                }
                else if(entry.Key.Contains("ConversionVideo"))
                {
                    shootScreen.ConversionVideo = entry.Value.ToString();
                }
                else if (entry.Key.Contains("ConversionImage"))
                {
                    shootScreen.ConversionImage = entry.Value.ToString();
                }

            }

            _shootScreen.Add(shootScreen.Key, shootScreen);
        }

        DownloadShootData();
    }

    private void SetFrameData()
    {
        string result = _configDefaultData.machine_config.FrameData.ToString();
        _frameData = JsonConvert.DeserializeObject<FrameData.FrameData>(result);
        DownloadFrameData();
    }

    private void DownloadFrameData()
    {
        bool isColorCodeSorting = _frameData.Theme.Sorting.ToLower() == StringCacheManager.inst.SortingSpecified.ToLower() ? true : false;
        _frameData.DefinitionTuple = new FrameDefinitionEntryDic();
        _frameData.Theme.OrderedColorCode = new OrderedColorCodeEntryDic();

        foreach (var item in _frameData.Theme.ColorCode)
        {
            if (!item.Value.Use)
            {
                continue;
            }

            item.Value.key = item.Key;

            if (!string.IsNullOrEmpty(item.Value.Thumbnail))
            {
                ApiCall.Instance.GetSequently<Sprite>
                    (item.Value.Thumbnail, (texture) => { item.Value.Thumbnail_data = texture; }, true);
            }

            _frameData.Theme.OrderedColorCode[int.Parse(item.Value.Sequence.ToLower())] = item.Value;

        }

        UserDataManager.Instance.SetDefaultFrameColor(_frameData.Theme.OrderedColorCode[1].key);

        foreach (var item in _frameData.Definition)
        {
            _frameData.DefinitionTuple[item.Key] = new FrameDefinitionServiceColorDic();
            for (int i = 0; i < item.Value.Count; i++)
            {
                int index = i;
                //Debug.Log($"Frame Definition Key:{item.Key}, Count:{i}");
                item.Value[i].picRects = new List<FrameRectTransform>();
                item.Value[i].dateRects = new List<FrameRectTransform>();
                item.Value[i].qrRects = new List<FrameRectTransform>();
                item.Value[i].prices = new List<int>();
                item.Value[i].sellingPrices = new List<int>();

                item.Value[i].key = item.Key;
                switch (item.Key)
                {
                    case "FR1X1001":
                        item.Value[i].FrameType = FRAME_TYPE.FRAME_1;
                        break;
                    case "FR2X1001":
                        item.Value[i].FrameType = FRAME_TYPE.FRAME_2;
                        break;
                    case "FR2X2001":
                        item.Value[i].FrameType = FRAME_TYPE.FRAME_4;
                        break;
                    case "FR4X2001":
                        item.Value[i].FrameType = FRAME_TYPE.FRAME_8;
                        break;
                    default:
                        break;
                }

                // Parsing Data
                if (!string.IsNullOrEmpty(item.Value[i].PicCanvas1))
                {
                    item.Value[i].picRects.Add(ParseRectData(item.Value[i].PicCanvas1));
                }
                if (!string.IsNullOrEmpty(item.Value[i].PicCanvas2))
                {
                    item.Value[i].picRects.Add(ParseRectData(item.Value[i].PicCanvas2));
                }
                if (!string.IsNullOrEmpty(item.Value[i].PicCanvas3))
                {
                    item.Value[i].picRects.Add(ParseRectData(item.Value[i].PicCanvas3));
                }
                if (!string.IsNullOrEmpty(item.Value[i].PicCanvas4))
                {
                    item.Value[i].picRects.Add(ParseRectData(item.Value[i].PicCanvas4));
                }
                if (!string.IsNullOrEmpty(item.Value[i].PicCanvas5))
                {
                    item.Value[i].picRects.Add(ParseRectData(item.Value[i].PicCanvas5));
                }
                if (!string.IsNullOrEmpty(item.Value[i].PicCanvas6))
                {
                    item.Value[i].picRects.Add(ParseRectData(item.Value[i].PicCanvas6));
                }
                if (!string.IsNullOrEmpty(item.Value[i].PicCanvas7))
                {
                    item.Value[i].picRects.Add(ParseRectData(item.Value[i].PicCanvas7));
                }
                if (!string.IsNullOrEmpty(item.Value[i].PicCanvas8))
                {
                    item.Value[i].picRects.Add(ParseRectData(item.Value[i].PicCanvas8));
                }


                if (!string.IsNullOrEmpty(item.Value[i].DateRect_1))
                {
                    item.Value[i].dateRects.Add(ParseRectData(item.Value[i].DateRect_1));
                }
                if (!string.IsNullOrEmpty(item.Value[i].DateRect_2))
                {
                    item.Value[i].dateRects.Add(ParseRectData(item.Value[i].DateRect_2));
                }

                if (!string.IsNullOrEmpty(item.Value[i].QRRect_1))
                {
                    item.Value[i].qrRects.Add(ParseRectData(item.Value[i].QRRect_1));
                }
                if (!string.IsNullOrEmpty(item.Value[i].QRRect_2))
                {
                    item.Value[i].qrRects.Add(ParseRectData(item.Value[i].QRRect_2));
                }

                item.Value[i].prices.Add(item.Value[i].Price1);
                item.Value[i].prices.Add(item.Value[i].Price2);
                item.Value[i].prices.Add(item.Value[i].Price3);
                item.Value[i].prices.Add(item.Value[i].Price4);
                item.Value[i].prices.Add(item.Value[i].Price5);
                item.Value[i].prices.Add(item.Value[i].Price6);
                item.Value[i].prices.Add(item.Value[i].Price7);
                item.Value[i].prices.Add(item.Value[i].Price8);

                item.Value[i].sellingPrices.Add(item.Value[i].sellingPrice1);
                item.Value[i].sellingPrices.Add(item.Value[i].sellingPrice2);
                item.Value[i].sellingPrices.Add(item.Value[i].sellingPrice3);
                item.Value[i].sellingPrices.Add(item.Value[i].sellingPrice4);
                item.Value[i].sellingPrices.Add(item.Value[i].sellingPrice5);
                item.Value[i].sellingPrices.Add(item.Value[i].sellingPrice6);
                item.Value[i].sellingPrices.Add(item.Value[i].sellingPrice7);
                item.Value[i].sellingPrices.Add(item.Value[i].sellingPrice8);

                // Download Texture Data
                if (!string.IsNullOrEmpty(item.Value[i].BgImage))
                {
                    ApiCall.Instance.GetSequently<Sprite>
                        (item.Value[index].BgImage, (texture) => { item.Value[index].BgImage_data = texture; }, true);
                }
                if (!string.IsNullOrEmpty(item.Value[i].LayerImage))
                {
                    ApiCall.Instance.GetSequently<Sprite>
                        (item.Value[index].LayerImage, (texture) => { item.Value[index].LayerImage_data = texture; }, true);
                }
                if (!string.IsNullOrEmpty(item.Value[i].ShootingDim))
                {
                    ApiCall.Instance.GetSequently<Sprite>
                        (item.Value[index].ShootingDim, (texture) => { item.Value[index].ShootingDim_data = texture; }, true);
                }
                if (!string.IsNullOrEmpty(item.Value[i].ThumbnailSelect))
                {
                    ApiCall.Instance.GetSequently<Sprite>
                        (item.Value[index].ThumbnailSelect, (texture) => { item.Value[index].ThumbnailSelect_data = texture; }, true);
                }
                if (!string.IsNullOrEmpty(item.Value[i].ThumbnailUnselect))
                {
                    ApiCall.Instance.GetSequently<Sprite>
                        (item.Value[index].ThumbnailUnselect, (texture) => { item.Value[index].ThumbnailUnselect_data = texture; }, true);
                }

                Tuple<string, string> tupleKey = new Tuple<string, string>(item.Value[index].Service, item.Value[index].ColorCode);
                _frameData.DefinitionTuple[item.Key][tupleKey] = item.Value[index];
            }

            //entry.FrameDefinitions[Tuple.Create(definition.Service, definition.ColorCode)] = definition;

            //Find Thumbnail tlqkf
            //if (entry.ThumbnailUnselect == null)
            //{
            //    if (!string.IsNullOrEmpty(definition.Thumbnaillink))
            //    {
            //        ApiCall.Instance.GetSequently<Sprite>
            //            (definition.Thumbnaillink, (texture) => { entry.ThumbnailUnselect = texture; }, true);
            //    }
            //}
            //if (entry.ThumbnailSelect == null)
            //{
            //    if (!string.IsNullOrEmpty(definition.ThumbnailSliced))
            //    {
            //        ApiCall.Instance.GetSequently<Sprite>
            //            (definition.ThumbnailSliced, (texture) => { entry.ThumbnailSelect = texture; }, true);
            //    }
            //}


        }

        foreach (var item in _frameData.ServiceFrame.Code)
        {
            item.Value.SelectFrames = new List<string>();

            if (!string.IsNullOrEmpty(item.Value.SelectFrame1) || item.Value.SelectFrame1.Length > 0)
            {
                item.Value.SelectFrames.Add(item.Value.SelectFrame1);
            }
            if (!string.IsNullOrEmpty(item.Value.SelectFrame2) || item.Value.SelectFrame2.Length > 0)
            {
                item.Value.SelectFrames.Add(item.Value.SelectFrame2);
            }
            if (!string.IsNullOrEmpty(item.Value.SelectFrame3) || item.Value.SelectFrame3.Length > 0)
            {
                item.Value.SelectFrames.Add(item.Value.SelectFrame3);
            }
            if (!string.IsNullOrEmpty(item.Value.SelectFrame4) || item.Value.SelectFrame4.Length > 0)
            {
                item.Value.SelectFrames.Add(item.Value.SelectFrame4);
            }
            if (!string.IsNullOrEmpty(item.Value.SelectFrame5) || item.Value.SelectFrame5.Length > 0)
            {
                item.Value.SelectFrames.Add(item.Value.SelectFrame5);
            }
            if (!string.IsNullOrEmpty(item.Value.SelectFrame6) || item.Value.SelectFrame6.Length > 0)
            {
                item.Value.SelectFrames.Add(item.Value.SelectFrame6);
            }
            if (!string.IsNullOrEmpty(item.Value.SelectFrame7) || item.Value.SelectFrame7.Length > 0)
            {
                item.Value.SelectFrames.Add(item.Value.SelectFrame7);
            }
            if (!string.IsNullOrEmpty(item.Value.SelectFrame8) || item.Value.SelectFrame8.Length > 0)
            {
                item.Value.SelectFrames.Add(item.Value.SelectFrame8);
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

        if (!string.IsNullOrEmpty(_basicSetting.Config.StartMediaImage))
        {
            ApiCall.Instance.GetSequently<Sprite>
                (_basicSetting.Config.StartMediaImage, (texture) => { _basicSetting.Config.StartMediaImage_data = texture; }, true);
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

        if (!string.IsNullOrEmpty(_basicSetting.Config.PromotionImage))
        {
            ApiCall.Instance.GetSequently<Texture2D>
                (_basicSetting.Config.PromotionImage, (texture) => { _basicSetting.Config.PromotionImage_data = texture; }, true);
        }

        if (!string.IsNullOrEmpty(_basicSetting.Config.PromotionVideo))
        {
            ApiCall.Instance.GetSequently<string>
                (_basicSetting.Config.PromotionVideo, (path) => { _basicSetting.Config.PromotionVideo_path = path; }, true);
        }
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

    public void DownloadFilterData()
    {
        List<string> orderKey = new List<string>();
        _filterData.OrderedFilterTable = new FilterData.FilterData.OrderedFilterTableDic();

        foreach (var item in _filterData.FilterTable)
        {
            if (!string.IsNullOrEmpty(item.Value.File))
            {
                ApiCall.Instance.GetSequently<Texture2D>
                    (item.Value.File, (texture) => { item.Value.LutFile_data = texture; }, true);
            }

            if (!string.IsNullOrEmpty(item.Value.Thumbnail))
            {
                ApiCall.Instance.GetSequently<Sprite>
                    (item.Value.Thumbnail, (texure) => { item.Value.Thumbnail_data = texure; }, true);
            }

            if (bool.Parse(item.Value.Used.ToLower()))
            {
                _filterData.OrderedFilterTable[int.Parse(item.Value.Sequence)] = item.Value;
            }
        }
    }

    FrameRectTransform ParseRectData(string data)
    {
        FrameRectTransform rectData = new FrameRectTransform();

        // 정규식 패턴
        //string pattern = @"Pos\.X:(?<PosX>-?\d+\.?\d*)\s+Pos\.Y:(?<PosY>-?\d+\.?\d*)\s+Width:(?<Width>\d+\.?\d*)\s+Height:(?<Height>\d+\.?\d*)\s+Min:\s+\[X:(?<AnchorMinX>\d+\.?\d*)\s+Y:(?<AnchorMinY>\d+\.?\d*)\]\s+Max:\s+\[X:(?<AnchorMaxX>\d+\.?\d*)\s+Y:(?<AnchorMaxY>\d+\.?\d*)\]\s+Pivot:\s+\[X:(?<PivotX>\d+\.?\d*)\s+Y:(?<PivotY>\d+\.?\d*)\]\s+Rotation:\s+\[X:(?<RotationX>\d+\.?\d*)\s+Y:(?<RotationY>\d+\.?\d*)\s+Z:(?<RotationZ>\d+\.?\d*)\]";

        string pattern = @"Pos\.X:\s*(?<PosX>-?\d+\.?\d*)\s*Pos\.Y:\s*(?<PosY>-?\d+\.?\d*)\s*Width:\s*(?<Width>\d+\.?\d*)\s*Height:\s*(?<Height>\d+\.?\d*)\s*Min:\s*\[X:\s*(?<AnchorMinX>\d+\.?\d*)\s*Y:\s*(?<AnchorMinY>\d+\.?\d*)\]\s*Max:\s*\[X:\s*(?<AnchorMaxX>\d+\.?\d*)\s*Y:\s*(?<AnchorMaxY>\d+\.?\d*)\]\s*Pivot:\s*\[X:\s*(?<PivotX>\d+\.?\d*)\s*Y:\s*(?<PivotY>\d+\.?\d*)\]\s*Rotation:\s*\[X:\s*(?<RotationX>\d+\.?\d*)\s*Y:\s*(?<RotationY>\d+\.?\d*)\s*Z:\s*(?<RotationZ>\d+\.?\d*)\]";

        Regex regex = new Regex(pattern);
        Match match = regex.Match(data);

        // Extract Position
        if (match.Success)
        {
            rectData.anchoredPosition = new Vector2(
                float.Parse(match.Groups["PosX"].Value),
                float.Parse(match.Groups["PosY"].Value)
            );

            rectData.sizeDelta = new Vector2(
                float.Parse(match.Groups["Width"].Value),
                float.Parse(match.Groups["Height"].Value)
            );

            rectData.anchorMin = new Vector2(
                float.Parse(match.Groups["AnchorMinX"].Value),
                float.Parse(match.Groups["AnchorMinY"].Value)
            );

            rectData.anchorMax = new Vector2(
                float.Parse(match.Groups["AnchorMaxX"].Value),
                float.Parse(match.Groups["AnchorMaxY"].Value)
            );

            rectData.pivot = new Vector2(
                float.Parse(match.Groups["PivotX"].Value),
                float.Parse(match.Groups["PivotY"].Value)
            );

            rectData.rotation = new Vector3(
                float.Parse(match.Groups["RotationX"].Value),
                float.Parse(match.Groups["RotationY"].Value),
                float.Parse(match.Groups["RotationZ"].Value)
                );
        }

        //// Extract Size (Width and Height)
        //var sizeMatch = Regex.Match(data, @"Width:(?<width>[\-0-9]+)\tHeight:(?<height>[\-0-9]+)");
        //if (sizeMatch.Success)
        //{
        //    rectData.sizeDelta = new Vector2(
        //        float.Parse(sizeMatch.Groups["width"].Value),
        //        float.Parse(sizeMatch.Groups["height"].Value)
        //    );
        //}

        //// Extract Anchor Min
        //var anchorMinMatch = Regex.Match(data, @"Min:\t\[X:(?<minX>[\-0-9\.]+)\tY:(?<minY>[\-0-9\.]+)\]");
        //if (anchorMinMatch.Success)
        //{
        //    rectData.anchorMin = new Vector2(
        //        float.Parse(anchorMinMatch.Groups["minX"].Value),
        //        float.Parse(anchorMinMatch.Groups["minY"].Value)
        //    );
        //}

        //// Extract Anchor Max
        //var anchorMaxMatch = Regex.Match(data, @"Max:\t\[X:(?<maxX>[\-0-9\.]+)\tY:(?<maxY>[\-0-9\.]+)\]");
        //if (anchorMaxMatch.Success)
        //{
        //    rectData.anchorMax = new Vector2(
        //        float.Parse(anchorMaxMatch.Groups["maxX"].Value),
        //        float.Parse(anchorMaxMatch.Groups["maxY"].Value)
        //    );
        //}

        //// Extract Pivot
        //var pivotMatch = Regex.Match(data, @"Pivot:\t\[X:(?<pivotX>[\-0-9\.]+)\tY:(?<pivotY>[\-0-9\.]+)\]");
        //if (pivotMatch.Success)
        //{
        //    rectData.pivot = new Vector2(
        //        float.Parse(pivotMatch.Groups["pivotX"].Value),
        //        float.Parse(pivotMatch.Groups["pivotY"].Value)
        //    );
        //}

        //// Extract Rotation
        //var rotationMatch = Regex.Match(data, @"Rotation:\t\[X:(?<rotX>[\-0-9\.]+)\tY:(?<rotY>[\-0-9\.]+)\tZ:(?<rotZ>[\-0-9\.]+)\]");
        //if (rotationMatch.Success)
        //{
        //    rectData.rotation = new Vector3(
        //        float.Parse(rotationMatch.Groups["rotX"].Value),
        //        float.Parse(rotationMatch.Groups["rotY"].Value),
        //        float.Parse(rotationMatch.Groups["rotZ"].Value)
        //        );
        //}

        return rectData;
    }

    public void DownloadChromaKeyData()
    {
        foreach(var item in _chromakeyFrame.ChromakeyToneTable)
        {
            ApiCall.Instance.GetSequently<Texture2D>(item.Value.Thumbnail, (texture) =>
            {
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                item.Value.Thumbnail_data = sprite;
            }, true);
            ApiCall.Instance.GetSequently<Texture2D>(item.Value.Image1, (texture) => item.Value.Image_data.Add(texture), true);
            ApiCall.Instance.GetSequently<Texture2D>(item.Value.Image2, (texture) => item.Value.Image_data.Add(texture), true);
            ApiCall.Instance.GetSequently<Texture2D>(item.Value.Image3, (texture) => item.Value.Image_data.Add(texture), true);
            ApiCall.Instance.GetSequently<Texture2D>(item.Value.Image4, (texture) => item.Value.Image_data.Add(texture), true);
            ApiCall.Instance.GetSequently<Texture2D>(item.Value.Image5, (texture) => item.Value.Image_data.Add(texture), true);
            ApiCall.Instance.GetSequently<Texture2D>(item.Value.Image6, (texture) => item.Value.Image_data.Add(texture), true);
            ApiCall.Instance.GetSequently<Texture2D>(item.Value.Image7, (texture) => item.Value.Image_data.Add(texture), true);
            ApiCall.Instance.GetSequently<Texture2D>(item.Value.Image8, (texture) => item.Value.Image_data.Add(texture), true);
        }

        foreach(var item in _chromakeyFrame.ChromakeyFrameTable)
        {
            ApiCall.Instance.GetSequently<Texture2D>(item.Value.Thumbnail, (texture) =>
            {
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                item.Value.Thumbnail_data = sprite;
            }, true);
            ApiCall.Instance.GetSequently<Texture2D>(item.Value.Image1, (texture) => item.Value.Image_data.Add(texture), true);
            ApiCall.Instance.GetSequently<Texture2D>(item.Value.Image2, (texture) => item.Value.Image_data.Add(texture), true);
            ApiCall.Instance.GetSequently<Texture2D>(item.Value.Image3, (texture) => item.Value.Image_data.Add(texture), true);
            ApiCall.Instance.GetSequently<Texture2D>(item.Value.Image4, (texture) => item.Value.Image_data.Add(texture), true);
            ApiCall.Instance.GetSequently<Texture2D>(item.Value.Image5, (texture) => item.Value.Image_data.Add(texture), true);
            ApiCall.Instance.GetSequently<Texture2D>(item.Value.Image6, (texture) => item.Value.Image_data.Add(texture), true);
            ApiCall.Instance.GetSequently<Texture2D>(item.Value.Image7, (texture) => item.Value.Image_data.Add(texture), true);
            ApiCall.Instance.GetSequently<Texture2D>(item.Value.Image8, (texture) => item.Value.Image_data.Add(texture), true);
        }
    }

    public void DownloadBubbleData ()
    {
        foreach(var item in _bubbleData.BubbleTable)
        {
            if(string.IsNullOrEmpty(item.Value.Image))
            {
                continue;
            }

            if(item.Value.ImageType.ToLower() == "png")
            {
                ApiCall.Instance.GetSequently<Texture2D>(item.Value.Image, (texture) =>
                {
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    item.Value.Image_sprite = sprite;
                }, true);
            }
            else if(item.Value.ImageType.ToLower() == "svg")
            {
                ApiCall.Instance.GetSequently<byte[]>(item.Value.Image, (stream) =>
                {
                    string svgText = System.Text.Encoding.UTF8.GetString(stream);
                    var TessOptions = new VectorUtils.TessellationOptions()
                    {
                        StepDistance = 100.0f,
                        MaxCordDeviation = 0.5f,
                        MaxTanAngleDeviation = 0.1f,
                        SamplingStepSize = 0.01f
                    };
                    var SceneInfo = SVGParser.ImportSVG(new StringReader(svgText));
                    var TessGeo = VectorUtils.TessellateScene(SceneInfo.Scene, TessOptions);
                    var spriteSvg = VectorUtils.BuildSprite(TessGeo, 10.0f, VectorUtils.Alignment.Center, Vector2.zero, 128, true);
                    item.Value.Image_sprite = spriteSvg;

                }, true);
            }
        }
    }
}
