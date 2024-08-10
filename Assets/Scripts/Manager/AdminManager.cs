using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using Unity.VectorGraphics;
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

    protected override void Init ()
    {
        GameManager.OnGameResetAction += ResetAdminData;

        ApiCall.Instance.Get<string>(_configDefaultAPI, GetResponse);
    }

    public void ResetAdminData ()
    {

    }

    public void GetResponse (string result)
    {
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

    private void SetBubbleData ()
    {
        string result = _configDefaultData.config_default_set.result.BubbleData.ToString();
        _bubbleData = JsonConvert.DeserializeObject<BubbleData.BubbleData>(result);
        DownloadBubbleData();
    }

    private void SetFilterData ()
    {
        string result = _configDefaultData.config_default_set.result.FilterData.ToString();
        _filterData = JsonConvert.DeserializeObject<FilterData.FilterData>(result);
    }

    private void SetServiceData ()
    {
        string result = _configDefaultData.config_default_set.result.ServiceData.ToString();
        _serviceData = JsonConvert.DeserializeObject<ServiceData.ServiceData>(result);
        DownloadServiceData();
    }

    private void SetBasicData ()
    {
        string result = _configDefaultData.config_default_set.result.BasicSetting.ToString();
        _basicSetting = JsonConvert.DeserializeObject<BasicData.BasicSetting>(result);
    }

    private void SetChromakeyFrameData ()
    {
        string result = _configDefaultData.config_default_set.result.ChromakeyFrame.ToString();
        _chromakeyFrame = JsonConvert.DeserializeObject<ChromakeyFrameData.ChromakeyFrame>(result);
        DownloadChromaKeyData();
    }

    private void SetShootScreenData ()
    {
        Dictionary<string, object> shootScreenPair = new Dictionary<string, object>();
        string result = _configDefaultData.config_default_set.result.ShootingScreen.ToString();
        shootScreenPair = JsonConvert.DeserializeObject<Dictionary<string, object>>(result);

        _shootScreen = new ShootingScreenData.ShootScreenDic();

        foreach(var item in shootScreenPair)
        {
            Dictionary<string, string> shootScreenEntryDic = new Dictionary<string, string>();
            shootScreenEntryDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(item.Value.ToString());

            ShootingScreenData.ShootScreenEntry shootScreen = new ShootingScreenData.ShootScreenEntry();
            shootScreen.url = new ShootingScreenData.ShootScreenEntryDic();
            shootScreen.ratio = new ShootingScreenData.ShootScreenEntryDic();
            shootScreen.korean = new ShootingScreenData.ShootScreenEntryDic();
            shootScreen.chinese = new ShootingScreenData.ShootScreenEntryDic();
            shootScreen.english = new ShootingScreenData.ShootScreenEntryDic();
            foreach(var entry in shootScreenEntryDic)
            {
                if(entry.Key.Contains("Key"))
                {
                    shootScreen.Key = entry.Value.ToString();
                }
                else if(entry.Key.Contains("url"))
                {
                    shootScreen.url.Add(entry.Key, entry.Value);
                }
                else if(entry.Key.Contains("ratio"))
                {
                    shootScreen.ratio.Add(entry.Key, entry.Value);
                }
                else if(entry.Key.Contains("Korean"))
                {
                    shootScreen.korean.Add(entry.Key, entry.Value);
                }
                else if(entry.Key.Contains("Chinese"))
                {
                    shootScreen.chinese.Add(entry.Key, entry.Value);
                }
                else if(entry.Key.Contains("English"))
                {
                    shootScreen.english.Add(entry.Key, entry.Value);
                }
                else if(entry.Key.Contains("ConversionTime"))
                {
                    shootScreen.ConversionTime = entry.Value.ToString();
                }
                else if(entry.Key.Contains("ConversionVideo"))
                {
                    shootScreen.ConversionVideo = entry.Value.ToString();
                }
            }

            _shootScreen.Add(shootScreen.Key, shootScreen);
        }
    }

    public void DownloadServiceData ()
    {
        foreach(var item in _serviceData.Contents)
        {
            if(item.Value.ImageThumbnail != null && item.Value.ImageThumbnail != string.Empty)
            {
                ApiCall.Instance.GetSequently<Texture2D>
                    (item.Value.ImageThumbnail, (texture) => { item.Value.ImageThumbnail_data = texture; }, true);
            }

            if(item.Value.VideoThumbnail != null && item.Value.VideoThumbnail != string.Empty)
            {
                ApiCall.Instance.GetSequently<string>
                    (item.Value.VideoThumbnail, (path) => { item.Value.VideoThumbnail_path = path; }, true);
            }

            if(item.Value.GuideImage != null && item.Value.GuideImage != string.Empty)
            {
                ApiCall.Instance.GetSequently<Texture2D>
                    (item.Value.GuideImage, (texture) => { item.Value.GuideImage_data = texture; }, true);
            }

            if(item.Value.BGGuideImage != null && item.Value.BGGuideImage != string.Empty)
            {
                ApiCall.Instance.GetSequently<Texture2D>
                    (item.Value.BGGuideImage, (texture) => { item.Value.BGGuideImage_data = texture; }, true);
            }

            if(item.Value.ShootGuideImage != null && item.Value.ShootGuideImage != string.Empty)
            {
                ApiCall.Instance.GetSequently<Texture2D>
                    (item.Value.ShootGuideImage, (texture) => { item.Value.ShootGuideImage_data = texture; }, true);
            }
        }
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
