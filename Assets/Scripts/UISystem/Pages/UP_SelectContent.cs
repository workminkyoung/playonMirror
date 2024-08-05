using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Vivestudios.UI;

public class UP_SelectContent : UP_BaseSelectContent, IPageTimeLimit
{
    public int MaxTime { get => _maxTime; set => _maxTime = value; }
    public TextMeshProUGUI timeText { get => _timeText; set => _timeText = value; }

    [SerializeField]
    private List<CONTENT_TYPE> _activeContentType = new List<CONTENT_TYPE>();
    [SerializeField]
    private GameObject _contentPrefab;
    [SerializeField]
    private Transform _contentParent;
    private List<UC_StyleVideoContent> _styleContents = new List<UC_StyleVideoContent>();

    public override void InitPage()
    {
        _maxTime = ConfigData.config.contentSelectTime;
        _styleContents = new List<UC_StyleVideoContent>();

        base.InitPage();
    }

    private void CreateContent()
    {
        foreach (var item in AdminManager.Instance.ServiceData.Contents)
        {
            if (item.Value.Use.ToLower() == "true")
            {
                GameObject contentObj = Instantiate(_contentPrefab, _contentParent);
                UC_StyleVideoContent content = contentObj.GetComponentInChildren<UC_StyleVideoContent>();
                content.SetTitle(item.Value.Korean_Title);
                content.SetDescription(item.Value.Korean_SubText);
                content.SetMaxPlayer(item.Value.Korean_People);
                content.SetVideo(item.Value.VideoThumbnail_path);
                content.SetThumbnail(item.Value.ImageThumbnail_data);
                content.Select(false);
                _styleContents.Add(content);
            }
        }
        

        //for (int i = 0; i < _activeContentType.Count; i++)
        //{
        //    GameObject contentObj = Instantiate(_contentPrefab, _contentParent);
        //    UC_StyleVideoContent content = contentObj.GetComponentInChildren<UC_StyleVideoContent>();
        //    content.SetTitle(StringCacheManager.inst.GetContentTitle(_activeContentType[i]));
        //    content.SetDescription(StringCacheManager.inst.GetContentDescription(_activeContentType[i]));
        //    content.SetMaxPlayer(StringCacheManager.inst.GetContentPlayerNum(_activeContentType[i]));
        //    content.SetVideo(ResourceCacheManager.inst.GetContentVideoThumbnail(_activeContentType[i]));
        //    content.Select(false);
        //    _styleContents.Add(content);
        //}
    }

    public override void BindDelegates()
    {
        base.BindDelegates();
    }

    protected override void OnClickContent(int index)
    {
        switch (index)
        {
            case 0:
                UserDataManager.inst.SelectContent(CONTENT_TYPE.AI_CARTOON);
                UserDataManager.inst.SetSelectedFrameColor(FRAME_COLOR_TYPE.FRAME_WHITE);
                _pageController.ChangePage(PAGE_TYPE.PAGE_SELECT_CARTOON_STYLE);
                break;
            case 1:
                UserDataManager.inst.SelectContent(CONTENT_TYPE.AI_PROFILE);
                UserDataManager.inst.SetSelectedFrameColor(FRAME_COLOR_TYPE.FRAME_WHITE);
                _pageController.ChangePage(PAGE_TYPE.PAGE_SELECT_AI_PROFILE);
                break;
            case 2:
                UserDataManager.inst.SelectContent(CONTENT_TYPE.AI_BEAUTY);
                UserDataManager.inst.SetSelectedFrameColor(FRAME_COLOR_TYPE.FRAME_WHITE);
                _pageController.ChangePage(PAGE_TYPE.PAGE_SELECT_FRAME);
                break;
            case 3:
                UserDataManager.inst.SelectContent(CONTENT_TYPE.WHAT_IF);
                UserDataManager.inst.SetSelectedFrameColor(FRAME_COLOR_TYPE.FRAME_JTBC_SI);
                _pageController.ChangePage(PAGE_TYPE.PAGE_SELECT_WHAT_IF);
                break;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }
    //다운받는 virture 함수 만들어서 관리

    public override void OnPageEnable()
    {
        //처음 다운받을때 로딩 필요
        if (!_isContentCreated)
        {
            UserDataManager.inst.SelectContent(CONTENT_TYPE.AI_CARTOON);
            CreateContent();
        }
    }

    public override void OnPageDisable()
    {
    }

    protected override void OnPageReset()
    {
    }
}
