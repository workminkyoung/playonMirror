using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vivestudios.UI;

public class UP_ShootProfile : UP_Shoot
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

        _photoCountMax = 3;
    }

    public override void BindDelegates()
    {
        base.BindDelegates();

        NextPage = () =>
        {
            (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_SELECT_AI_PROFILE_RESULT);
        };
    }

    public override void OnPageEnable()
    {
        base.OnPageEnable();

        _guideGrid.SetSize(PhotoDataManager.inst.isLandscape);
        //_guideGrid.SetActivate(false);
        _width = PlayOnProperties.crop3x4_width;
        _height = PlayOnProperties.crop3x4_height;
        ConfigData.config.photoTime = ConfigData.config.photoTime_profile;
        SaveCapturePhoto = (tex) => PhotoDataManager.Instance.AddPhotoOrigin(tex);
    }

    public override void StartShoot()
    {
        base.StartShoot();

        _shootState.duration = ConfigData.config.photoTime_profile;

        //start shooting
        if (ConfigData.config.camType == (int)CAMERA_TYPE.DSLR)
        {
            //DSLR 사진 로드 요청
            if (UserDataManager.inst.selectedContent == CONTENT_TYPE.AI_PROFILE)
            {
                _shootState.ResetShootAction(() =>
                {
                    if (_takeshootCoroutine != null)
                        StopCoroutine(_takeshootCoroutine);

                    _takeshootCoroutine = StartCoroutine(TakeShootDSLR());
                }, NextPage);
            }
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
