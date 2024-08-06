using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

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
    private LANGUAGE_TYPE _language = LANGUAGE_TYPE.KOR;
    private string _configDefaultAPI = "http://api.playon-vive.com/config/default/latest";

    public ConfigDefaultData ConfigDefaultData => _configDefaultData;
    public BubbleData.BubbleData BubbleData => _bubbleData;
    public FilterData.FilterData FilterData => _filterData;
    public ServiceData.ServiceData ServiceData => _serviceData;
    public BasicData.BasicSetting BasicSetting => _basicSetting;
    public ChromakeyFrameData.ChromakeyFrame ChromakeyFrame => _chromakeyFrame;
    public ShootingScreenData.ShootScreenDic ShootScreen => _shootScreen;
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
            shootScreen.ratio = new ShootingScreenData.ShootScreenEntryDic();
            shootScreen.korean = new ShootingScreenData.ShootScreenEntryDic();
            shootScreen.chinese = new ShootingScreenData.ShootScreenEntryDic();
            shootScreen.english = new ShootingScreenData.ShootScreenEntryDic();
            foreach (var entry in shootScreenEntryDic)
            {
                if (entry.Key.Contains("Key"))
                {
                    shootScreen.Key = entry.Value.ToString();
                }
                else if (entry.Key.Contains("url"))
                {
                    shootScreen.url.Add(entry.Key, entry.Value);
                }
                else if (entry.Key.Contains("ratio"))
                {
                    shootScreen.ratio.Add(entry.Key, entry.Value);
                }
                else if (entry.Key.Contains("Korean"))
                {
                    shootScreen.korean.Add(entry.Key, entry.Value);
                }
                else if (entry.Key.Contains("Chinese"))
                {
                    shootScreen.chinese.Add(entry.Key, entry.Value);
                }
                else if (entry.Key.Contains("English"))
                {
                    shootScreen.english.Add(entry.Key, entry.Value);
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
    }

    public void DownloadServiceData()
    {
        foreach (var item in _serviceData.Contents)
        {
            if(item.Value.Key != null && item.Value.Key != string.Empty)
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

            if(item.Value.ImageThumbnail != null && item.Value.ImageThumbnail != string.Empty)
            {
                ApiCall.Instance.GetSequently<Texture2D>
                    (item.Value.ImageThumbnail, (texture) => {item.Value.ImageThumbnail_data = texture;}, true);
            }

            if (item.Value.VideoThumbnail !=  null && item.Value.VideoThumbnail != string.Empty)
            {
                ApiCall.Instance.GetSequently<string>
                    (item.Value.VideoThumbnail, (path) => { item.Value.VideoThumbnail_path = path; }, true);
            }

            if (item.Value.GuideImage != null && item.Value.GuideImage != string.Empty)
            {
                ApiCall.Instance.GetSequently<Texture2D>
                    (item.Value.GuideImage, (texture) => { item.Value.GuideImage_data = texture; }, true);
            }

            if (item.Value.BGGuideImage != null && item.Value.BGGuideImage != string.Empty)
            {
                ApiCall.Instance.GetSequently<Texture2D>
                    (item.Value.BGGuideImage, (texture) => { item.Value.BGGuideImage_data = texture; }, true);
            }

            if (item.Value.ShootGuideImage != null && item.Value.ShootGuideImage != string.Empty)
            {
                ApiCall.Instance.GetSequently<Texture2D>
                    (item.Value.ShootGuideImage, (texture) => { item.Value.ShootGuideImage_data = texture; }, true);
            }
        }

    }
}
