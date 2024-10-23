using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vivestudios.UI;

public class UP_ShootWhatIf : UP_Shoot
{
    public override void InitPage()
    {
        base.InitPage();

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

        if(UserDataManager.inst.selectedContent == CONTENT_TYPE.WHAT_IF)
        {
            UserDataManager.Instance.SetCurrentShootTime(AdminManager.Instance.BasicSetting.Config.WFShootTime);
        }
        else if(UserDataManager.inst.selectedContent == CONTENT_TYPE.AI_CARICATURE)
        {
            UserDataManager.Instance.SetCurrentShootTime(AdminManager.Instance.BasicSetting.Config.CCShootTime);
        }

        _width = PlayOnProperties.crop3x4_width;
        _height = PlayOnProperties.crop3x4_height;
        SaveCapturePhoto = (tex) => PhotoDataManager.Instance.AddPhotoOrigin(tex);
    }

    public override void StartShoot()
    {
        base.StartShoot();

        _shootState.duration = AdminManager.Instance.BasicSetting.Config.WFShootTime;

        //start shooting
        if (ConfigData.config.camType == (int)CAMERA_TYPE.DSLR)
        {
            //DSLR 사진 로드 요청
            if (UserDataManager.inst.selectedContent == CONTENT_TYPE.WHAT_IF ||
                UserDataManager.inst.selectedContent == CONTENT_TYPE.AI_CARICATURE)
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
