using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vivestudios.UI;

public class UP_Shoot : UP_BasePage
{
    public RawImage _preview;

    protected FlashEffect _flashEffect;
    protected UC_ShootState _shootState;
    protected UC_GuideGrid _guideGrid;

    protected int _width;
    protected int _height;
    protected Camera _camera;
    protected Action<Texture2D> SaveCapturePhoto;
    protected Action NextPage;
    protected Coroutine _takeshootCoroutine = null;

    private RenderTexture _clearTexture;

    public override void InitPage()
    {
        _flashEffect = GetComponentInChildren<FlashEffect>();
        _shootState = GetComponentInChildren<UC_ShootState>();
        _guideGrid = GetComponentInChildren<UC_GuideGrid>();

        _guideGrid.Setting();
        _flashEffect.Setting();
        _shootState.Setting();
        _shootState.SetParentPage(this);
        _shootState.main = _pageController as PC_Main;
        _camera = (_pageController as PC_Main).captureCameras[CAMERA_VIEW_TYPE.CAPTURE];

        DSLRManager.Instance.OnLoadPreview += (preview) =>
        {
            _preview.texture = preview;
        };

        GameManager.OnGameResetAction += () =>
        {
            _preview.texture = null;
            _preview.color = Color.clear;
        };

        (_pageController as PC_Main).SetCameraData();
        (_pageController as PC_Main).globalPage.TempErrorOpenAction += () => _shootState.StopCountDown();
        InitCameraType();
    }

    public override void BindDelegates()
    {
    }

    public override void OnPageEnable()
    {
        if(ConfigData.config.camType == (int)CAMERA_TYPE.DSLR)
        {
            DSLRManager.inst.OpenSession();
        }
        
        _flashEffect.Init();

        //TODO : RESET

        //set record
        (_pageController as PC_Main).SetRecordCamera();
        (_pageController as PC_Main).SetRecordTexture();

        PhotoDataManager.Instance.SetPhotoCount();

        if (ConfigData.config.camType == (int)CAMERA_TYPE.DSLR)
        {
            DSLRManager.Instance.LoadCountMax = PhotoDataManager.inst.photoCount;
        }

        StartCoroutine(WaitCameraStart());
    }

    public override void OnPageDisable()
    {

    }


    void InitCameraType()
    {
        switch (ConfigData.config.camType)
        {
            case (int)CAMERA_TYPE.WEBCAM:
                foreach (var raw in (_pageController as PC_Main).captureRawimgs)
                {
                    raw.Value.rectTransform.sizeDelta = new Vector2(1920, 1080);
                }
                _preview.rectTransform.sizeDelta = new Vector2(1920, 1080);
                break;
            case (int)CAMERA_TYPE.NDI:
                foreach (var raw in (_pageController as PC_Main).captureRawimgs)
                {
                    raw.Value.rectTransform.sizeDelta = new Vector2(1920, 1080);
                }
                _preview.rectTransform.sizeDelta = new Vector2(1920, 1080);
                break;
            case (int)CAMERA_TYPE.DSLR:
                foreach (var raw in (_pageController as PC_Main).captureRawimgs)
                {
                    raw.Value.rectTransform.sizeDelta = new Vector2(1920, 1280);
                }
                _preview.rectTransform.sizeDelta = new Vector2(1920, 1280);
                break;
            default:
                break;
        }
    }

    IEnumerator WaitCameraStart()
    {
        //ready texture change needed
        _shootState.photoMax = PhotoDataManager.inst.photoCount;
        yield return new WaitForSecondsRealtime(1);
        CheckCameraType();
        StartShoot();
    }

    void CheckCameraType()
    {
        PC_Main main = _pageController as PC_Main;

        switch (ConfigData.config.camType)
        {
            case (int)CAMERA_TYPE.WEBCAM:
                CameraManager.Instance.PlayWebcam();
                if (_preview.texture == null)
                {
                    _preview.texture = CameraManager.Instance.webCamTexture;
                }
                if (main.captureRawimgs[CAMERA_VIEW_TYPE.CAPTURE].texture == null)
                {
                    main.captureRawimgs[CAMERA_VIEW_TYPE.CAPTURE].texture = CameraManager.Instance.webCamTexture;
                }
                _preview.color = Color.white;
                break;
            case (int)CAMERA_TYPE.NDI:
                if (_preview.texture == null)
                {
                    _preview.texture = NDIManager.Instance.ndiTexture;
                }
                if (main.captureRawimgs[CAMERA_VIEW_TYPE.CAPTURE].texture == null)
                {
                    main.captureRawimgs[CAMERA_VIEW_TYPE.CAPTURE].texture = NDIManager.Instance.ndiTexture;
                }
                break;
            case (int)CAMERA_TYPE.DSLR:
                DSLRManager.Instance.CameraSetting();
                DSLRManager.Instance.RemovePhoto();
                DSLRManager.Instance.StartEVF();
                _preview.color = Color.white;
                break;
            default:
                break;
        }
    }

