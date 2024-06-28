using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Vivestudios.UI;

public class UP_SelectCartoonStyle : UP_BaseSelectContent, IPageTimeLimit
{
    [SerializeField]
    private Button _descriptionBtn;
    [SerializeField]
    private string _popupTitle;
    [SerializeField]
    private string _popupDescription;
    [SerializeField]
    private GameObject _styleContent;
    [SerializeField]
    private HorizontalLayoutGroup _contentParent;
    [SerializeField]
    private Button _prevBtn;

    [SerializeField]
    private CARTOON_TYPE[] _activeCartoonTypes;

    public int MaxTime { get => _maxTime; set => _maxTime = value; }
    public TextMeshProUGUI timeText { get => _timeText; set => _timeText = value; }

    public override void InitPage()
    {
        CreatStyleContent();
        _maxTime = ConfigData.config.cartoonStyleSelectTime;

        for (int i = 0; i < _activeCartoonTypes.Length; i++)
        {
            int index = i;

            (_contents[i] as UC_StyleContent).SetThumbnail(ResourceCacheManager.inst.GetCartoonThumbnailSprite(_activeCartoonTypes[index]));
            (_contents[i] as UC_StyleContent).SetTitle(StringCacheManager.inst.GetCartoonTitle(_activeCartoonTypes[index]));
            (_contents[i] as UC_StyleContent).SetDescription(StringCacheManager.inst.GetCartoonDescription(_activeCartoonTypes[index]));
        }
    }

    public override void BindDelegates()
    {
        for (int i = 0; i < _contents.Length; i++)
        {
            int index = (int)_activeCartoonTypes[i];
            _contents[i].pointerClickAction += () => OnClickContent(index);
        }

        _prevBtn?.onClick.AddListener(() => _pageController.ChangePage(PAGE_TYPE.PAGE_SELECT_CONTENT));
        _descriptionBtn?.onClick.AddListener(OnClickDescription);

        (_pageController as PC_Main).OnShuffleAction += ShuffleContents;
    }

    protected override void OnClickContent(int index)
    {
        UserDataManager.inst.SelectSubContent(index);
        _pageController.ChangePage(PAGE_TYPE.PAGE_SELECT_FRAME);
    }

    private void CreatStyleContent()
    {
        List<UC_SelectableContent> contents = new List<UC_SelectableContent>();
        for (int i = 0; i < _activeCartoonTypes.Length; i++)
        {
            GameObject content = Instantiate(_styleContent, _contentParent.transform);
            UC_StyleContent styleContent = content.GetComponentInChildren<UC_StyleContent>();
            styleContent.InitComponent();
            contents.Add(styleContent);
            _contentParents.Add(styleContent.transform.parent);
        }

        _contents = contents.ToArray();
        _shuffledContentParents = _contentParents.ToList();
    }

    private void OnClickDescription()
    {
        (_pageController as PC_Main).globalPage.OpenConfirmPopup(_popupTitle, _popupDescription, ResourceCacheManager.inst.cartoonPopupThumbnailSprite);
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
