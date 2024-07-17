using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using Vivestudios.UI;

public class UP_Load : UP_BasePage
{
    [SerializeField]
    private TextMeshProUGUI _loadingText;
    [SerializeField]
    private bool _isReady = false;

    private int _loadingTime;
    private int _curStep = 0;
    private float _curInterval;
    private float _interval;
    private string[] _loadingTexts;

    public float time;

    public override void InitPage()
    {
    }

    public override void BindDelegates()
    {
    }

    public override void EnablePage(bool isEnable)
    {
        _loadingTime = ConfigData.config.loadingTime;
        _interval = _loadingTime / StringCacheManager.inst.loadingTexts.Length;
        base.EnablePage(isEnable);
        if (isEnable)
        {
            //(_pageController as PC_Main).selectedContent
            //if (ConfigData.config.camType == (int)CAMERA_TYPE.DSLR)
            //    DSLRManager.Instance.ReleaseCamera();

            _isReady = false;
            _loadingTexts = StringCacheManager.inst.loadingTexts;
            switch (UserDataManager.inst.selectedContent)
            {
                case CONTENT_TYPE.AI_CARTOON:
                    StartCoroutine(CheckReady());
                    break;
                case CONTENT_TYPE.AI_PROFILE:
                    RequestAIProfile();
                    break;
                case CONTENT_TYPE.AI_TIME_MACHINE:
                    _isReady = true;
                    break;
                case CONTENT_TYPE.AI_BEAUTY:
                    _loadingTime = ConfigData.config.loadingTimeBeauty;
                    _interval = (float)_loadingTime / StringCacheManager.inst.loadingTexts.Length;
                    LoadBeautyPhotos();
                    break;
                case CONTENT_TYPE.WHAT_IF:
                    _loadingTexts = StringCacheManager.inst.loadingTextsWhatIf;
                    RequestWhatIf();
                    break;
                default:
                    break;
            }

            _curStep = 0;
            _curInterval = _interval;
            _loadingText.text = _loadingTexts[_curStep];

            DSLRManager.Instance.EndEVF();
            DSLRManager.Instance.CloseSession();
            StartCoroutine(Loading());
        }
    }

    void RequestAIProfile()
    {
        ProfileModule.inst.GetProfileImages(PhotoDataManager.inst.selectedAIProfile,
                                            UserDataManager.inst.selectedProfileType,
                                            (value) =>
                                            {
                                                CustomLogger.Log("complet ai");
                                                PhotoDataManager.inst.SetPhotoConverted(value);
                                                _isReady = true;
                                            });
    }

    void RequestWhatIf()
    {
        ProfileModule.inst.GetWhatIfImages(PhotoDataManager.inst.selectedAIProfile,
                                            UserDataManager.inst.selectedProfileType,
                                            (value) =>
                                            {
                                                CustomLogger.Log("complet ai");
                                                PhotoDataManager.inst.SetPhotoConverted(value);
                                                _isReady = true;
                                            });
    }

    void LoadBeautyPhotos()
    {
        int _width, _height;
        _width = PlayOnProperties.crop4x3_width;
        _height = PlayOnProperties.crop4x3_height;

        DSLRManager.Instance.OnLoadTexture = (texture) =>
        {
            //한장 로드했을때

            float rate = texture.width / 1920.0f;
            float width = _width * rate;
            float height = _height * rate;
            float x = texture.width / 2 - width / 2;
            float y = texture.height / 2 - height / 2;
            Rect rect = new Rect(x, y, width, height);

            Texture2D cropped = CropTexture(texture, rect, true);
            PhotoDataManager.inst.AddPhotoOrigin(cropped);
        };
        DSLRManager.Instance.OnEndLoadAllTexture = () =>
        {
            List<Texture2D> originalPics = PhotoDataManager.inst.photoOrigin;
            if(originalPics.Count >= 8)
            {
                _isReady = true;
            }
        };

        DSLRManager.Instance.LoadPhotoAll();
    }
    private Texture2D CropTexture(Texture2D sourceTexture, Rect cropRect, bool flip)
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

    IEnumerator Loading()
    {
        float t = 0;
        while (t < _loadingTime)
        {
            if (UserDataManager.inst.selectedContent != CONTENT_TYPE.AI_BEAUTY && _isReady)
                break;
            t += Time.deltaTime;
            time = t;

            if (t >= _curInterval)
            {
                if (_curStep < _loadingTexts.Length - 1)
                {
                    _curStep++;
                    if(_curInterval < _loadingTime)
                    {
                        _curInterval += _interval;
                    }
                    _loadingText.text = _loadingTexts[_curStep];
                }
            }
            yield return null;
        }

        //if(UserDataManager.inst.selectedContent != CONTENT_TYPE.AI_BEAUTY)
        //{
        //    //beauty가 아니라면 get image 기다리기
        //    yield return new WaitUntil(() => _isReady);
        //}


        while (!_isReady)
        {
            t += Time.deltaTime;
            yield return null;

            if (t >= 600)
            {
                CustomLogger.Log($"[Content : {UserDataManager.inst.selectedContent}] Get Converted files too long");
                GameManager.inst.SetDiffusionState(false);
                yield break;
            }
        }

        switch (UserDataManager.inst.selectedContent)
        {
            case CONTENT_TYPE.AI_CARTOON:
                (_pageController as PC_Main)?.ChangePage(PAGE_TYPE.PAGE_DECO_SELECT_PICS_CARTOON);
                break;
            case CONTENT_TYPE.AI_PROFILE:
                (_pageController as PC_Main)?.ChangePage(PAGE_TYPE.PAGE_DECO_SELECT_PICS_PROFILE);
                break;
            case CONTENT_TYPE.AI_BEAUTY:
                _pageController?.ChangePage(PAGE_TYPE.PAGE_DECO_SELECT_PICS_BEAUTY);
                break;
            case CONTENT_TYPE.WHAT_IF:
                (_pageController as PC_Main)?.ChangePage(PAGE_TYPE.PAGE_DECO_SELECT_PICS_WHAT_IF);
                break;
        }

    }

    IEnumerator CheckReady()
    {
        yield return new WaitUntil(() =>
        PhotoDataManager.inst.photoConverted.Count == PhotoDataManager.inst.photoCount);

        _isReady = true;
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
