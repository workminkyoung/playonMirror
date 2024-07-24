using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vivestudios.UI;
using HSVPicker;
using UnityEngine.EventSystems;

public class UC_ChromaKeySetting : UC_BaseComponent, IPointerClickHandler
{
    [SerializeField]
    private Slider _dSlider;
    [SerializeField]
    private TextMeshProUGUI _dValueText;

    [Space(10)]
    [SerializeField]
    private Slider _tSlider;
    [SerializeField]
    private TextMeshProUGUI _tValueText;

    [Space(10)]
    [SerializeField]
    private Slider _blurSlider;
    [SerializeField]
    private TextMeshProUGUI _blurValueText;

    [Space(10)]
    [SerializeField]
    private Slider _alphaPowSlider;
    [SerializeField]
    private TextMeshProUGUI _alphaPowValueText;

    [Space(10)]
    [SerializeField]
    private Slider _alphaEdgeSlider;
    [SerializeField]
    private TextMeshProUGUI _alphaEdgeValueText;

    [Space(10)]
    [SerializeField]
    private Button _prevBtn;
    [SerializeField]
    private Button _nextBtn;
    [SerializeField]
    private RawImage _testBgImg;

    [SerializeField]
    private Button _changeColorBtn;
    [SerializeField]
    private ColorPicker _colorPicker;

    [SerializeField]
    private Button _resetBtn;
    [SerializeField]
    private Button _defaultBtn;

    [SerializeField]
    private Button _saveBtn;
    [SerializeField]
    private Button _closeBtn;

    [SerializeField]
    private RawImage _previewImage;

    [SerializeField]
    private Texture _testBg;

    private int _optionIndex = 0;
    private int _imageIndex = 0;

    private bool _isInited = false;

    private const float DEFAULT_D = 0.5f;
    private const float DEFAULT_T = 0.05f;
    private const int DEFAULT_BLUR = 2;
    private const float DEFAULT_ALPHA_POW = 3;
    private const float DEFAULT_ALPHA_EDGE = 0;
    private const string DEFAULT_COLOR = "#00FF00";

    public override void InitComponent ()
    {
        BindDelegates();
        _colorPicker.gameObject.SetActive(false);
        _isInited = true;
        ResetChromaKey();
        UpdateBG();
    }

    private void BindDelegates ()
    {
        Debug.Log(ConfigData.config.camType);
        if(ConfigData.config.camType == (int)CAMERA_TYPE.DSLR)
        {
            DSLRManager.inst.OnLoadPreview += OnLoadPreview;
        }

        _prevBtn.onClick.AddListener(PrevBG);
        _nextBtn.onClick.AddListener(NextBG);

        _changeColorBtn.onClick.AddListener(OpenColorPicker);
        _colorPicker.onValueChanged.AddListener(ChangeColor);

        _dSlider.onValueChanged.AddListener(value => UpdateChromaKeyTemporary());
        _tSlider.onValueChanged.AddListener(value => UpdateChromaKeyTemporary());
        _blurSlider.onValueChanged.AddListener(value => UpdateChromaKeyTemporary());
        _alphaPowSlider.onValueChanged.AddListener(value => UpdateChromaKeyTemporary());
        _alphaEdgeSlider.onValueChanged.AddListener(value => UpdateChromaKeyTemporary());

        _resetBtn.onClick.AddListener(ResetChromaKey);
        _defaultBtn.onClick.AddListener(SetAsDefault);
        _saveBtn.onClick.AddListener(Save);
        _closeBtn.onClick.AddListener(Close);
    }

    private void OnLoadPreview (Texture2D tex)
    {
        if(gameObject.activeInHierarchy)
        {
            ChromaKeyModule.inst.SetCamImg(tex);
            _previewImage.texture = ChromaKeyModule.inst.resultRT;
        }
    }

    private void NextBG ()
    {
        Texture curImg = ChromaKeyModule.inst.options[_optionIndex].images[_imageIndex];
        if(_imageIndex + 1 >= ChromaKeyModule.inst.options[_optionIndex].images.Length)
        {
            if(_optionIndex + 1 >= ChromaKeyModule.inst.options.Length)
            {
                _optionIndex = 0;
                _imageIndex = 0;
            }
            else
            {
                _optionIndex++;
                _imageIndex = 0;
            }
        }
        else
        {
            _imageIndex++;
        }

        if(curImg == ChromaKeyModule.inst.options[_optionIndex].images[_imageIndex])
        {
            NextBG();
        }
        UpdateBG();
    }

    private void PrevBG ()
    {
        Texture curImg = ChromaKeyModule.inst.options[_optionIndex].images[_imageIndex];
        if(_imageIndex - 1 < 0)
        {
            if(_optionIndex - 1 < 0)
            {
                _optionIndex = ChromaKeyModule.inst.options.Length - 1;
                _imageIndex = ChromaKeyModule.inst.options[_optionIndex].images.Length - 1;
            }
            else
            {
                _optionIndex--;
                _imageIndex = ChromaKeyModule.inst.options[_optionIndex].images.Length - 1;
            }
        }
        else
        {
            _imageIndex--;
        }
        if(curImg == ChromaKeyModule.inst.options[_optionIndex].images[_imageIndex])
        {
            PrevBG();
        }
        UpdateBG();
    }

