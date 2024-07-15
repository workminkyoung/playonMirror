using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Vivestudios.UI;

public class UP_SelectProfilePic : UP_BasePage
{
    private int _maxTime;
    [SerializeField]
    protected TextMeshProUGUI _timeText;

    [SerializeField]
    private UC_SelectableContent[] _selectableContents;
    [SerializeField]
    private Button _nextBtn;
    [SerializeField]
    private GameObject _imageShield;

    private Coroutine _timerCoroutine = null;
    private Coroutine _delayCoroutine = null;
    private int _width, _height;
    private int _loadCountMax = 3;
    private int _loadCount = 0;

    public override void InitPage()
    {
        _selectableContents = GetComponentsInChildren<UC_SelectableContent>(true);

        _maxTime = ConfigData.config.profilePicSelectTime;
    }

    public override void BindDelegates()
    {
        for (int i = 0; i < _selectableContents.Length; i++)
        {
            int index = i;
            _selectableContents[i].pointerDownAction += () => OnClickContent(index);
        }

        _nextBtn.onClick.AddListener(OnClickNext);
    }

    private void OnClickContent(int index)
    {
        UserDataManager.inst.SelectProfilePic(index);

        for (int i = 0; i < _selectableContents.Length; i++)
        {
            _selectableContents[i].Select(i == index);

            if (i == index)
            {
                if (_selectableContents[i].thumbnailImg.sprite != null)
                {
                    PhotoDataManager.inst.SetSelectedAIProfile(_selectableContents[i].thumbnailImg.sprite.texture);
                }
            }
        }

        _nextBtn.interactable = true;
    }


    private void OnClickNext()
    {
        if (_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
        }

        (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_LOADING);
    }

    private void OnEnable()
    {
        if (!_pageController)
            return;

        _imageShield.SetActive(true);
        _loadCount = 0; 
        _nextBtn.interactable = false;
        _delayCoroutine = StartCoroutine(DelayOnLoad());// SetContents();
        _timerCoroutine = StartCoroutine(TimerRoutine());

        for (int i = 0; i < _selectableContents.Length; i++)
        {
            _selectableContents[i]?.SetThumbnailClear(Color.clear);
            _selectableContents[i].Select(false);
        }
    }

    void FindCaptureSize()
    {
        switch (UserDataManager.inst.selectedContent)
        {
            case CONTENT_TYPE.AI_CARTOON:
                _width = PlayOnProperties.crop4x3_width;
                _height = PlayOnProperties.crop4x3_height;
                break;
            case CONTENT_TYPE.AI_PROFILE:
                _width = PlayOnProperties.crop3x4_width;
                _height = PlayOnProperties.crop3x4_height;
                break;
            case CONTENT_TYPE.AI_TIME_MACHINE:
                _width = PlayOnProperties.crop3x4_width;
                _height = PlayOnProperties.crop3x4_height;
                break;
            case CONTENT_TYPE.WHAT_IF:
                _width = PlayOnProperties.crop3x4_width;
                _height = PlayOnProperties.crop3x4_height;
                break;
            default:
                break;
        }
    }

    private IEnumerator DelayOnLoad()
    {
        yield return new WaitForSecondsRealtime(1);
        SetContents();
    }

    private void SetContents()
    {
        _loadCount++;
        if (_loadCount > _loadCountMax)
        {
            Debug.Log("[ ERROR ] " + this.name + " Photo not Loaded all");
            DSLRManager.inst.ErrorOnCamera();
            return;
        }
        if (ConfigData.config.camType == (int)CAMERA_TYPE.DSLR)
        {
            FindCaptureSize();
            //load photo
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
                //전부다 로드 완료했을때

                Debug.Log("Try To Load Photo" + _loadCount);
                if(PhotoDataManager.inst.photoOrigin.Count < 3)
                {
                    PhotoDataManager.inst.SetPhotoOrigin(new List<Texture2D>());
                    SetContents();
                    return;
                }

                _imageShield.SetActive(false);
                List<Texture2D> originalPics = PhotoDataManager.inst.photoOrigin;

                for (int i = 0; i < originalPics.Count; i++)
                {
                    _selectableContents[i]?.SetThumbnailClear(Color.white);
                    _selectableContents[i]?.SetThumbnail(TextureToSprite(originalPics[i]));
                }

                Debug.Log("select default image");
                _selectableContents[0].Select(true);
                UserDataManager.inst.SelectProfilePic(0);
                if (_selectableContents[0].thumbnailImg.sprite != null)
                {
                    PhotoDataManager.inst.SetSelectedAIProfile(_selectableContents[0].thumbnailImg.sprite.texture);
                }
            };
            DSLRManager.Instance.LoadPhotoAll();
        }
        else
        {
            List<Texture2D> originalPics = PhotoDataManager.inst.photoOrigin;

            for (int i = 0; i < originalPics.Count; i++)
            {
                _selectableContents[i]?.SetThumbnail(TextureToSprite(originalPics[i]));
            }
        }
    }

    private Sprite TextureToSprite(Texture2D tex)
    {
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }

    public void ResetTimer()
    {
        if (_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
        }
        if (_delayCoroutine != null)
        {
            StopCoroutine(_delayCoroutine);
        }
    }

    private IEnumerator TimerRoutine()
    {
        _timeText.text = _maxTime.ToString();

        int time = 0;

        while (time < _maxTime)
        {
            yield return new WaitForSecondsRealtime(1);
            time++;

            _timeText.text = (_maxTime - time).ToString();

            if (_maxTime - time == 5)
            {
                (_pageController as PC_Main)?.globalPage?.OpenTimerToast(5);
            }
        }

        (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_LOADING);
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

    public override void OnPageEnable()
    {
    }

    public override void OnPageDisable()
    {
        (_pageController as PC_Main)?.globalPage?.CloseToast();
        if(_delayCoroutine != null)
        {
            StopCoroutine(_delayCoroutine);
        }
    }

    protected override void OnPageReset()
    {
        if (!_pageController)
            return;
        OnClickContent(0);

        for (int i = 0; i < _selectableContents.Length; i++)
        {
            _selectableContents[i].SetThumbnail(null);
        }

        Debug.LogFormat("[CLEAR UI] {0} pictures", name);
    }
}
