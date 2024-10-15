using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    private HorizontalLayoutGroup _contentParentRow1;
    [SerializeField]
    private HorizontalLayoutGroup _contentParentRow2;
    [SerializeField]
    private Button _prevBtn;
    private Sprite _guideImage;

    [SerializeField]
    private CARTOON_TYPE[] _activeCartoonTypes;

    public int MaxTime { get => _maxTime; set => _maxTime = value; }
    public TextMeshProUGUI timeText { get => _timeText; set => _timeText = value; }

    public override void InitPage()
    {
    }

    public override void ApplyAdminData()
    {
        base.ApplyAdminData();
        _maxTime = AdminManager.Instance.BasicSetting.Config.CAMenu_data;//ConfigData.config.cartoonStyleSelectTime;
    }

    public override void BindDelegates()
    {
        for (int i = 0; i < _contents.Length; i++)
        {
            int index = (int)_activeCartoonTypes[i];
            _contents[i].pointerClickAction += () => OnClickContent(index);
        }

        _prevBtn?.onClick.AddListener(() =>
        {
            if(UserDataManager.inst.isSingleContent)
            {
                _pageController.ChangePage(PAGE_TYPE.PAGE_AOD);
            }
            else
            {
                _pageController.ChangePage(PAGE_TYPE.PAGE_SELECT_CONTENT);
            }
        });
        _descriptionBtn?.onClick.AddListener(OnClickDescription);

        (_pageController as PC_Main).OnShuffleAction += ShuffleContents;
    }

    private void OnClickContent(string cartoonKey)
    {
        UserDataManager.inst.SelectSubContent(cartoonKey);
        //UserDataManager.inst.SelectContentCode(_activeCartoonTypes[index]);
        _pageController.ChangePage(PAGE_TYPE.PAGE_SELECT_FRAME);
    }

    private void CreateStyleContent()
    {
        List<UC_SelectableContent> contents = new List<UC_SelectableContent>();
        string key = StringCacheManager.Instance.GetContentKey(CONTENT_TYPE.AI_CARTOON);
        foreach (var item in AdminManager.Instance.ServiceData.ContentsDetail)
        {
            if (item.Key.Contains(key) && item.Value.Use.ToLower() == "true")
            {
                GameObject content;
                if (_contentParentRow1.transform.childCount < 4)
                {
                    content = Instantiate(_styleContent, _contentParentRow1.transform);
                }
                else
                {
                    content = Instantiate(_styleContent, _contentParentRow2.transform);
                }

                UC_StyleContent styleContent = content.GetComponentInChildren<UC_StyleContent>();
                contents.Add(styleContent);
                styleContent.InitComponent();

                styleContent.SetThumbnail(item.Value.Thumbnail_data);
                styleContent.SetTitle(item.Value.Korean_Title);
                styleContent.SetDescription(item.Value.Korean_SubText);
                styleContent.pointerClickAction += () => OnClickContent(item.Key);

                _contentParents.Add(styleContent.transform.parent);
            }
        }

        _contents = contents.ToArray();
        _shuffledContentParents = _contentParents.ToList();

        _guideImage = AdminManager.Instance.ServiceData.Contents[key].PopupGuideImage_data;
        _isContentCreated = true;
    }

    private void OnClickDescription()
    {
        (_pageController as PC_Main).globalPage.OpenConfirmPopup(_popupTitle, _popupDescription, _guideImage);
    }

    public override void OnPageEnable()
    {
        if (!_isContentCreated)
        {
            CreateStyleContent();
        }
    }

    public override void OnPageDisable()
    {
        (_pageController as PC_Main).globalPage.CloseConfirmPopup();
    }

    protected override void OnPageReset()
    {
    }
}