    private void UpdateBG ()
    {
        _testBgImg.texture = ChromaKeyModule.inst.options[_optionIndex].images[_imageIndex];
        ChromaKeyModule.inst.SetBg(ChromaKeyModule.inst.options[_optionIndex].images[_imageIndex]);
        _colorPicker.gameObject.SetActive(false);
    }

    private void OpenColorPicker ()
    {
        _colorPicker.gameObject.SetActive(true);
    }

    private void ChangeColor (Color color)
    {
        _changeColorBtn.image.color = color;

        UpdateChromaKeyTemporary();
    }

    private void ResetChromaKey ()
    {
        ChromaKeyModule.inst.ResetChromaKeySetting();

        _dSlider.value = ConfigData.config.chromaKey.d;
        _tSlider.value = ConfigData.config.chromaKey.t;
        _blurSlider.value = ConfigData.config.chromaKey.blur;
        _alphaPowSlider.value = ConfigData.config.chromaKey.alphaPow;
        _alphaEdgeSlider.value = ConfigData.config.chromaKey.alphaEdge;
        _changeColorBtn.image.color = UtilityExtensions.HexToColor(ConfigData.config.chromaKey.color);
        _colorPicker.AssignColor(UtilityExtensions.HexToColor(ConfigData.config.chromaKey.color));

        UpdateChromaKeyTemporary();
        _colorPicker.gameObject.SetActive(false);
    }

    private void SetAsDefault ()
    {
        _dSlider.value = DEFAULT_D;
        _tSlider.value = DEFAULT_T;
        _blurSlider.value = DEFAULT_BLUR;
        _alphaPowSlider.value = DEFAULT_ALPHA_POW;
        _alphaEdgeSlider.value = DEFAULT_ALPHA_EDGE;
        _changeColorBtn.image.color = UtilityExtensions.HexToColor(DEFAULT_COLOR);
        _colorPicker.AssignColor(UtilityExtensions.HexToColor(DEFAULT_COLOR));

        UpdateChromaKeyTemporary();

        ChromaKeyModule.inst.TemporaryApplySetting(DEFAULT_D, DEFAULT_T, DEFAULT_BLUR, DEFAULT_ALPHA_POW, DEFAULT_ALPHA_EDGE, DEFAULT_COLOR);
        _colorPicker.gameObject.SetActive(false);
    }

    private void Save ()
    {
        ChromaKeyModule.inst.ChangeChromaKeyConfig(_dSlider.value, _tSlider.value, (int)_blurSlider.value, _alphaPowSlider.value, _alphaEdgeSlider.value, UtilityExtensions.ColorToHex(_changeColorBtn.image.color));
        _colorPicker.gameObject.SetActive(false);
    }

    private void Close ()
    {
        ResetChromaKey();
        gameObject.SetActive(false);
    }

    private void UpdateChromaKeyTemporary ()
    {
        _dValueText.text = _dSlider.value.ToString("0.00");
        _tValueText.text = _tSlider.value.ToString("0.00");
        _blurValueText.text = _blurSlider.value.ToString();
        _alphaPowValueText.text = _alphaPowSlider.value.ToString("0.00");
        _alphaEdgeValueText.text = _alphaEdgeSlider.value.ToString("0.00");

        ChromaKeyModule.inst.TemporaryApplySetting(_dSlider.value, _tSlider.value, (int)_blurSlider.value, _alphaPowSlider.value, _alphaEdgeSlider.value, UtilityExtensions.ColorToHex(_changeColorBtn.image.color));
    }

    private void OnEnable ()
    {
        if(_isInited == false)
        {
            return;
        }

        switch(ConfigData.config.camType)
        {
            case (int)CAMERA_TYPE.WEBCAM:
                CameraManager.inst.PlayWebcam();
                ChromaKeyModule.inst.SetCamImg(CameraManager.inst.webCamTexture);
                _previewImage.texture = ChromaKeyModule.inst.resultRT;
                break;
            case (int)CAMERA_TYPE.NDI:
                break;
            case (int)CAMERA_TYPE.DSLR:
                DSLRManager.Instance.StartEVF();
                break;
        }

        ResetChromaKey();
        UpdateBG();
    }

    private void OnDisable ()
    {
        if(_isInited == false)
        {
            return;
        }

        switch(ConfigData.config.camType)
        {
            case (int)CAMERA_TYPE.WEBCAM:
                CameraManager.inst.StopWebcam();
                _previewImage.texture = null;
                break;
            case (int)CAMERA_TYPE.NDI:
                break;
            case (int)CAMERA_TYPE.DSLR:
                DSLRManager.Instance.EndEVF();
                _previewImage.texture = null;
                break;
        }

        _colorPicker.gameObject.SetActive(false);
        ChromaKeyModule.inst.SetCamImg(null);
    }

    public void OnPointerClick (PointerEventData eventData)
    {
        Debug.Log(eventData);

        if(_colorPicker.gameObject.activeInHierarchy == false)
        {
            return;
        }

        Vector2 localMousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((_colorPicker.gameObject.transform as RectTransform), eventData.position, eventData.pressEventCamera, out localMousePos);

        if((_colorPicker.gameObject.transform as RectTransform).rect.Contains(localMousePos))
        {
            return;
        }

        _colorPicker.gameObject.SetActive(false); 
    }
}
