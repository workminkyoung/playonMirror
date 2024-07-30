using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdminManager : SingletonBehaviour<AdminManager>
{
    public ConfigDefaultData _configDefaultData;

    private string _configDefaultAPI = "http://api.playon-vive.com/config/default/latest";

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
    }
}
