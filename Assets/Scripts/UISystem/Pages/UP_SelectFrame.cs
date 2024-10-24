using FrameData;
using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vivestudios.UI;

public class UP_SelectFrame : UP_BaseSelectContent, IPageTimeLimit
{
    [SerializeField]
    private Button _prevBtn;
    [SerializeField]
    private Button _nextBtn;

    [SerializeField]
    private Button _plusBtn;
    [SerializeField]
    private Image _plusIcon;
    [SerializeField]
    private TextMeshProUGUI _amountText;
    [SerializeField]
    private Button _minusBtn;
    [SerializeField]
    private Image _minusIcon;
    [SerializeField]
    private TextMeshProUGUI _priceText;
    [SerializeField]
    private TextMeshProUGUI _discountedPriceText;
    [SerializeField]
    private Image _discountLine;
    [SerializeField]
    private RectTransform _priceArea;
    [SerializeField]
    private RectTransform _freeText;
    [SerializeField]
    private RectTransform _sumArea;

    [SerializeField]
    private GameObject _framePrefab;
    [SerializeField]
    private Transform _frameContainer;
    [SerializeField]
    private Transform _tempFrameContainer;
    [SerializeField]
    private FrameKeyDic _allFrameDic = new FrameKeyDic();
    [SerializeField]
    private FramePriceDic _allPriceDic = new FramePriceDic();
    [SerializeField]
    private List<string> selectableFrameKeys = new List<string>();
    [SerializeField]
    private string _defaultFramekey;

    private Vector2 _priceAreaOriginPos;
    private int _curAmount = 1;
    private int _maxPrintAmount;
    private int[] _originalPrices;
    private int[] _discountPrices;
    private FrameData.FrameData _frameData;
    private bool _isSorting = false;
    [SerializeField]
    private bool _isChromakeyUse = false;

    private readonly Color BTN_BACK_DISABLE_COLOR = new Color(0.88f, 0.88f, 0.88f);
    private readonly Color BTN_BACK_ENABLE_COLOR = new Color(0.74f, 0.74f, 0.74f);
    private readonly Color BTN_ICON_DISABLE_COLOR = new Color(0.74f, 0.74f, 0.74f);
    private readonly Color BTN_ICON_ENABLE_COLOR = Color.white;

    public TextMeshProUGUI timeText { get => _timeText; set => _timeText = value; }
    public int MaxTime { get => _maxTime; set => _maxTime = value; }

    public override void InitPage()
    {
        base.InitPage();

        _priceAreaOriginPos = _priceArea.anchoredPosition;
    }

    public override void ApplyAdminData()
    {
        base.ApplyAdminData();
        _maxTime = AdminManager.Instance.BasicSetting.Config.FrameSelect_data;//ConfigData.config.frameSelectTime;
    }

    public override void BindDelegates()
    {
        base.BindDelegates();

        //for (int i = 0; i < _contents.Length; i++)
        //{
        //    int index = i;
        //    _contents[i].pointerDownAction += () => OnTouchContent(index);
        //}

        _prevBtn?.onClick.AddListener(OnClickPrev);
        _nextBtn?.onClick.AddListener(OnClickNext);

        _plusBtn.onClick.AddListener(OnClickPlus);
        _minusBtn.onClick.AddListener(OnClickMinus);
    }

    protected override void ShuffleContents()
    {
        base.ShuffleContents();

        ResetContents();
        CurAmountCheck();
    }

    private void OnClickPrev()
    {
        switch (UserDataManager.inst.selectedContent)
        {
            case CONTENT_TYPE.AI_CARTOON:
                _pageController.ChangePage(PAGE_TYPE.PAGE_SELECT_CARTOON_STYLE);
                break;
            case CONTENT_TYPE.AI_PROFILE:
                _pageController.ChangePage(PAGE_TYPE.PAGE_SELECT_AI_PROFILE);
                break;
            case CONTENT_TYPE.AI_TIME_MACHINE:
                _pageController.ChangePage(PAGE_TYPE.PAGE_SELECT_CONTENT);
                break;
            case CONTENT_TYPE.AI_BEAUTY:
                if (UserDataManager.inst.isSingleContent)
                {
                    _pageController.ChangePage(PAGE_TYPE.PAGE_AOD);
                }
                else
                {
                    _pageController.ChangePage(PAGE_TYPE.PAGE_SELECT_CONTENT);
                }
                break;
            case CONTENT_TYPE.WHAT_IF:
                _pageController.ChangePage(PAGE_TYPE.PAGE_SELECT_WHAT_IF);
                break;
            case CONTENT_TYPE.AI_CARICATURE:
                _pageController.ChangePage(PAGE_TYPE.PAGE_SELECT_WHAT_IF);
                break;
            default:
                _pageController.ChangePage(PAGE_TYPE.PAGE_SELECT_CONTENT);
                break;
        }
    }

