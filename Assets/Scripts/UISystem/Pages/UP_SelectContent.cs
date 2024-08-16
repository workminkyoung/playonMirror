using System;
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
        _styleContents = new List<UC_StyleVideoContent>();

        base.InitPage();
    }

    public override void ApplyAdminData()
    {
        base.ApplyAdminData();
        _maxTime = AdminManager.Instance.BasicSetting.Config.ContentsMenu_data;//ConfigData.config.contentSelectTime;
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
                content.pointerClickAction += () => OnClickContent(item.Value.ContentType, item.Key);
                content.Select(false);
                _styleContents.Add(content);
            }
        }

        _isContentCreated = true;
    }

    public override void BindDelegates()
    {
    }

    protected override void OnClickContent(int index)
    {
    }

    private void OnClickContent(CONTENT_TYPE contentType, string contentKey)
    {
        switch (contentType)
        {
            case CONTENT_TYPE.AI_CARTOON:
                UserDataManager.inst.SelectContent(CONTENT_TYPE.AI_CARTOON);
                UserDataManager.inst.SelectContent(contentKey);
                UserDataManager.inst.SetSelectedFrameColor(UserDataManager.inst.defaultFrameColor);
                _pageController.ChangePage(PAGE_TYPE.PAGE_SELECT_CARTOON_STYLE);
                break;
            case CONTENT_TYPE.AI_PROFILE:
                UserDataManager.inst.SelectContent(CONTENT_TYPE.AI_PROFILE);
                UserDataManager.inst.SelectContent(contentKey);
                UserDataManager.inst.SetSelectedFrameColor(UserDataManager.inst.defaultFrameColor);
                _pageController.ChangePage(PAGE_TYPE.PAGE_SELECT_AI_PROFILE);
                break;
            case CONTENT_TYPE.AI_BEAUTY:
                UserDataManager.inst.SelectContent(CONTENT_TYPE.AI_BEAUTY);
                UserDataManager.inst.SelectContent(contentKey);
                UserDataManager.inst.SetSelectedFrameColor(UserDataManager.inst.defaultFrameColor);
                _pageController.ChangePage(PAGE_TYPE.PAGE_SELECT_FRAME);
                break;
            case CONTENT_TYPE.WHAT_IF:
                UserDataManager.inst.SelectContent(CONTENT_TYPE.WHAT_IF);
                UserDataManager.inst.SelectContent(contentKey);
                UserDataManager.inst.SetSelectedFrameColor(UserDataManager.inst.defaultFrameColor);
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
