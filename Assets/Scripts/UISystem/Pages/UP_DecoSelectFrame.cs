using System;
using System.Collections.Generic;
//using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
using Vivestudios.UI;

public class UP_DecoSelectFrame : UP_DecoratePageBase
{
    //[SerializeField]
    //private UC_SelectableContent[] _frameColors;
    //[SerializeField]
    //private RectTransform _frameShapeArea;

    [SerializeField]
    private List<UC_SelectableContent> _frameColors;
    [SerializeField]
    private Button _prevBtn;
    [SerializeField]
    private Button _printBtn;

    [SerializeField]
    private GameObject stickerContainerParent;
    [SerializeField]
    private GameObject stickerContainer;

    [SerializeField]
    private GameObject _colorPrefab;
    [SerializeField]
    private Transform _colorContainer;

    // QR 인화 Toggle
    [SerializeField]
    private Image _qrToggleImage;
    [SerializeField]
    private Toggle _qrToggle;
    [SerializeField]
    private Sprite _toggleOn;
    [SerializeField]
    private Sprite _toggleOff;
    [SerializeField]
    private bool _isQRUse = false;
    [SerializeField]
    private bool _qrDefaultUsed = false;

    private UC_SelectableContent _selectedColor = null;

    public override void BindDelegates()
    {
        base.BindDelegates();

        _prevBtn.onClick.AddListener(OnClickPrev);
        _printBtn.onClick.AddListener(OnClickPrint);
        _qrToggle.onValueChanged.AddListener(OnChangeQRToggle);

        (pageController as PC_Main).StickerUpdateAction += UpdateSticker;
    }

    private void OnChangeQRToggle(bool isOn)
    {
        if (!_isQRUse)
            return;

        _qrToggleImage.sprite = isOn ? _toggleOn : _toggleOff;
        UserDataManager.Instance.SetIsQRPrint(isOn);
    }

    private void UpdateSticker ()
    {
        if(stickerContainer != null)
        {
            Destroy(stickerContainer);
        }
        stickerContainer = GameObject.Instantiate((pageController as PC_Main).stickerContainerPrefab, stickerContainerParent.transform);
        foreach(var elem in stickerContainer.GetComponentsInChildren<UC_StickerController>())
        {
            elem.HideController();
            Destroy(elem);
        }
    }

    private void OnClickPrev()
    {
        (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_DECO_SELECT_EFFECT);
    }

    private void OnClickPrint()
    {
        (_pageController as PC_Main).StopTimeLimit();
        (_pageController as PC_Main).SetResultTextures(_frameAreaDic[UserDataManager.inst.selectedFrameType].GetResultPics());
        (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_PRINT);
    }

    protected override void OnTimeLimitDone()
    {
        if (gameObject.activeInHierarchy)
        {
            OnClickPrint();
        } 
    }

    private void OnClickColor(string key)
    {
        UserDataManager.inst.SetSelectedFrameColor(key);
        Tuple<string, string> tupleKey = new Tuple<string, string>(UserDataManager.Instance.selectedContentKey, UserDataManager.Instance.selectedFrameColor);
        UserDataManager.Instance.SetSelectedFrameDefinition(AdminManager.Instance.FrameData.DefinitionTuple[UserDataManager.inst.selectedFrameKey][tupleKey]);

        foreach (var color in _frameColors)
        {
            color.Select(key == color.Key);
        }

        (_pageController as PC_Main).UpdateFrame();
    }

    private void CreateContent()
    {
        // Check Used
        _isQRUse = bool.Parse(AdminManager.Instance.BasicSetting.Config.OptionalUse.ToLower());
        _qrDefaultUsed = bool.Parse(AdminManager.Instance.BasicSetting.Config.DefaultUsed.ToLower());

        // Create Frame Color
        if (AdminManager.Instance.FrameData.Theme.Sorting.ToLower() == StringCacheManager.inst.SortingSpecified)
        {
            for (int i = 1; i <= AdminManager.Instance.FrameData.Theme.OrderedColorCode.Count; i++)
            {
                FrameData.ColorCodeEntry colorCode = AdminManager.Instance.FrameData.Theme.OrderedColorCode[i];
                GameObject colorObj = Instantiate(_colorPrefab, _colorContainer);
                UC_SelectableContent color = colorObj.GetComponent<UC_SelectableContent>();
                color.SetThumbnail(colorCode.Thumbnail_data);
                color.SetNameText(colorCode.korean);
                color.SetKey(colorCode.key);
                color.pointerDownAction += () => OnClickColor(colorCode.key);

                _frameColors.Add(color);
            }
        }
        else
        {
            foreach (var item in AdminManager.Instance.FrameData.Theme.ColorCode)
            {
                if (item.Value.Use)
                {
                    GameObject colorObj = Instantiate(_colorPrefab, _colorContainer);
                    UC_SelectableContent color = colorObj.GetComponent<UC_SelectableContent>();
                    color.SetThumbnail(item.Value.Thumbnail_data);
                    color.SetNameText(item.Value.korean);
                    color.SetKey(item.Value.key);
                    color.pointerDownAction += () => OnClickColor(item.Value.key);

                    _frameColors.Add(color);
                }
            }
        }
        _isContentCreated = true;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (!_isContentCreated)
        {
            CreateContent();
        }
        if (!_pageController)
        {
            return;
        }

        if ((_pageController as PC_Main).timeLimitDone == true)
        {
            _prevBtn.interactable = false;
        }
        else
        {
            _prevBtn.interactable = true;
        }

        _qrToggle.isOn = _qrDefaultUsed;
        _qrToggle.gameObject.SetActive(_isQRUse);
        UserDataManager.Instance.SetIsQRPrint(_qrDefaultUsed);

        for (int i = 0; i < _frameColors.Count; i++)
        {
            if (_frameColors[i].Key == UserDataManager.Instance.selectedFrameColor)
            {
                _frameColors[i].pointerDownAction();
            }
        }
    }

    public override void OnPageEnable()
    {
    }

    public override void OnPageDisable()
    {
    }

    protected override void OnPageReset()
    {
        if(stickerContainer != null)
        {
            Destroy(stickerContainer);
        }

    }
}
