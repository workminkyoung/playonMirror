using ChromakeyFrameData;
using MPUIKIT;
using Newtonsoft.Json;
using RotaryHeart.Lib.SerializableDictionary;
using System;
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

    [SerializeField]
    private Sprite[] _frameSprites;
    [SerializeField]
    private Sprite[] _frameShadowSprites;

    [SerializeField]
    private TextMeshProUGUI _timeText;
    [SerializeField]
    private GameObject _bgTheme;
    [SerializeField]
    private FrameTypeTextureDic _bgThemeImageDict;

    [SerializeField]
    private Tuple<string, string> selectedContent;
    [SerializeField]
    private Button _openKeyboardBtn;

    [SerializeField]
    private GameObject _couponObj;
    [SerializeField]
    private TextMeshProUGUI _couponText;
    [SerializeField]
    private TextMeshProUGUI _couponWarnText;

    private int _couponUsedPrice = 0;
    protected int _maxTime;
    protected int _failTime;
    private Coroutine _timerCoroutine = null;
    private Coroutine _failTimerCoroutine = null;

    public override void InitPage()
    {
        foreach (var item in _bgThemeImageDict)
        {
            item.Value.Init();
        }
    }

    public override void ApplyAdminData()
    {
        base.ApplyAdminData();
        _maxTime = AdminManager.Instance.BasicSetting.Config.PayConfirm_data;
        _failTime = AdminManager.Instance.BasicSetting.Config.PaymentFailTime;
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
        _openKeyboardBtn?.onClick.AddListener((_pageController as PC_Main).globalPage.OpenKeyboard);
        (_pageController as PC_Main).globalPage.SetKeyboardCloseAction(UpdateCoupon);
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

        if(UserDataManager.inst.curPrice != 0 || (UserDataManager.inst.getCouponAvailable && _couponUsedPrice != 0))
        {
            _nextBtn.gameObject.SetActive(isSelected);
            _warnBtn.gameObject.SetActive(isSelected ? false : true);

            _freeNextBtn.gameObject.SetActive(false);
            _freeWarnBtn.gameObject.SetActive(false);
        }
        else if(UserDataManager.inst.curPrice == 0 || (UserDataManager.inst.getCouponAvailable && _couponUsedPrice == 0))
        {
            _freeNextBtn.gameObject.SetActive(isSelected);
            _freeWarnBtn.gameObject.SetActive(isSelected ? false : true);

            _nextBtn.gameObject.SetActive(false);
            _warnBtn.gameObject.SetActive(false);
        }
    }

    private void OnClickPrev()
    {
        // TODO : page enable로 바꿔야할 필요가 있을까? 고민해보기
        UserDataManager.inst.InitCouponData();
        _openKeyboardBtn.gameObject.SetActive(true);
        _couponObj.SetActive(false);
        _couponWarnText.gameObject.SetActive(false);
        if (UserDataManager.inst.isChromaKeyOn)
        {
            (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_SELECT_CHROMA_KEY_BACKGROUND);
        }
        else
        {
            (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_SELECT_FRAME);
        }
    }

    //TODO : userdata에 price 할인 금액으로 바꿔주기
    private void OnClickPayment()
    {
        if (UserDataManager.inst.getCouponAvailable)
        {
            if (_paymentRequired && _couponUsedPrice != 0)
            {
                (_pageController as PC_Main).globalPage.OpenDim(true);
                StartCoroutine(PaymentModule.inst.PaymentRoutine(_couponUsedPrice, OnPaycheckDone));
            }
            else
            {
                //(_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_CAUTION); // 결제모듈 미사용 기존코드
                OnSuccessedPayment(); // 결제모듈 미사용 임시코드
            }
        }
        else
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
        }

        ResetTimer();
    }

    private void OnClickWarning()
    {
        (_pageController as PC_Main)?.globalPage?.OpenToast("필수 약관에 동의해주세요.", 5);
    }

    private void OnSuccessedPayment()
    {
        var data = new Dictionary<string, string>
        {
            { "coupon_number", UserDataManager.inst.getCouponNumber},
            //{ "uuid", "vive1" }, // 테스트용 쿠폰 전용 UUID 
            { "uuid", LogDataManager.inst.GetGuid}, // 실제 사용할 코드
            { "status", "used" } 
        };
        string json = JsonConvert.SerializeObject(data);
        string url = ApiCall.inst.CouponAPIUrl;
        ApiCall.inst.Patch(url, json, (string response) => 
        {
            //쿠폰 적용시 적용 이후 가격으로 업데이트
            UserDataManager.inst.SetPrice(_couponUsedPrice);
            (_pageController as PC_Main).globalPage.OpenDim(false);
            (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_CAUTION);
        });
    }

    private void OnPaycheckDone(bool isSuccessed, string failMsg, bool showErrorPage)
    {
        if (isSuccessed)
        {
            if (UserDataManager.inst.getCouponAvailable)
            {
                OnSuccessedPayment();
            }
            else
            {
                (_pageController as PC_Main).globalPage.OpenDim(false);
                (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_CAUTION);
            }
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
                    CustomLogger.LogError("[ payment ] 카드 결제 실패");
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

        _bgTheme.SetActive(false);
        switch (UserDataManager.inst.selectedContent)
        {
            case CONTENT_TYPE.AI_CARTOON:
                _selectedContentText.text = AdminManager.Instance.ServiceData.Contents[UserDataManager.Instance.selectedContentKey].Korean_Title;
                _content.SetThumbnail(AdminManager.Instance.ServiceData.ContentsDetail[UserDataManager.Instance.selectedSubContentKey].Thumbnail_data);
                _content.SetTitle(AdminManager.Instance.ServiceData.ContentsDetail[UserDataManager.Instance.selectedSubContentKey].Korean_Title);
                _content.SetDescription(AdminManager.Instance.ServiceData.ContentsDetail[UserDataManager.Instance.selectedSubContentKey].Korean_SubText);
                _content.SetGenderActive(false);
                break;
            case CONTENT_TYPE.AI_PROFILE:
                _selectedContentText.text = AdminManager.Instance.ServiceData.Contents[UserDataManager.Instance.selectedContentKey].Korean_Title;
                _content.SetThumbnail(AdminManager.Instance.ServiceData.ContentsDetail[UserDataManager.Instance.selectedSubContentKey].Thumbnail_data);
                _content.SetTitle(AdminManager.Instance.ServiceData.ContentsDetail[UserDataManager.Instance.selectedSubContentKey].Korean_Title);
                _content.SetDescription(AdminManager.Instance.ServiceData.ContentsDetail[UserDataManager.Instance.selectedSubContentKey].Korean_SubText);
                _content.SetGenderActive(false);
                break;
            case CONTENT_TYPE.AI_BEAUTY:
                _selectedContentText.text = AdminManager.Instance.ServiceData.Contents[UserDataManager.Instance.selectedContentKey].Korean_Title;
                // TODO : 하드코딩된 부분 나중에 수정하기
                _timeMachineImg.sprite = AdminManager.Instance.ServiceData.ContentsDetail["BT0000"].Thumbnail_data;
                _content.SetGenderActive(false);
                break;
            case CONTENT_TYPE.WHAT_IF:
                _selectedContentText.text = AdminManager.Instance.ServiceData.Contents[UserDataManager.Instance.selectedContentKey].Korean_Title;
                _content.SetThumbnail(AdminManager.Instance.ServiceData.ContentsDetail[UserDataManager.Instance.selectedSubContentKey].Thumbnail_data, true);
                _content.SetTitle("");
                _content.SetDescription("");
                _content.SetGenderActive(false);
                //_content.SetGender(UserDataManager.inst.selectedGender);
                break;
            case CONTENT_TYPE.AI_CARICATURE:
                _selectedContentText.text = AdminManager.Instance.ServiceData.Contents[UserDataManager.Instance.selectedContentKey].Korean_Title;
                _content.SetThumbnail(AdminManager.Instance.ServiceData.ContentsDetail[UserDataManager.Instance.selectedSubContentKey].Thumbnail_data, true);
                _content.SetTitle("");
                _content.SetDescription("");
                _content.SetGenderActive(false);
                //_content.SetGender(UserDataManager.inst.selectedGender);
                break;
            default:
                _selectedContentText.text = StringCacheManager.inst.GetContentTitle(CONTENT_TYPE.AI_CARTOON);
                break;
        }
    }

    private void UpdateCoupon()
    {
        //가격 업데이트
        if (UserDataManager.inst.getCouponAvailable)
        {
            CouponValidataResponse coupon = UserDataManager.inst.getvalidataResponse;
            if (coupon != null)
            {
                if (coupon.is_valid_number)
                {
                    _couponObj.SetActive(true);
                    _couponText.text = UserDataManager.inst.curPrice.ToString("#,###원");
                    if (coupon.is_fixed_rate)
                    {
                        _couponUsedPrice = UserDataManager.inst.curPrice - UserDataManager.inst.curPrice * coupon.amount / 100;
                    }
                    else
                    {
                        _couponUsedPrice = UserDataManager.inst.curPrice - coupon.amount;
                    }

                    ConvertContentFree();
                }
                _openKeyboardBtn.gameObject.SetActive(false);
                _couponWarnText.gameObject.SetActive(true);
            }
        }
    }

    private void SetBGTexture()
    {
        //if (!_bgTheme.activeSelf)
        //{
        //    return;
        //}

        _bgTheme.SetActive(true);

        foreach (var item in _bgThemeImageDict)
        {
            item.Value.gameObject.SetActive(item.Key == UserDataManager.Instance.selectedFrameType);
        }

        ImageOrderedDic orderedTexture = new ImageOrderedDic();
        switch (UserDataManager.inst.selectedContent)
        {
            case CONTENT_TYPE.AI_CARTOON:
                orderedTexture = AdminManager.Instance.ChromakeyFrame.ChromakeyFrameTable[UserDataManager.Instance.selectedChromaKey].orderedImage;
                break;
            case CONTENT_TYPE.AI_BEAUTY:
                orderedTexture = AdminManager.Instance.ChromakeyFrame.ChromakeyToneTable[UserDataManager.Instance.selectedChromaKey].orderedImage;
                break;
            default:
                break;
        }

        _bgThemeImageDict[UserDataManager.Instance.selectedFrameType].SetTextures(orderedTexture);
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
        UserDataManager.inst.InitCouponData();
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
        if (!_pageController)
            return;

        //버튼 디폴트상태 트루일때

        SetContent();
        if (UserDataManager.Instance.isChromaKeyOn)
        {
            SetBGTexture();
        }
        else
        {
            _bgTheme.SetActive(false);
        }

        _childToggle.gameObject.SetActive(AdminManager.Instance.BasicSetting.Config.Age14TermUse);
        _agreeToggle.gameObject.SetActive(AdminManager.Instance.BasicSetting.Config.PaymentTermUse);

        selectedContent = new Tuple<string, string>(UserDataManager.inst.selectedFrameKey, UserDataManager.inst.selectedContentKey);

        _frameImg.sprite = AdminManager.Instance.FrameData.CommonTuple[selectedContent].ThumbnailUnselect_data;
        _frameShadowImg.sprite = UserDataManager.inst.selectedFrameDefinition.FrameType == FRAME_TYPE.FRAME_8 ? _frameShadowSprites[1] : _frameShadowSprites[0];
        _frameShadowImg.SetNativeSize();
        _selectedFrameText.text = UserDataManager.inst.curPicAmount.ToString("# 장");
        ConvertContentFree();

        if (GameManager.Instance.isAdminDownloadSuccess)
        {
            if (_childToggle.gameObject.activeSelf)
            {
                _childToggle.isOn = AdminManager.Instance.BasicSetting.Config.Age14TermUsed;
            }
            if (_agreeToggle.gameObject.activeSelf)
            {
                _agreeToggle.isOn = AdminManager.Instance.BasicSetting.Config.PaymentTermUsed;
            }
        }

        OnChildToggleChanged(_childToggle.isOn);
        OnAgreeToggleChanged(_agreeToggle.isOn);

        _timerCoroutine = StartCoroutine(TimerRoutine());
    }
    
    private void ConvertContentFree()
    {
        if (UserDataManager.inst.getCouponAvailable)
        {
            if (_couponUsedPrice == 0)
            {
                _titleText.text = "선택한 내용을 확인한 후 촬영해주세요";
                _priceText.text = "무료";

                if (_agreeToggle.isOn)
                {
                    _freeNextBtn.gameObject.SetActive(true);
                    _freeWarnBtn.gameObject.SetActive(false);
                    _nextBtn.gameObject.SetActive(false);
                    _warnBtn.gameObject.SetActive(false);
                }
                else
                {
                    _freeNextBtn.gameObject.SetActive(false);
                    _freeWarnBtn.gameObject.SetActive(true);
                    _nextBtn.gameObject.SetActive(false);
                    _warnBtn.gameObject.SetActive(false);
                }
            }
            else
            {
                _titleText.text = "선택한 내용을 확인한 후 결제해주세요";
                _priceText.text = _couponUsedPrice.ToString("#,###원");

                if (_agreeToggle.isOn)
                {
                    _freeNextBtn.gameObject.SetActive(false);
                    _freeWarnBtn.gameObject.SetActive(false);
                    _nextBtn.gameObject.SetActive(true);
                    _warnBtn.gameObject.SetActive(false);
                }
                else
                {
                    _freeNextBtn.gameObject.SetActive(false);
                    _freeWarnBtn.gameObject.SetActive(false);
                    _nextBtn.gameObject.SetActive(false);
                    _warnBtn.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            if (UserDataManager.inst.curPrice == 0)
            {
                _titleText.text = "선택한 내용을 확인한 후 촬영해주세요";
                _priceText.text = "무료";

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
        }
    }

    public override void OnPageDisable()
    {
        //if (_childToggle.gameObject.activeSelf)
        //{
        //    _childToggle.isOn = AdminManager.Instance.BasicSetting.Config.Age14TermUsed;
        //}
        //if (_agreeToggle.gameObject.activeSelf)
        //{
        //    _agreeToggle.isOn = AdminManager.Instance.BasicSetting.Config.PaymentTermUsed;
        //}

        //OnChildToggleChanged(_childToggle.isOn);
        //OnAgreeToggleChanged(_agreeToggle.isOn);
    }

    private void OnDisable()
    {
        (_pageController as PC_Main).globalPage.OpenDim(false);
        (_pageController as PC_Main).globalPage.CloseAlertPopup();
        (_pageController as PC_Main)?.globalPage?.CloseToast();
        (_pageController as PC_Main).globalPage.ClosePrivacyPopup();
        (_pageController as PC_Main).globalPage.CloseKeyboard();

        // TODO : 리펙토링 대상, 타이머 0이 되어 첫페이지로 초기화될때 onreset함수 등록해주는 부분 구현필요
        UserDataManager.inst.InitCouponData();
        _openKeyboardBtn.gameObject.SetActive(true);
        _couponObj.SetActive(false);
        _couponWarnText.gameObject.SetActive(false);
    }

    protected override void OnPageReset()
    {
        UserDataManager.inst.InitCouponData();
        _openKeyboardBtn.gameObject.SetActive(true);
        _couponObj.SetActive(false);
        _couponWarnText.gameObject.SetActive(false);
    }

    [Serializable]
    public class FrameTypeTextureDic : SerializableDictionaryBase<FRAME_TYPE, UC_FrameMask> { }
}