    public virtual void StartShoot()
    {
        _shootState.StartCheckTime();
        _shootState.StartCountDown();
    }

    //screen shot [WEBCAM & NDI]
    protected IEnumerator TakeShoot()

    {
        SoundManager.Instance.Play(AUDIO.CAMERA);
        yield return new WaitForEndOfFrame();

        Texture2D screenShoot = null;
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);

        RenderTexture rt = new RenderTexture(_width, _height, 32);
        rt.enableRandomWrite = true;
        rt.Create();

        RenderTexture preTargetTexture = _camera.targetTexture;
        _camera.targetTexture = renderTexture;
        _camera.Render();

        Vector2 readArea = new Vector2(_width, _height);
        Vector2 readStartPoint = new Vector2(renderTexture.width / 2 - readArea.x / 2, renderTexture.height / 2 - readArea.y / 2);

        RenderTexture.active = renderTexture;
        screenShoot = new Texture2D(_width, _height);
        screenShoot.ReadPixels(new Rect(readStartPoint.x, readStartPoint.y,
                                        screenShoot.width, screenShoot.height), 0, 0);
        screenShoot.Apply();

        _camera.targetTexture = preTargetTexture;
        SaveCapturePhoto(screenShoot);//send? pc_main에 저장해두는 의미로 변수명을 바꿈좋을듯

        if (Debug.isDebugBuild)
        {
            //사진저장
            StorageManager.Instance.SavePicture("resultimg", screenShoot);
        }

        CameraManager.Instance.PauseWebcam();
        if (UserDataManager.inst.selectedContent == CONTENT_TYPE.AI_CARTOON ||
            UserDataManager.inst.selectedContent == CONTENT_TYPE.AI_BEAUTY)
        {
            //_aiCartoon.RequestDiffusion(screenShoot);
            ApiCall.Instance.InRequestList(screenShoot);
        }

        _flashEffect.StartFlashing(() =>
        {
            if (_shootState.PhotoCountUp())
            {
                // end shooting
                StartCoroutine(WaitSecToNext());
            }
            else
            {
                // shooting
                CameraManager.Instance.PlayWebcam();
                _shootState.StartCountDown();
            }
        });

    }
    protected IEnumerator TakeShootDSLR()
    {
        SoundManager.Instance.Play(AUDIO.CAMERA);
        yield return new WaitForEndOfFrame();
        DSLRManager.Instance.CameraForceShoot(() =>
        {
            if (_shootState.PhotoCountUp())
            {
                // end shooting
                StartCoroutine(WaitSecToNext());
            }
            else
            {
                // shooting
                _shootState.StartCountDown();
                //DSLRManager.inst.CameraAutoFocusON();
            }
        });

        _flashEffect.StartFlashing();
    }

    IEnumerator WaitSecToNext()
    {
        yield return new WaitForSeconds(1);
        NextPage();
    }

    protected Texture2D CropTexture(Texture2D sourceTexture, Rect cropRect, bool flip)
    {
        int x = Mathf.FloorToInt(cropRect.x);
        int y = Mathf.FloorToInt(cropRect.y);
        int width = Mathf.FloorToInt(cropRect.width);
        int height = Mathf.FloorToInt(cropRect.height);

        // 새로운 Texture2D 생성
        Texture2D croppedTexture = new Texture2D(width, height);

        // 지정한 영역의 픽셀 값을 가져와 새로운 Texture2D에 설정
        Color[] pixels = sourceTexture.GetPixels(x, y, width, height);
        if (flip)
        {
            var newPixels = new Color[pixels.Length];

            var width_flip = width;
            var rows = height;

            for (var i = 0; i < width_flip; i++)
            {
                for (var j = 0; j < rows; j++)
                {
                    newPixels[i + j * width_flip] = pixels[(width_flip - i - 1) + j * width_flip];
                }
            }

            croppedTexture.SetPixels(newPixels);
        }
        else
        {
            croppedTexture.SetPixels(pixels);
        }
        croppedTexture.Apply(); // 변경사항 적용

        return croppedTexture;
    }

    protected override void OnPageReset()
    {
    }
}