    private void OnClickNext()
    {
        switch (UserDataManager.inst.selectedFrameType)
        {
            case FRAME_TYPE.FRAME_4:
                PhotoDataManager.inst.SetLandscape(false);
                break;
            case FRAME_TYPE.FRAME_1:
                PhotoDataManager.inst.SetLandscape(false);
                break;
            default:
                PhotoDataManager.inst.SetLandscape(true);
                break;
        }

        switch(UserDataManager.inst.selectedContent)
        {
            case CONTENT_TYPE.AI_CARTOON:
                UserDataManager.inst.SetChromaKeyEnable(_isChromakeyUse);
                break;
            case CONTENT_TYPE.AI_BEAUTY:
                UserDataManager.inst.SetChromaKeyEnable(_isChromakeyUse);
                break;
            default:
                UserDataManager.inst.SetChromaKeyEnable(false);
                break;
        }

        if(UserDataManager.inst.isChromaKeyOn)
        {
            (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_SELECT_CHROMA_KEY_BACKGROUND);
        }
        else
        {
            (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_PAYMENT);
        }

    }

    private void ResetContents()
    {
        // Activate Frame Object
        foreach (var item in _allFrameDic)
        {
            item.Value.gameObject.SetActive(false);
        }

        for (int i = 0; i < selectableFrameKeys.Count; i++)
        {
            _allFrameDic[selectableFrameKeys[i]].transform.parent = _frameContainer;
            _allFrameDic[selectableFrameKeys[i]].gameObject.SetActive(true);

            // in case frame data changes
            Tuple<string, string> tupleKey = new Tuple<string, string>(selectableFrameKeys[i], UserDataManager.inst.selectedContentKey);
            CommonEntry commonData = _frameData.CommonTuple[tupleKey];
            _allFrameDic[selectableFrameKeys[i]].SetThumbnail(commonData.ThumbnailSelect_data, commonData.ThumbnailUnselect_data);
            _allFrameDic[selectableFrameKeys[i]].SetKey(selectableFrameKeys[i]);

            PriceConfig priceConfig = new PriceConfig();
            priceConfig.originalPrices = commonData.originPrice_datas;
            priceConfig.discountPrices = commonData.discountPrice_datas;
            priceConfig.priceNum = priceConfig.originalPrices.Length;

            _allPriceDic[selectableFrameKeys[i]] = priceConfig;
        }

        if (!string.IsNullOrEmpty(_defaultFramekey))
        {
            _allFrameDic[_defaultFramekey].pointerDownAction.Invoke();
        }
    }

    private void OnClickPlus()
    {
        _curAmount = Mathf.Clamp(_curAmount + 1, 1, Mathf.Min(_maxPrintAmount, PhotoPaperCheckModule.GetRemainPhotoPaper()));
        CurAmountCheck();
        CurPriceCheck();
    }

    private void OnClickMinus()
    {
        _curAmount = Mathf.Clamp(_curAmount - 1, 1, Mathf.Min(_maxPrintAmount, PhotoPaperCheckModule.GetRemainPhotoPaper()));
        CurAmountCheck();
        CurPriceCheck();
    }

