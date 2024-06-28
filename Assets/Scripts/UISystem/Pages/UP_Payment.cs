using MPUIKIT;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vivestudios.UI;

public class UP_Payment : UP_BasePage
{
    [SerializeField]
    private bool _paymentRequired = false;

    [SerializeField]
    private TextMeshProUGUI _titleText;

    [SerializeField]
    private Button _prevBtn;
    [SerializeField]
    private Button _warnBtn;
    [SerializeField]
    private Button _nextBtn;
    [SerializeField]
    private Button _freeWarnBtn;
    [SerializeField]
    private Button _freeNextBtn;

    //[SerializeField]
    //private UC_StyleContent _styleContent;
    [SerializeField]
    private UC_ProfileContent _content;
    [SerializeField]
    private Image _timeMachineImg;
    [SerializeField]
    private TextMeshProUGUI _selectedContentText;
    [SerializeField]
    private Image _frameImg;
    [SerializeField]
    private Image _frameShadowImg;
    [SerializeField]
    private TextMeshProUGUI _selectedFrameText;
    [SerializeField]
    private TextMeshProUGUI _priceText;
    [SerializeField]
    private Toggle _agreeToggle;
    [SerializeField]
    private Toggle _childToggle;
    [SerializeField]
    private Button _detailBtn;

    [SerializeField]
    private string _alertTitle;
    [SerializeField]
    private string _alertString;

    //[SerializeField]
    //private Sprite[] _cartoonSprites;
    //[SerializeField]
    //private string[] _cartoonTitles;
    //[SerializeField]
    //private string[] _cartoonDescriptions;
    [SerializeField]
    private Sprite[] _frameSprites;
    [SerializeField]
    private Sprite[] _frameShadowSprites;

    [SerializeField]
    private TextMeshProUGUI _timeText;

    protected int _maxTime;
    protected int _failTime;
    private Coroutine _timerCoroutine = null;
    private Coroutine _failTimerCoroutine = null;

    public override void InitPage()
    {
        _maxTime = ConfigData.config.paymentPageTime;
        _failTime = ConfigData.config.paymentFailTime;
    }

    public override void BindDelegates()
    {
        (_pageController as PC_Main).OnPaymentChangeAction += (isOn) =>
        {
            _paymentRequired = isOn;
        };

        _prevBtn?.onClick.AddListener(OnClickPrev);
        _nextBtn?.onClick.AddListener(OnClickPayment);
        _warnBtn?.onClick.AddListener(OnClickWarning);
        _freeWarnBtn?.onClick.AddListener(OnClickWarning);
        _freeNextBtn?.onClick.AddListener(OnClickPayment);

        _detailBtn?.onClick.AddListener(OnClickDetail);
        _childToggle?.onValueChanged.AddListener(OnChildToggleChanged);
        _agreeToggle?.onValueChanged.AddListener(OnAgreeToggleChanged);
    }
    private void OnEnable()
    {
        if (!_pageController)
            return;

        SetContent();

        _frameImg.sprite = _frameSprites[(int)UserDataManager.inst.selectedFrame];
        _frameShadowImg.sprite = UserDataManager.inst.selectedFrame == FRAME_TYPE.FRAME_8 ? _frameShadowSprites[1] : _frameShadowSprites[0];
        _frameShadowImg.SetNativeSize();
        _selectedFrameText.text = UserDataManager.inst.curPicAmount.ToString("# 장");
        if (UserDataManager.inst.curPrice == 0)
        {
            _titleText.text = "선택한 내용을 확인한 후 촬영해주세요";
            _priceText.text = UserDataManager.inst.curPrice.ToString("무료");

            _freeNextBtn.gameObject.SetActive(false);
            _freeWarnBtn.gameObject.SetActive(true);
            _nextBtn.gameObject.SetActive(false);
            _warnBtn.gameObject.SetActive(false);
        }
        else
        {
            _titleText.text = "선택한 내용을 확인한 후 결제해주세요";
            _priceText.text = UserDataManager.inst.curPrice.ToString("#,###원");

            _freeNextBtn.gameObject.SetActive(false);
            _freeWarnBtn.gameObject.SetActive(false);
            _nextBtn.gameObject.SetActive(false);
            _warnBtn.gameObject.SetActive(true);
        }

        if (UserDataManager.inst.selectedContent == CONTENT_TYPE.AI_CARTOON)
        {
            ApiCall.Instance.RequestModel(CartoonManager.cartoons[UserDataManager.inst.selectedSubContentNum]);
        }

        _timerCoroutine = StartCoroutine(TimerRoutine());
    }

