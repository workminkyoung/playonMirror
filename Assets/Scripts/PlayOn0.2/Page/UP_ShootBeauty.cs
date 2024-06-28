using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vivestudios.UI;

public class UP_ShootBeauty : UP_Shoot
{
    public override void InitPage()
    {
        base.InitPage();

        //(_pageController as PC_Main).globalPage.ErrorOpenAction += () =>
        //{
        //    if (gameObject.activeSelf)
        //    {
        //        if (_takeshootCoroutine != null)
        //            StopCoroutine(_takeshootCoroutine);
        //    }
        //};
        (_pageController as PC_Main).globalPage.TempErrorClossAction += () =>
        {
            if (gameObject.activeSelf)
            {
                DSLRManager.inst.ResetData();
                OnPageEnable();
            }
        };
    }

    public override void BindDelegates()
    {
        base.BindDelegates();


        NextPage = () =>
        {
            (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_LOADING);
        };
    }

    public override void OnPageEnable()
    {
        base.OnPageEnable();

        _shootState.duration = ConfigData.config.photoTime_beauty;
        _guideGrid.SetSize(PhotoDataManager.inst.isLandscape);
        _width = PlayOnProperties.crop4x3_width;
        _height = PlayOnProperties.crop4x3_height;
        ConfigData.config.photoTime = ConfigData.config.photoTime_beauty;
        SaveCapturePhoto = (tex) => PhotoDataManager.Instance.AddPhotoOrigin(tex);
    }

    public override void OnPageDisable()
    {
        base.OnPageDisable();
    }

    public override void StartShoot()
    {
        base.StartShoot();

        //start shooting
        if (ConfigData.config.camType == (int)CAMERA_TYPE.DSLR)
        {
            _shootState.ResetShootAction(() =>
            {
                if (_takeshootCoroutine != null)
                    StopCoroutine(_takeshootCoroutine);

                _takeshootCoroutine = StartCoroutine(TakeShootDSLR());
            }, NextPage);
        }
        else
        {
            _shootState.ResetShootAction(() =>
            {
                if (_takeshootCoroutine != null)
                    StopCoroutine(_takeshootCoroutine);

                _takeshootCoroutine = StartCoroutine(TakeShoot());
            }, NextPage);
        }
    }
}
