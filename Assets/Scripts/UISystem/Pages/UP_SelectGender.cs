using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vivestudios.UI;

public class UP_SelectGender : UP_BaseSelectContent, IPageTimeLimit
{
    public int MaxTime { get => _maxTime; set => _maxTime = value; }
    public TextMeshProUGUI timeText { get => _timeText; set => _timeText = value; }

    [SerializeField]
    private Button _prevBtn;
    [SerializeField]
    private GameObject _styleContent;
    [SerializeField]
    private Transform _contentParent;
    private string key;

    private List<UC_ProfileContent> _profileContents = new List<UC_ProfileContent>();

    public override void InitPage()
    {
        base.InitPage();

    }

    public override void ApplyAdminData()
    {
        base.ApplyAdminData();
        _maxTime = AdminManager.Instance.BasicSetting.Config.WFMenu_data;//ConfigData.config.genderSelectTime;
    }

    public override void BindDelegates()
    {
        base.BindDelegates();

        for (int i = 0; i < _contents.Length; i++)
        {
            int index = i;
            _contents[i].pointerClickAction += () => OnClickContent(index);
        }

        _prevBtn?.onClick.AddListener(() =>
        {
            if (UserDataManager.inst.isSingleContent)
            {
                _pageController.ChangePage(PAGE_TYPE.PAGE_AOD);
            }
            else
            {
                _pageController.ChangePage(PAGE_TYPE.PAGE_SELECT_CONTENT);
            }
        });
    }

    public override void OnPageDisable()
    {

    }

    public override void OnPageEnable()
    {
        if (!_isContentCreated)
        {
            CreateContent();
        }
    }

    protected override void OnPageReset()
    {

    }

    private void CreateContent()
    {
        key = StringCacheManager.Instance.GetContentKey(CONTENT_TYPE.WHAT_IF);

        foreach (var item in AdminManager.Instance.ServiceData.ContentsDetail)
        {
            if (item.Value.Category.Contains(key) && item.Value.Use.ToLower() == "true")
            {
                GameObject content = Instantiate(_styleContent, _contentParent);
                UC_ProfileContent styleContent = content.GetComponent<UC_ProfileContent>();
                //styleContent.SetContentDetail(item.Value);
                styleContent.SetThumbnail(item.Value.Thumbnail_data, true);
                styleContent.SetTitle("");
                styleContent.SetDescription("");
                styleContent.SetGenderActive(false);

                styleContent.pointerClickAction += () => OnClickContent(item.Value);
                styleContent.Select(false);

                _profileContents.Add(styleContent);
            }
        }

        _isContentCreated = true;
    }

    private void OnClickContent(ServiceData.ContentsDetailEntry contentDetail)
    {
        UserDataManager.Instance.SelectSubContent(contentDetail.Key);

        (_pageController as PC_Main)?.globalPage?.OpenAIProfileAlert(
            AdminManager.Instance.ServiceData.Contents[key].GuideImage_data, 
            () => { _pageController.ChangePage(PAGE_TYPE.PAGE_SELECT_FRAME); });
    }
}