    private void OnDisable()
    {
        (_pageController as PC_Main).globalPage.OpenDim(false);
        (_pageController as PC_Main).globalPage.CloseAlertPopup();
        (_pageController as PC_Main)?.globalPage?.CloseToast();
    }

    private void OnChildToggleChanged(bool isSelected)
    {
        _childToggle.GetComponent<MPImage>().StrokeWidth = isSelected ? 0 : 2;
        GameManager.inst.SetChildPlaying(isSelected);
    }

    private void OnAgreeToggleChanged(bool isSelected)
    {
        _agreeToggle.GetComponent<MPImage>().StrokeWidth = isSelected ? 0 : 2;
        //_nextBtn.interactable = isSelected;

        if(UserDataManager.inst.curPrice != 0)
        {
            _nextBtn.gameObject.SetActive(isSelected);
            _warnBtn.gameObject.SetActive(isSelected ? false : true);

            _freeNextBtn.gameObject.SetActive(false);
            _freeWarnBtn.gameObject.SetActive(false);
        }
        else
        {
            _freeNextBtn.gameObject.SetActive(isSelected);
            _freeWarnBtn.gameObject.SetActive(isSelected ? false : true);

            _nextBtn.gameObject.SetActive(false);
            _warnBtn.gameObject.SetActive(false);
        }
    }

    private void OnClickPrev()
    {
        (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_SELECT_FRAME);
    }

    private void OnClickPayment()
    {
        if (_paymentRequired && UserDataManager.inst.curPrice != 0)
        {
            (_pageController as PC_Main).globalPage.OpenDim(true);

            StartCoroutine(PaymentModule.inst.PaymentRoutine(UserDataManager.inst.curPrice, OnPaycheckDone));
        }
        else
        {
            (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_CAUTION);
        }

        ResetTimer();
    }

    private void OnClickWarning()
    {
        (_pageController as PC_Main)?.globalPage?.OpenToast("필수 약관에 동의해주세요.", 5);
    }

    private void OnPaycheckDone(bool isSuccessed, string failMsg, bool showErrorPage)
    {
        if (isSuccessed)
        {
            (_pageController as PC_Main).globalPage.OpenDim(false);
            (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_CAUTION);
        }
        else
        {
            if(_failTimerCoroutine  != null)
            {
                StopCoroutine(_failTimerCoroutine);
            }
            _failTimerCoroutine = StartCoroutine(FailTimerRoutine());

            (_pageController as PC_Main).globalPage.OpenAlertPopup("카드 결제에 실패했습니다.", failMsg, () =>
            {
                StartTimer();
                if (showErrorPage)
                {
                    Debug.Log("[ ERROR ][ payment ] 카드 결제 실패");
                    GameManager.inst.SetPaymentReaderConnected(false);
                }
            });
            (_pageController as PC_Main).globalPage.OpenDim(false);
        }
    }

    private void OnClickDetail()
    {
        (_pageController as PC_Main).globalPage.OpenPrivacyPopup();
    }

