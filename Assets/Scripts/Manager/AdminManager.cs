using FrameData;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using Unity.VectorGraphics;
using UnityEngine;
using System;
using ShootingScreenData;
using System.Linq;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

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

        _configDefaultAPI += LogDataManager.Instance.GetGuid;
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
        // Theme Data
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

        // Common
        _frameData.CommonTuple = new CommonTupleEntryDic();
        
        foreach (var item in _frameData.Common.Code)
        {
            for (int i = 0; i < item.Value.Count; i++)
            {
                int index = i;
                item.Value[i].Dim_datas = new List<Sprite>();

                item.Value[i].originPrice_datas = ParsePriceData(item.Value[i].originPrice);
                item.Value[i].discountPrice_datas = ParsePriceData(item.Value[i].discountPrice);

                if (!string.IsNullOrEmpty(item.Value[i].ThumbnailUnselect))
                {
                    ApiCall.Instance.GetSequently<Sprite>
                        (item.Value[i].ThumbnailUnselect, (sprite) => { item.Value[index].ThumbnailUnselect_data = sprite; }, true);
                }
                if (!string.IsNullOrEmpty(item.Value[i].ThumbnailSelect))
                {
                    ApiCall.Instance.GetSequently<Sprite>
                        (item.Value[i].ThumbnailSelect, (sprite) => { item.Value[index].ThumbnailSelect_data = sprite; }, true);
                }

                if (!string.IsNullOrEmpty(item.Value[i].Dim1))
                {
                    ApiCall.Instance.GetSequently<Sprite>
                        (item.Value[i].Dim1, (sprite) => { item.Value[index].Dim_datas.Add(sprite); }, true);
                }
                if (!string.IsNullOrEmpty(item.Value[i].Dim2))
                {
                    ApiCall.Instance.GetSequently<Sprite>
                        (item.Value[i].Dim2, (sprite) => { item.Value[index].Dim_datas.Add(sprite); }, true);
                }
                if (!string.IsNullOrEmpty(item.Value[i].Dim3))
                {
                    ApiCall.Instance.GetSequently<Sprite>
                        (item.Value[i].Dim3, (sprite) => { item.Value[index].Dim_datas.Add(sprite); }, true);
                }
                if (!string.IsNullOrEmpty(item.Value[i].Dim4))
                {
                    ApiCall.Instance.GetSequently<Sprite>
                        (item.Value[i].Dim4, (sprite) => { item.Value[index].Dim_datas.Add(sprite); }, true);
                }
                if (!string.IsNullOrEmpty(item.Value[i].Dim5))
                {
                    ApiCall.Instance.GetSequently<Sprite>
                        (item.Value[i].Dim5, (sprite) => { item.Value[index].Dim_datas.Add(sprite); }, true);
                }
                if (!string.IsNullOrEmpty(item.Value[i].Dim6))
                {
                    ApiCall.Instance.GetSequently<Sprite>
                        (item.Value[i].Dim6, (sprite) => { item.Value[index].Dim_datas.Add(sprite); }, true);
                }
                if (!string.IsNullOrEmpty(item.Value[i].Dim7))
                {
                    ApiCall.Instance.GetSequently<Sprite>
                        (item.Value[i].Dim7, (sprite) => { item.Value[index].Dim_datas.Add(sprite); }, true);
                }
                if (!string.IsNullOrEmpty(item.Value[i].Dim8))
                {
                    ApiCall.Instance.GetSequently<Sprite>
                        (item.Value[i].Dim8, (sprite) => { item.Value[index].Dim_datas.Add(sprite); }, true);
                }

                Tuple<string, string> tupleKey = new Tuple<string, string>(item.Key, item.Value[index].Service);
                _frameData.CommonTuple[tupleKey] = item.Value[i];
            }
        }
        
        // Definition Data
        foreach (var item in _frameData.Definition.Code)
        {
            _frameData.DefinitionTuple[item.Key] = new FrameDefinitionServiceColorDic();
            for (int i = 0; i < item.Value.Count; i++)
            {
                int index = i;
                //Debug.Log($"Frame Definition Key:{item.Key}, Count:{i}");
                item.Value[i].picRects = new List<FrameRectTransform>();
                item.Value[i].dateRects = new List<FrameRectTransform>();
                item.Value[i].qrRects = new List<FrameRectTransform>();

                item.Value[i].key = item.Key;
                switch (item.Key)
                {
                    case StringCacheManager.frame1:
                        item.Value[i].FrameType = FRAME_TYPE.FRAME_1;
                        break;
                    case StringCacheManager.frame2:
                        item.Value[i].FrameType = FRAME_TYPE.FRAME_2;
                        break;
                    case StringCacheManager.frame4:
                        item.Value[i].FrameType = FRAME_TYPE.FRAME_4;
                        break;
                    case StringCacheManager.frame8:
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

                Tuple<string, string> tupleKey = new Tuple<string, string>(item.Value[index].Service, item.Value[index].ColorCode);
                _frameData.DefinitionTuple[item.Key][tupleKey] = item.Value[index];
            }

        }

        // Each Frame Service Data
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
            if (!item.Value.Use)
                continue;

            if ( !string.IsNullOrEmpty(item.Value.Key))
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
                    case "CC":
                        item.Value.ContentType = CONTENT_TYPE.AI_CARICATURE;
                        break;
                    default:
                        CustomLogger.LogError("Cant find content type");
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

        /// 페이지별 대기 시간 파싱
        // 프린트 페이지
        if (!string.IsNullOrEmpty(BasicSetting.Config.Printing))
        {
            BasicSetting.Config.Printing_data = int.Parse(BasicSetting.Config.Printing);
        }

        // 컨텐츠 페이지
        if (!string.IsNullOrEmpty(BasicSetting.Config.ContentsMenu))
        {
            BasicSetting.Config.ContentsMenu_data = int.Parse(BasicSetting.Config.ContentsMenu);
        }

        // 서브 컨텐츠 페이지
        if (!string.IsNullOrEmpty(BasicSetting.Config.CAMenu))
        {
            BasicSetting.Config.CAMenu_data = int.Parse(BasicSetting.Config.CAMenu);
        }
        if (!string.IsNullOrEmpty(BasicSetting.Config.PRMenu))
        {
            BasicSetting.Config.PRMenu_data = int.Parse(BasicSetting.Config.PRMenu);
        }
        if (!string.IsNullOrEmpty(BasicSetting.Config.WFMenu))
        {
            BasicSetting.Config.WFMenu_data = int.Parse(BasicSetting.Config.WFMenu);
        }

        // 배경 테마 페이지
        if (!string.IsNullOrEmpty(BasicSetting.Config.BTBG))
        {
            BasicSetting.Config.BTBG_data = int.Parse(BasicSetting.Config.BTBG);
        }
        if (!string.IsNullOrEmpty(BasicSetting.Config.CABG))
        {
            BasicSetting.Config.CABG_data = int.Parse(BasicSetting.Config.CABG);
        }

        // 프레임 선택 페이지
        if (!string.IsNullOrEmpty(BasicSetting.Config.FrameSelect))
        {
            BasicSetting.Config.FrameSelect_data = int.Parse(BasicSetting.Config.FrameSelect);
        }

        // 결제 정보 확인
        if (!string.IsNullOrEmpty(BasicSetting.Config.PayConfirm))
        {
            BasicSetting.Config.PayConfirm_data = int.Parse(BasicSetting.Config.PayConfirm);
        }

        // 촬영전 유의사항 페이지
        if (!string.IsNullOrEmpty(BasicSetting.Config.PhotoStandby))
        {
            BasicSetting.Config.PhotoStandby_data = int.Parse(BasicSetting.Config.PhotoStandby);
        }

        // 프로필 결과물 선택 페이지
        if (!string.IsNullOrEmpty(BasicSetting.Config.ShootingPRSelect))
        {
            BasicSetting.Config.ShootingPRSelect_data = int.Parse(BasicSetting.Config.ShootingPRSelect);
        }

        // 꾸미기 화면 페이지
        if (!string.IsNullOrEmpty(BasicSetting.Config.OtherMenu))
        {
            BasicSetting.Config.OtherMenu_data = int.Parse(BasicSetting.Config.OtherMenu);
        }

        // 메일 데이터 쪼개기
        foreach (var item in BasicSetting.Device)
        {
            item.Value.MailList = item.Value.MailList.Replace(" ", string.Empty).Replace("\r", string.Empty).Replace("\t", string.Empty);
            item.Value.mailList_data = item.Value.MailList.Split(',');
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
                item.Value.LutFile_data = ResourceCacheManager.Instance.GetLutTexture(item.Key);
                //ApiCall.Instance.GetSequently<Texture2D>
                //    (item.Value.File, (texture) => 
                //    {
                //        item.Value.LutFile_data = texture; 
                //    }, true);
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

        UserDataManager.Instance.SetDefaultLutEffect(_filterData.OrderedFilterTable[1].Key);
    }

    FrameRectTransform ParseRectData(string data)
    {
        FrameRectTransform rectData = new FrameRectTransform();

        string removeEmpty = data.Replace(" ", string.Empty).Replace("\r", string.Empty).Replace("\t", string.Empty).Replace("\n", string.Empty);
        removeEmpty = removeEmpty.ToLower();
        string pattern = @"pos\.x:(?<posX>[-\d.]+)\s*pos\.y:(?<posY>[-\d.]+)\]\[width:(?<width>[-\d.]+)\s*height:(?<height>[-\d.]+)\]anchors:min:\[x:(?<AnchorMinX>[-\d.]+)\s*y:(?<AnchorMinY>[-\d.]+)\]max:\[x:(?<AnchorMaxX>[-\d.]+)\s*y:(?<AnchorMaxY>[-\d.]+)\]pivot:\[x:(?<pivotX>[-\d.]+)\s*y:(?<pivotY>[-\d.]+)\]rotation:\[x:(?<rotX>[-\d.]+)\s*y:(?<rotY>[-\d.]+)\s*z:(?<rotZ>[-\d.]+)\]";

        Regex regex = new Regex(pattern);
        Match match = regex.Match(removeEmpty);

        // Extract Position
        if (match.Success)
        {
            rectData.anchoredPosition = new Vector2(
                float.Parse(match.Groups["posX"].Value),
                float.Parse(match.Groups["posY"].Value)
            );

            rectData.sizeDelta = new Vector2(
                float.Parse(match.Groups["width"].Value),
                float.Parse(match.Groups["height"].Value)
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
                float.Parse(match.Groups["pivotX"].Value),
                float.Parse(match.Groups["pivotY"].Value)
            );

            rectData.rotation = new Vector3(
                float.Parse(match.Groups["rotX"].Value),
                float.Parse(match.Groups["rotY"].Value),
                float.Parse(match.Groups["rotZ"].Value)
                );
        }
        else
        {
            Debug.Log($"{removeEmpty} is Not match with pattern");
        }

        return rectData;
    }

    int[] ParsePriceData(string data)
    {
        if(string.IsNullOrEmpty(data))
        {
            return null;
        }

        string trimmedInput = data.Replace(" ", "");
        string[] splitArray = trimmedInput.Split(',');
        int[] prices = new int[splitArray.Length];
        for (int i = 0; i < splitArray.Length; i++)
        {
            prices[i] = int.Parse(splitArray[i]);
        }

        return prices;
    }

    public void DownloadChromaKeyData()
    {
        _chromakeyFrame.OrderedChromakeyToneTable = new ChromakeyFrameData.ChromakeyFrame.OrderedChromakeyTableEntryDic();
        foreach (var item in _chromakeyFrame.ChromakeyToneTable)
        {
            ApiCall.Instance.GetSequently<Texture2D>(item.Value.Thumbnail, (texture) =>
            {
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                item.Value.Thumbnail_data = sprite;
            }, true);

            item.Value.orderedImage = new ChromakeyFrameData.ImageOrderedDic();

            ApiCall.Instance.GetSequently<Texture2D>(item.Value.Image1, (texture) => item.Value.orderedImage[0] = texture, true);
            ApiCall.Instance.GetSequently<Texture2D>(item.Value.Image2, (texture) => item.Value.orderedImage[1] = texture, true);
            ApiCall.Instance.GetSequently<Texture2D>(item.Value.Image3, (texture) => item.Value.orderedImage[2] = texture, true);
            ApiCall.Instance.GetSequently<Texture2D>(item.Value.Image4, (texture) => item.Value.orderedImage[3] = texture, true);
            ApiCall.Instance.GetSequently<Texture2D>(item.Value.Image5, (texture) => item.Value.orderedImage[4] = texture, true);
            ApiCall.Instance.GetSequently<Texture2D>(item.Value.Image6, (texture) => item.Value.orderedImage[5] = texture, true);
            ApiCall.Instance.GetSequently<Texture2D>(item.Value.Image7, (texture) => item.Value.orderedImage[6] = texture, true);
            ApiCall.Instance.GetSequently<Texture2D>(item.Value.Image8, (texture) => item.Value.orderedImage[7] = texture, true);

            _chromakeyFrame.OrderedChromakeyToneTable[item.Value.Sequence] = item.Value;
        }

        _chromakeyFrame.OrderedChromakeyFrameTable = new ChromakeyFrameData.ChromakeyFrame.OrderedChromakeyTableEntryDic();
        foreach (var item in _chromakeyFrame.ChromakeyFrameTable)
        {
            ApiCall.Instance.GetSequently<Texture2D>(item.Value.Thumbnail, (texture) =>
            {
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                item.Value.Thumbnail_data = sprite;
            }, true);

            item.Value.orderedImage = new ChromakeyFrameData.ImageOrderedDic();

            ApiCall.Instance.GetSequently<Texture2D>(item.Value.Image1, (texture) => item.Value.orderedImage[0] = texture, true);
            ApiCall.Instance.GetSequently<Texture2D>(item.Value.Image2, (texture) => item.Value.orderedImage[1] = texture, true);
            ApiCall.Instance.GetSequently<Texture2D>(item.Value.Image3, (texture) => item.Value.orderedImage[2] = texture, true);
            ApiCall.Instance.GetSequently<Texture2D>(item.Value.Image4, (texture) => item.Value.orderedImage[3] = texture, true);
            ApiCall.Instance.GetSequently<Texture2D>(item.Value.Image5, (texture) => item.Value.orderedImage[4] = texture, true);
            ApiCall.Instance.GetSequently<Texture2D>(item.Value.Image6, (texture) => item.Value.orderedImage[5] = texture, true);
            ApiCall.Instance.GetSequently<Texture2D>(item.Value.Image7, (texture) => item.Value.orderedImage[6] = texture, true);
            ApiCall.Instance.GetSequently<Texture2D>(item.Value.Image8, (texture) => item.Value.orderedImage[7] = texture, true);

            _chromakeyFrame.OrderedChromakeyFrameTable[item.Value.Sequence] = item.Value;
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
