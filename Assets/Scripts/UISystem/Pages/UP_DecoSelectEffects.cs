using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vivestudios.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class UP_DecoSelectEffects : UP_DecoratePageBase
{
    [SerializeField]
    private List<UC_SelectableContent> _contents;
    [SerializeField]
    private RectTransform _skinTransform;

    [SerializeField]
    private Image _skinToggleImage;
    [SerializeField]
    private Toggle _skinToggle;
    [SerializeField]
    private TextMeshProUGUI _skinDescription;
    [SerializeField]
    private Sprite _toggleOn;
    [SerializeField]
    private Sprite _toggleOff;

    [SerializeField]
    private Button _prevBtn;
    [SerializeField]
    private Button _nextBtn;

    [SerializeField]
    private GameObject stickerContainerParent;
    [SerializeField]
    private GameObject stickerContainer;

    // Filter Data
    [SerializeField]
    private Transform _filterContainer;
    [SerializeField]
    private GameObject _filterPrefab;

    private bool _isSkinUsed = false;
    private bool _isSorting = false;
    private UC_SelectableContent _selectedFilter = null;

    protected const int DISABLE_STROKE_SIZE = 2;

    public override void InitPage()
    {
        base.InitPage();
    }

    public override void BindDelegates()
    {
        base.BindDelegates();

        _prevBtn.onClick.AddListener(OnClickPrev);
        _nextBtn.onClick.AddListener(OnClickNext);

        //for (int i = 0; i < _contents.Count; i++)
        //{
        //    int index = i;
        //    _contents[i].pointerDownAction += () => OnClickFilter(index);
        //}

        _skinToggle.onValueChanged.AddListener(OnChangeSkin);

        (pageController as PC_Main).StickerUpdateAction += UpdateSticker;
    }

    private void UpdateSticker()
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
        switch (UserDataManager.inst.selectedContent)
        {

            case CONTENT_TYPE.AI_CARTOON:
                if (UserDataManager.Instance.IsStickerUser)
                {
                    (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_DECO_SELECT_STICKER);
                }
                else
                {
                    (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_DECO_SELECT_PICS_CARTOON);
                }
                break;
            case CONTENT_TYPE.AI_PROFILE:
                _pageController.ChangePage(PAGE_TYPE.PAGE_DECO_SELECT_PICS_PROFILE);
                break;
            case CONTENT_TYPE.AI_BEAUTY:
                _pageController.ChangePage(PAGE_TYPE.PAGE_DECO_SELECT_PICS_BEAUTY);
                break;
            case CONTENT_TYPE.WHAT_IF:
                _pageController.ChangePage(PAGE_TYPE.PAGE_DECO_SELECT_PICS_WHAT_IF);
                break;
            case CONTENT_TYPE.AI_CARICATURE:
                _pageController.ChangePage(PAGE_TYPE.PAGE_DECO_SELECT_PICS_CARICATURE);
                break;
        }
    }

    private void OnClickNext()
    {
        (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_DECO_SELECT_FRAME);
    }

    private void OnClickFilter(UC_SelectableFilter selected)
    {
        _selectedFilter = selected;
        UserDataManager.inst.SetLutEffect(selected.Key);

        for (int i = 0; i < _contents.Count; i++)
        {
            _contents[i].Select(_contents[i] == selected);
        }

        (_pageController as PC_Main).UpdateFrame();
    }

    private void OnChangeSkin(bool isOn)
    {
        _skinToggleImage.sprite = isOn ? _toggleOn : _toggleOff;
        (_pageController as PC_Main).SkinFilterOn(isOn);
        (_pageController as PC_Main).UpdateFrame();
    }

    protected override void OnTimeLimitDone()
    {
        if (gameObject.activeInHierarchy)
        {
            (_pageController as PC_Main).StopTimeLimit();
            (_pageController as PC_Main).StartTimeLimit(10);
            (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_DECO_SELECT_FRAME);
        }
    }

    private void CreateContent()
    {
        FilterData.FilterData filterData = AdminManager.Instance.FilterData;

        _isSkinUsed = bool.Parse(filterData.Config.BilateralDefaultCheck.ToLower());
        _isSorting = filterData.Config.Sorting.ToLower() == StringCacheManager.inst.SortingSpecified ? true : false;

        if (_isSorting)
        {
            for (int i = 1; i <= filterData.FilterTable.Count; i++)
            {
                FilterData.FilterTableEntry entry = filterData.OrderedFilterTable[i];
                GameObject filterObj = Instantiate(_filterPrefab, _filterContainer);
                UC_SelectableFilter filter = filterObj.GetComponent<UC_SelectableFilter>();
                filter.SetThumbnail(entry.Thumbnail_data);
                filter.SetNameText(entry.Korean);
                filter.SetlutTex(entry.LutFile_data);
                filter.SetKey(entry.Key);
                filter.pointerDownAction += () => OnClickFilter(filter);

                if(_selectedFilter == null)
                {
                    _selectedFilter = filter;
                }
                _contents.Add(filter);
            }
        }
        else
        {
            foreach (var item in filterData.FilterTable)
            {
                GameObject filterObj = Instantiate(_filterPrefab, _filterContainer);
                UC_SelectableFilter filter = filterObj.GetComponent<UC_SelectableFilter>();
                filter.SetThumbnail(item.Value.Thumbnail_data);
                filter.SetNameText(item.Value.Korean);
                filter.SetlutTex(item.Value.LutFile_data);
                filter.SetKey(item.Value.Key);
                filter.pointerDownAction += () => OnClickFilter(filter);

                if (_selectedFilter == null)
                {
                    _selectedFilter = filter;
                }
                _contents.Add(filter);
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

        //bilateral 필터 사용여부 정리되면 다시 진행하기
        _skinTransform.gameObject.SetActive(false);

        bool originalContains = false;
        switch (UserDataManager.inst.selectedContent)
        {
            case CONTENT_TYPE.AI_CARTOON:
                foreach (var elem in PhotoDataManager.inst.selectedPhoto.Values)
                {
                    if (elem == PHOTO_TYPE.REAL)
                    {
                        originalContains = true;
                    }
                }
                if (originalContains)
                {
                    _skinTransform.gameObject.SetActive(true);
                    _skinDescription.text = StringCacheManager.inst.GetFilterDescription(CONTENT_TYPE.AI_CARTOON);
                }
                else
                {
                    _skinTransform.gameObject.SetActive(false);
                }
                break;
            case CONTENT_TYPE.AI_PROFILE:
                break;
            case CONTENT_TYPE.AI_TIME_MACHINE:
                break;
            case CONTENT_TYPE.AI_BEAUTY:
                foreach (var elem in PhotoDataManager.inst.selectedPhoto.Values)
                {
                    if (elem == PHOTO_TYPE.REAL)
                    {
                        originalContains = true;
                    }
                }
                if (originalContains)
                {
                    _skinTransform.gameObject.SetActive(true);
                    _skinDescription.text = StringCacheManager.inst.GetFilterDescription(CONTENT_TYPE.AI_BEAUTY);
                }
                else
                {
                    _skinTransform.gameObject.SetActive(false);
                }
                break;
            case CONTENT_TYPE.AI_CARICATURE:
                break;
            case CONTENT_TYPE.WHAT_IF:
                break;
            default:
                break;
        }

        if (_skinTransform.gameObject.activeSelf)
        {
            _skinTransform.gameObject.SetActive(bool.Parse(AdminManager.inst.FilterData.Config.UseBilateral));
            if (bool.Parse(AdminManager.inst.FilterData.Config.UseBilateral))
            {
                (_pageController as PC_Main).SkinFilterOn(bool.Parse(AdminManager.inst.FilterData.Config.BilateralDefaultCheck));
                _skinToggle.isOn = (_pageController as PC_Main).isSkinFilterOn;
            }
        }

        if ((_pageController as PC_Main).timeLimitDone == true)
        {
            _prevBtn.interactable = false;
        }
        else
        {
            _prevBtn.interactable = true;
        }

        if(_selectedFilter != null)
        {
            _selectedFilter.pointerDownAction();
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

        if(_contents != null && _contents.Count > 0)
        {
            _selectedFilter = _contents[0];
        }
    }
}