    private void SetContent()
    {
        _timeMachineImg.gameObject.SetActive(UserDataManager.inst.selectedContent == CONTENT_TYPE.AI_BEAUTY);
        _content.gameObject.SetActive(UserDataManager.inst.selectedContent != CONTENT_TYPE.AI_BEAUTY);

        switch (UserDataManager.inst.selectedContent)
        {
            case CONTENT_TYPE.AI_CARTOON:
                _selectedContentText.text = StringCacheManager.inst.GetContentTitle(CONTENT_TYPE.AI_CARTOON);
                _content.SetThumbnail(ResourceCacheManager.inst.GetCartoonThumbnailSprite((CARTOON_TYPE)UserDataManager.inst.selectedSubContentNum));
                _content.SetTitle(StringCacheManager.inst.GetCartoonTitle((CARTOON_TYPE)UserDataManager.inst.selectedSubContentNum));
                _content.SetDescription(StringCacheManager.inst.GetCartoonDescription((CARTOON_TYPE)UserDataManager.inst.selectedSubContentNum));
                _content.SetGenderActive(false);
                break;
            case CONTENT_TYPE.AI_PROFILE:

                _selectedContentText.text = StringCacheManager.inst.GetProfileTitle(UserDataManager.inst.selectedProfileType);
                _content.SetThumbnail(ResourceCacheManager.inst.GetProfileThumbnailSprite(UserDataManager.inst.selectedProfileType));
                _content.SetTitle(StringCacheManager.inst.GetProfileTitle(UserDataManager.inst.selectedProfileType));
                _content.SetDescription(StringCacheManager.inst.GetProfileDescription(UserDataManager.inst.selectedProfileType));
                _content.SetGenderActive(false);
                break;
            case CONTENT_TYPE.AI_BEAUTY:
                _selectedContentText.text = StringCacheManager.inst.GetContentTitle(CONTENT_TYPE.AI_BEAUTY);
                _content.SetGenderActive(false);
                break;
            case CONTENT_TYPE.WHAT_IF:

                _selectedContentText.text = StringCacheManager.inst.GetContentTitle(CONTENT_TYPE.WHAT_IF);
                _content.SetThumbnail(ResourceCacheManager.inst.GetProfileThumbnailSprite(UserDataManager.inst.selectedProfileType), true);
                _content.SetTitle(StringCacheManager.inst.GetProfileTitle(UserDataManager.inst.selectedProfileType));
                _content.SetDescription(StringCacheManager.inst.GetProfileDescription(UserDataManager.inst.selectedProfileType));
                _content.SetGenderActive(true);
                _content.SetGender(UserDataManager.inst.selectedGender);
                break;
            default:
                _selectedContentText.text = StringCacheManager.inst.GetContentTitle(CONTENT_TYPE.AI_CARTOON);
                break;
        }
    }

    public void StartTimer()
    {
        ResetTimer();
        _timerCoroutine = StartCoroutine(TimerRoutine());
    }

    public void ResetTimer()
    {
        if (_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
        }
        if(_failTimerCoroutine != null)
        {
            StopCoroutine(_failTimerCoroutine);
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

        (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_AOD);
    }

    private IEnumerator FailTimerRoutine()
    {
        int time = 0;

        while (time < _failTime)
        {
            yield return new WaitForSecondsRealtime(1);
            time++;
        }

        (_pageController as PC_Main).globalPage.CloseAlertPopup();
        StartTimer();
    }

    public override void OnPageEnable()
    {
    }

    public override void OnPageDisable()
    {
        _childToggle.isOn = ConfigData.config.childDefaultCheck;
        _agreeToggle.isOn = ConfigData.config.policyDefaultCheck;

        OnChildToggleChanged(_childToggle.isOn);
        OnAgreeToggleChanged(_agreeToggle.isOn);
    }

    protected override void OnPageReset()
    {
        _childToggle.isOn = ConfigData.config.childDefaultCheck;
        _agreeToggle.isOn = ConfigData.config.policyDefaultCheck;

        OnChildToggleChanged(_childToggle.isOn);
        OnAgreeToggleChanged(_agreeToggle.isOn);
    }
}
