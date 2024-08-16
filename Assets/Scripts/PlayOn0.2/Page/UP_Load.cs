using Microsoft.SqlServer.Server;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Vivestudios.UI;

public class UP_Load : UP_BasePage
{
    // TODO : load base 만들어서 상속받아서 페이지 두개로 찢기
    // 기본 로딩화면과 event 로딩화면 구분
    [SerializeField]
    private GameObject _defaultLoad;
    [SerializeField]
    private GameObject _eventLoad;

    // 기본 로딩화면 데이터
    [SerializeField]
    private TextMeshProUGUI _loadingTitle;
    [SerializeField]
    private TextMeshProUGUI _loadingSubtitle;
    [SerializeField]
    private RectTransform _loadingFill;
    [SerializeField]
    private Image _loadingIcon;

    // 이벤트 로딩화면 데이터
    [SerializeField]
    private RawImage _eventLoadingRawImage;
    [SerializeField] 
    private TextMeshProUGUI _eventLoadingTitle;
    [SerializeField]
    private VideoPlayer _eventVideoPlayer;
    [SerializeField]
    private Slider _eventFill;
    private RenderTexture _eventVideoRenderTexture = null;

    [SerializeField]
    private bool _isReady = false;

    private ShootingScreenData.ShootScreenEntry _shootEntry;
    private int _loadingTime;
    private int _curStep = 0;
    private float _curInterval;
    private float _interval;
    private float _fillMax;
    private LOAD_TYPE _loadType;

    public float time;

    public override void InitPage()
    {
        _fillMax = _loadingFill.sizeDelta.x;
        _eventVideoRenderTexture = new RenderTexture(
            (int)_eventLoadingRawImage.rectTransform.sizeDelta.x, (int)_eventLoadingRawImage.rectTransform.sizeDelta.y, 16);
        _eventVideoRenderTexture.Create();
    }

    public override void BindDelegates()
    {
    }

    private void CreateContent()
    {
        _shootEntry = AdminManager.inst.ShootScreen[UserDataManager.inst.selectedContentKey];
    }

