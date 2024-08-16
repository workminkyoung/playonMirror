using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vivestudios.UI;

//NotUsed
public class UP_ProcessLoading : UP_BasePage
{
    private int _loadingTime = 30;
    private string[] _loadingText;
    private float _interval;

    public override void InitPage()
    {
        //_loadingTime = ConfigData.config.loadingTime;
        _loadingText = StringCacheManager.inst.loadingTexts;
        _interval = _loadingTime / _loadingText.Length;
    }

    public override void BindDelegates()
    {
    }

    private IEnumerator LoadingRoutine()
    {
        int time = 0;
        while (0 < _loadingTime - time)
        {
            yield return new WaitForSecondsRealtime(1);
            time++;
        }
    }

    public override void OnPageEnable()
    {
    }

    public override void OnPageDisable()
    {
    }

    protected override void OnPageReset()
    {
    }
}
