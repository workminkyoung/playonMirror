using System.Collections;
using System.Collections.Generic;
using System.Drawing.Design;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vivestudios.UI;

public class UP_SelectProfile : UP_BaseSelectContent, IPageTimeLimit
{
    [SerializeField]
    private string _popupTitle;
    [SerializeField]
    private string _popupDescription;
    [SerializeField]
    private Button _prevBtn;
    //[SerializeField]
    //private PROFILE_TYPE[] _profileContents;
    [SerializeField]
    private GameObject _styleContent;
    [SerializeField]
    private Transform _contentParent;
    private List<UC_ProfileContent> _profileContents = new List<UC_ProfileContent>();
    private string key;// = StringCacheManager.Instance.GetContentKey(CONTENT_TYPE.AI_PROFILE);
    //private Sprite _guideImage;

    private bool _requirePopup = true;
    public int MaxTime { get => _maxTime; set => _maxTime = value; }
    public TextMeshProUGUI timeText { get => _timeText; set => _timeText = value; }

    public override void InitPage()
    {
        base.InitPage();

        _maxTime = ConfigData.config.profileSelectTime;
    }

    protected override void OnDisable()
    {
        (_pageController as PC_Main).globalPage.CloseConfirmPopup();
        base.OnDisable();
    }

    public override void BindDelegates()
    {
        for(int i = 0; i < _contents.Length; i++)
        {
            int index = i;
            _contents[i].pointerClickAction += () => OnClickContent(index);
        }
        
        _prevBtn?.onClick.AddListener(() =>
        {
            _pageController.ChangePage(PAGE_TYPE.PAGE_SELECT_CONTENT);
            _requirePopup = false;
        });

        GameManager.OnGameResetAction += () =>
        {
            _requirePopup = true;
        };
    }

    private void CreateContent()
    {
        key = StringCacheManager.Instance.GetContentKey(CONTENT_TYPE.AI_PROFILE);

        foreach (var item in AdminManager.Instance.ServiceData.ContentsDetail)
        {
            if (item.Value.Category.Contains(key) && item.Value.Use.ToLower() == "true")
            {
                GameObject contentObj = Instantiate(_styleContent, _contentParent);
                UC_ProfileContent content = contentObj.GetComponentInChildren<UC_ProfileContent>();
                //content.SetContentDetail(item.Value);
                content.SetThumbnail(item.Value.Thumbnail_data);
                content.SetTitle(item.Value.Korean_Title);
                content.SetDescription(item.Value.Korean_SubText);
                content.SetGenderActive(true);
                content.SetGender(item.Value.Gender_type);

                content.pointerClickAction += () => OnClickContent(item.Value);
                content.Select(false);

                _profileContents.Add(content);
            }
        }

        _isContentCreated = true;
    }

    private void OnClickContent(ServiceData.ContentsDetailEntry contentDetail)
    {
        _requirePopup = false;
        UserDataManager.Instance.SelectSubContent(contentDetail.Key);

        (_pageController as PC_Main)?.globalPage?.OpenAIProfileAlert(
            AdminManager.Instance.ServiceData.Contents[key].GuideImage_data,
            () => { _pageController.ChangePage(PAGE_TYPE.PAGE_SELECT_FRAME); });
    }

    public override void OnPageEnable()
    {
        if (!_isContentCreated)
        {
            CreateContent();
        }

        if (_pageController && _requirePopup)
        {
            //(_pageController as PC_Main).globalPage.OpenConfirmPopup(_popupTitle, _popupDescription, _guideImage);
        }
    }

    public override void OnPageDisable()
    {
    }

    protected override void OnPageReset()
    {
    }
}
