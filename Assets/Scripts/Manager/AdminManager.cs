using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using UnityEngine;

public class AdminManager : SingletonBehaviour<AdminManager>
{
    private ConfigDefaultData _configDefaultData;
    private BubbleData.BubbleData _bubbleData;
    private FilterData.FilterData _filterData;
    private ServiceData.ServiceData _serviceData;
    private BasicData.BasicSetting _basicSetting;
    private ChromakeyFrameData.ChromakeyFrame _chromakeyFrame;
    private ShootingScreenData.ShootScreenDic _shootScreen;

    private string _configDefaultAPI = "http://api.playon-vive.com/config/default/latest";

    public ConfigDefaultData ConfigDefaultData => _configDefaultData;
    public BubbleData.BubbleData BubbleData => _bubbleData;
    public FilterData.FilterData FilterData => _filterData;
    public ServiceData.ServiceData ServiceData => _serviceData;
    public BasicData.BasicSetting BasicSetting => _basicSetting;
    public ChromakeyFrameData.ChromakeyFrame ChromakeyFrame => _chromakeyFrame;
    public ShootingScreenData.ShootScreenDic ShootScreen => _shootScreen;

    protected override void Init()
    {
        GameManager.OnGameResetAction += ResetAdminData;

        ApiCall.Instance.Get(_configDefaultAPI, GetResponse);
    }

    public void ResetAdminData()
    {

    }

    public void GetResponse(string result)
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
}