    void RequestAIProfile()
    {
        ProfileModule.inst.GetProfileImages(PhotoDataManager.inst.selectedAIProfile,
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
            }
            else
            {
                PhotoDataManager.inst.AddPhotoOrigin(cropped);
            }
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

    IEnumerator DefaultLoading()
    {
        float t = 0;
        float point = 0;
        float fill = 0;
        _loadingIcon.sprite = _shootEntry.url_datas[_shootEntry.url_orderdKey[_curStep]];
        while (t < _loadingTime)
        {
            if (UserDataManager.inst.selectedContent != CONTENT_TYPE.AI_BEAUTY && _isReady)
                break;
            t += Time.deltaTime;
            time = t;

            fill = UtilityExtensions.Remap(t, 0, _loadingTime, 0, _fillMax);
            point = UtilityExtensions.Remap(t, 0, _loadingTime, _loadingFill.anchoredPosition.x, _loadingFill.anchoredPosition.x + _fillMax);

            if (!float.IsNaN(point) && !float.IsInfinity(point))
                _loadingIcon.rectTransform.anchoredPosition = new Vector2(point, _loadingIcon.rectTransform.anchoredPosition.y);
            if (!float.IsNaN(fill) && !float.IsInfinity(fill) && fill > 1)
                _loadingFill.sizeDelta = new Vector2(fill, _loadingFill.sizeDelta.y);

            if (t >= _curInterval)
            {
                if (_curStep < _shootEntry.korean.Count - 1)
                {
                    _curStep++;
                    if(_curInterval < _loadingTime)
                    {
                        _curInterval += _interval;
                    }
                    _loadingSubtitle.text = _shootEntry.korean[_curStep];
                    _loadingIcon.sprite = _shootEntry.url_datas[_shootEntry.url_orderdKey[_curStep]];
                }
            }
            yield return null;
        }

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

    IEnumerator EventLoading()
    {
        float t = 0;
        float normalT = 0;

        while (t < _loadingTime)
        {
            if (UserDataManager.inst.selectedContent != CONTENT_TYPE.AI_BEAUTY && _isReady)
                break;
            t += Time.deltaTime;
            time = t;
            normalT = UtilityExtensions.Remap(t, 0, _loadingTime, 0, 1);
            if( normalT > 0.0002f)
            {
                _eventFill.value = normalT;
            }

            yield return null;
        }

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
        CreateContent();

        DSLRManager.Instance.EndEVF();
        DSLRManager.Instance.CloseSession();

        _defaultLoad.SetActive(false);
        _eventLoad.SetActive(false);

        if (!string.IsNullOrEmpty(_shootEntry.ConversionVideo_path))
        {
            // Event Video Load
            LoadEventVideoPage();
            StartCoroutine(EventLoading());
        }
        else if(_shootEntry.ConversionImage_data != null)
        {
            // Event Image Load
            LoadEventImagePage();
            StartCoroutine(EventLoading());
        }
        else
        {
            // Default Load
            LoadDefaultPage();
            StartCoroutine(DefaultLoading());
        }

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
                //_loadingTime = ConfigData.config.loadingTimeBeauty;
                LoadBeautyPhotos();
                break;
            case CONTENT_TYPE.WHAT_IF:
                RequestWhatIf();
                break;
            default:
                break;
        }

    }

    private void LoadDefaultPage()
    {
        _defaultLoad.SetActive(true);
        _loadType = LOAD_TYPE.DEFAULT;

        _loadingTime = int.Parse(_shootEntry.ConversionTime);
        _loadingFill.sizeDelta = new Vector2(0, _loadingFill.sizeDelta.y);
        _interval = _loadingTime / _shootEntry.url_datas.Count;
        _isReady = false;
        //switch (UserDataManager.inst.selectedContent)
        //{
        //    case CONTENT_TYPE.AI_CARTOON:
        //        StartCoroutine(CheckReady());
        //        break;
        //    case CONTENT_TYPE.AI_PROFILE:
        //        RequestAIProfile();
        //        break;
        //    case CONTENT_TYPE.AI_TIME_MACHINE:
        //        _isReady = true;
        //        break;
        //    case CONTENT_TYPE.AI_BEAUTY:
        //        _loadingTime = ConfigData.config.loadingTimeBeauty;
        //        LoadBeautyPhotos();
        //        break;
        //    case CONTENT_TYPE.WHAT_IF:
        //        RequestWhatIf();
        //        break;
        //    default:
        //        break;
        //}

        _curStep = 0;
        _curInterval = _interval;
    }

    private void LoadEventVideoPage()
    {
        _eventLoad.SetActive(true);
        _loadType = LOAD_TYPE.EVENT_VIDEO;
        _loadingTime = int.Parse(_shootEntry.ConversionTime);
        _isReady = false;
        _eventFill.value = 0;

        _eventLoadingRawImage.texture = _eventVideoRenderTexture;
        _eventVideoPlayer.targetTexture = _eventVideoRenderTexture;
        _eventVideoPlayer.source = VideoSource.Url;
        _eventVideoPlayer.url = _shootEntry.ConversionVideo_path;
        _eventVideoPlayer.Play();
    }

    private void LoadEventImagePage()
    {
        _eventLoad.SetActive(true);
        _loadType = LOAD_TYPE.EVENT_IMAGE;
        _loadingTime = int.Parse(_shootEntry.ConversionTime);
        _isReady = false;
        _eventFill.value = 0;

        _eventLoadingRawImage.texture = _shootEntry.ConversionImage_data;
    }

    public override void OnPageDisable()
    {
        if(_loadType == LOAD_TYPE.EVENT_VIDEO)
        {
            _eventVideoPlayer.Stop();
        }
    }

    protected override void OnPageReset()
    {
    }

    enum LOAD_TYPE
    {
        EVENT_VIDEO = 0,
        EVENT_IMAGE,
        DEFAULT
    }
}
