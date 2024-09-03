using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vivestudios.UI;

public class UP_ShootCartoon : UP_Shoot
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
                ApiCall.inst.StopCheckRequest();
                ApiCall.inst.StopActiveCoroutine();
                DSLRManager.inst.ResetData();

                OnPageEnable();
                //StartCoroutine(StartReconnect());
            }
        };
    }

    //IEnumerator StartReconnect()
    //{
    //    yield return new WaitForSeconds(0.5f);
    //}

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
        PhotoDataManager.inst.ResetPhotoData();
        //중첩? 덮어쓰기? 확인하기
        ApiCall.Instance.SendResult = (tex) =>
        {
            PhotoDataManager.inst.AddPhotoConverted(tex);
        };

        base.OnPageEnable();

        _shootState.duration = ConfigData.config.photoTime_cartoon;
        //_guideGrid.SetSize(PhotoDataManager.inst.isLandscape);
        _width = PlayOnProperties.crop4x3_width;
        _height = PlayOnProperties.crop4x3_height;
        ConfigData.config.photoTime = ConfigData.config.photoTime_cartoon;
        SaveCapturePhoto = (tex) => PhotoDataManager.Instance.AddPhotoOrigin(tex);

        ApiCall.Instance.ResetRequest(PhotoDataManager.inst.photoCount);
        ApiCall.Instance.StartCheckRequest();

        //매번 초기화됨
        //dslr 촬영 이미지 로드 action 할당
        DSLRManager.Instance.OnLoadTexture = (texture) =>
        {
            float rate = texture.width / 1920.0f;
            float width = _width * rate;
            float height = _height * rate;
            float x = texture.width / 2 - width / 2;
            float y = texture.height / 2 - height / 2;
            Rect rect = new Rect(x, y, width, height);

            Texture2D cropped = CropTexture(texture, rect, true);

            if(UserDataManager.inst.isChromaKeyOn)
            {
                Texture2D combined = ChromaKeyModule.inst.CombineImage(ChromaKeyModule.inst.options[UserDataManager.inst.selectedChromaKeyNum].orderedImage[PhotoDataManager.inst.photoOrigin.Count], cropped);

                while(combined == null)
                {
                    if(combined != null)
                    {
                        break;
                    }
                }

                PhotoDataManager.inst.AddPhotoOrigin(combined);
                ApiCall.Instance.InRequestList(combined);
            }
            else
            {
                PhotoDataManager.inst.AddPhotoOrigin(cropped);

                ApiCall.Instance.InRequestList(cropped);
            }
        };
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
            //DSLR 사진 로드 요청
            if (UserDataManager.inst.selectedContent == CONTENT_TYPE.AI_CARTOON)
            {
                DSLRManager.Instance.StartLoadPhoto();
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
