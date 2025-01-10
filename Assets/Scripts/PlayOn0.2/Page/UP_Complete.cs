using System.Collections;
using UnityEngine;
using Vivestudios.UI;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using System;

public class UP_Complete : UP_BasePage
{
    [SerializeField]
    Button _nextBtn;
    [SerializeField]
    Image _bgImage;

    private const int WAIT_TIME = 10;
    private Coroutine TimeLimitCoroutine = null;

    public override void InitPage()
    {
        //throw new System.NotImplementedException();
        _nextBtn.onClick.AddListener(() =>
        {
            GameOver();
        });
    }

    public override void BindDelegates()
    {
        //throw new System.NotImplementedException();
        //_rawimage.texture = MakeQRCode("https://forms.gle/gHHdb82HmsAsGB5R8");
    }

    public override void ApplyAdminData()
    {
        base.ApplyAdminData();
        _bgImage.sprite = AdminManager.Instance.BasicSetting.Config.EndImage_data;
    }

    private void GameOver()
    {
        if (TimeLimitCoroutine != null)
        {
            StopCoroutine(TimeLimitCoroutine);
            TimeLimitCoroutine = null;
        }

        CustomLogger.Log(StringCacheManager.inst.DividerLine + " Session Closing " + StringCacheManager.inst.DividerLine);
        (_pageController as PC_Main).UpdatePhotoPaper();
        GameManager.inst.ResetGame();
    }

    private IEnumerator TimeLimitRoutine()
    {
        int time = 0;
        while (0 <= WAIT_TIME - time)
        {
            yield return new WaitForSecondsRealtime(1);
            time++;
        }

        GameOver();
        TimeLimitCoroutine = null;
    }

    public override void OnPageEnable()
    {
        if (_pageController == null)
            return;
        TimeLimitCoroutine = StartCoroutine(TimeLimitRoutine());
    }

    public override void OnPageDisable()
    {
        if (TimeLimitCoroutine != null)
        {
            StopCoroutine(TimeLimitCoroutine);
            TimeLimitCoroutine = null;
        }
    }

    protected override void OnPageReset()
    {
    }
}