    private void CurAmountCheck()
    {
        _amountText.text = _curAmount.ToString();

        if (_curAmount == 1)
        {
            _minusBtn.targetGraphic.color = BTN_BACK_DISABLE_COLOR;
            _minusIcon.color = BTN_ICON_DISABLE_COLOR;
        }
        else
        {
            _minusBtn.targetGraphic.color = BTN_BACK_ENABLE_COLOR;
            _minusIcon.color = BTN_ICON_ENABLE_COLOR;
        }

        if (_curAmount == Mathf.Min(_maxPrintAmount, PhotoPaperCheckModule.GetRemainPhotoPaper()))
        {
            _plusBtn.targetGraphic.color = BTN_BACK_DISABLE_COLOR;
            _plusIcon.color = BTN_ICON_DISABLE_COLOR;
        }
        else
        {
            _plusBtn.targetGraphic.color = BTN_BACK_ENABLE_COLOR;
            _plusIcon.color = BTN_ICON_ENABLE_COLOR;
        }
    }
    private void CurPriceCheck()
    {
        if (!_pageController)
            return;

        _priceArea.anchoredPosition = _priceAreaOriginPos;
        _freeText.gameObject.SetActive(false);
        _sumArea.gameObject.SetActive(true);

        if (_originalPrices[_curAmount - 1] > _discountPrices[_curAmount - 1])
        {
            _discountedPriceText.gameObject.SetActive(true);
            _discountLine.gameObject.SetActive(true);
            _priceText.text = _discountPrices[_curAmount - 1].ToString("#,###") + "원";
            _discountedPriceText.text = _originalPrices[_curAmount - 1].ToString("#,###") + "원";

            LayoutRebuilder.ForceRebuildLayoutImmediate(_discountedPriceText.rectTransform);
            _discountLine.rectTransform.sizeDelta = new Vector2(_discountedPriceText.rectTransform.sizeDelta.x, 8);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_discountLine.rectTransform);
            UserDataManager.inst.SetPicAmount(_curAmount);
            UserDataManager.inst.SetPrice(_discountPrices[_curAmount - 1]);
        }
        else
        {
            if(_originalPrices[_curAmount - 1] == 0)
            {
                _priceArea.anchoredPosition = new Vector2(_priceAreaOriginPos.x +115, _priceAreaOriginPos.y);
                _freeText.gameObject.SetActive(true);
                _sumArea.gameObject.SetActive(false);
            }
            else
            {
                _priceText.text = _originalPrices[_curAmount - 1].ToString("#,###") + "원";
                _discountedPriceText.gameObject.SetActive(false);
                _discountLine.gameObject.SetActive(false);
                _discountedPriceText.text = string.Empty;
            }
            UserDataManager.inst.SetPicAmount(_curAmount);
            UserDataManager.inst.SetPrice(_originalPrices[_curAmount - 1]);
        }
    }

    public override void OnPageEnable()
    {
        if (!_isContentCreated)
        {
            CreateContent();
        }

        _defaultFramekey = _frameData.ServiceFrame.Code[UserDataManager.Instance.selectedContentKey].SelectFrame1;
        selectableFrameKeys = _frameData.ServiceFrame.Code[UserDataManager.Instance.selectedContentKey].SelectFrames;
        _curAmount = _frameData.ServiceFrame.Code[UserDataManager.Instance.selectedContentKey].DefaultSellAmount;

        ResetContents();

        // set price by reset content data
        PriceConfig priceConfig = _allPriceDic[selectableFrameKeys[0]];

        _maxPrintAmount = priceConfig.priceNum;
        _originalPrices = new int[_maxPrintAmount];
        _discountPrices = new int[_maxPrintAmount];


        for (int i = 0; i < priceConfig.originalPrices.Length; i++)
        {
            if (i < _originalPrices.Length)
            {
                _originalPrices[i] = priceConfig.originalPrices[i];
            }
        }

        for (int i = 0; i < priceConfig.discountPrices.Length; i++)
        {
            if (i < _discountPrices.Length)
            {
                _discountPrices[i] = priceConfig.discountPrices[i];
            }
        }

        CurAmountCheck();
        CurPriceCheck();
    }

    //여기서부터 다시 진행
    public void CreateContent()
    {
        _isSorting = AdminManager.Instance.FrameData.ServiceFrame.Sorting.ToLower() == StringCacheManager.Instance.SortingSpecified;
        _frameData = AdminManager.Instance.FrameData;
        _isChromakeyUse = bool.Parse(AdminManager.Instance.ChromakeyFrame.Config["UsePage"].value1.ToLower());
        
        foreach (var item in _frameData.Common.Code)
        {
            GameObject frameObj = Instantiate(_framePrefab, _tempFrameContainer);
            UC_SelectableContent frame = frameObj.GetComponent<UC_SelectableContent>();
            FRAME_TYPE type = FRAME_TYPE.FRAME_1;
            switch (item.Key)
            {
                case StringCacheManager.frame1:
                    type = FRAME_TYPE.FRAME_1;
                    break;
                case StringCacheManager.frame2:
                    type = FRAME_TYPE.FRAME_2;
                    break;
                case StringCacheManager.frame4:
                    type = FRAME_TYPE.FRAME_4;
                    break;
                case StringCacheManager.frame8:
                    type = FRAME_TYPE.FRAME_8;
                    break;
                default:
                    break;
            }
            frame.pointerDownAction += () => OnTouchContent(item.Key, type);
            _allFrameDic.Add(item.Key, frame);
            _allPriceDic.Add(item.Key, null);
        }
        _isContentCreated = true;
    }

    private void OnTouchContent(string key, FRAME_TYPE type)
    {
        foreach (var item in _allFrameDic)
        {
            if (item.Value.gameObject.activeSelf)
            {
                item.Value.Select(item.Value.Key ==  key);
            }
        }

        PriceConfig priceConfig = _allPriceDic[key];

        _maxPrintAmount = priceConfig.priceNum;
        //_curAmount = ConfigData.config.firstPrintAmount;
        _originalPrices = _allPriceDic[key].originalPrices; //new int[_maxPrintAmount];
        _discountPrices = _allPriceDic[key].discountPrices;//new int[_maxPrintAmount];

        CurPriceCheck();

        UserDataManager.inst.SelectFrameKey(key);
        UserDataManager.inst.SelectFrameType(type);

        // Set Defalt Frame Definition
        Tuple<string, string> tupleKey = new Tuple<string, string>(UserDataManager.Instance.selectedContentKey, UserDataManager.Instance.defaultFrameColor);
        UserDataManager.Instance.SetSelectedFrameDefinition(_frameData.DefinitionTuple[UserDataManager.inst.selectedFrameKey][tupleKey]);
    }

    public override void OnPageDisable()
    {
        foreach(var item in _allFrameDic)
        {
            item.Value.transform.parent = _tempFrameContainer;
        }
    }

    protected override void OnPageReset()
    {
        //_curAmount = UserDataManager.inst.curPicAmount;
    }

    [Serializable]
    public class FrameKeyDic : SerializableDictionaryBase<string, UC_SelectableContent> { }

    [Serializable]
    public class FramePriceDic : SerializableDictionaryBase<string, PriceConfig> { }

}