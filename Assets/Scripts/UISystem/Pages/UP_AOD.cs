using ServiceData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Vivestudios.UI;
using static UnityEditor.Progress;

public class UP_AOD : UP_BasePage, IPointerClickHandler
{
    [SerializeField]
    private Button _privacyPolicyBtn;
    [SerializeField]
    private Button _termsOfUseBtn;
    [SerializeField]
    private Image _bgImage;
    [SerializeField]
    private Image _eventLogo;
    [SerializeField]
    private PAGE_TYPE _nextPage = PAGE_TYPE.PAGE_SELECT_CONTENT;

    private Action _singleNextPage = null;
    private bool _checkContentType = false;

    public override void InitPage()
    {
    }


    public override void BindDelegates()
    {
        _privacyPolicyBtn?.onClick.AddListener(() =>
        {
            (_pageController as PC_Main).globalPage.OpenPolicyPopup(POLICY_TYPE.PRIVACY_POLICY);
        });

        _termsOfUseBtn?.onClick.AddListener(() =>
        {
            (_pageController as PC_Main).globalPage.OpenPolicyPopup(POLICY_TYPE.TERMS_OF_USE);
        });
    }

    private void CheckContentPage()
    {
        int contentCount = 0;
        ServiceData.ContentsEntry contentData = null;
        string contentKey = string.Empty;
        foreach (var item in AdminManager.Instance.ServiceData.Contents)
        {
            if (!item.Value.Use)
                continue;

            contentData = item.Value;
            contentKey = item.Key;
            contentCount++;
        }

        if (contentData == null)
        {
            // Error Case
        }
        else
        {
            if (contentCount == 0)
            {
                // Error Case
            }
            else if (contentCount == 1)
            {
                // Skip Content Depth

                switch (contentData.ContentType)
                {
                    case CONTENT_TYPE.AI_CARTOON:
                        _singleNextPage = () =>
                        {
                            UserDataManager.inst.SetSingleContent(true);
                            UserDataManager.inst.SelectContent(CONTENT_TYPE.AI_CARTOON);
                            UserDataManager.inst.SelectContent(contentKey);
                            UserDataManager.inst.SetSelectedFrameColor(UserDataManager.inst.defaultFrameColor);
                            _pageController.ChangePage(PAGE_TYPE.PAGE_SELECT_CARTOON_STYLE);
                        };
                        _singleNextPage();
                        break;
                    case CONTENT_TYPE.AI_PROFILE:
                        _singleNextPage = () =>
                        {
                            UserDataManager.inst.SetSingleContent(true);
                            UserDataManager.inst.SelectContent(CONTENT_TYPE.AI_PROFILE);
                            UserDataManager.inst.SelectContent(contentKey);
                            UserDataManager.inst.SetSelectedFrameColor(UserDataManager.inst.defaultFrameColor);
                            _pageController.ChangePage(PAGE_TYPE.PAGE_SELECT_AI_PROFILE);
                        };
                        _singleNextPage();
                        break;
                    case CONTENT_TYPE.AI_BEAUTY:
                        _singleNextPage = () =>
                        {
                            UserDataManager.inst.SetSingleContent(true);
                            UserDataManager.inst.SelectContent(CONTENT_TYPE.AI_BEAUTY);
                            UserDataManager.inst.SelectContent(contentKey);
                            UserDataManager.inst.SetSelectedFrameColor(UserDataManager.inst.defaultFrameColor);
                            _pageController.ChangePage(PAGE_TYPE.PAGE_SELECT_FRAME);
                        };
                        _singleNextPage();
                        break;
                    case CONTENT_TYPE.WHAT_IF:
                        _singleNextPage = () =>
                        {
                            UserDataManager.inst.SetSingleContent(true);
                            UserDataManager.inst.SelectContent(CONTENT_TYPE.WHAT_IF);
                            UserDataManager.inst.SelectContent(contentKey);
                            UserDataManager.inst.SetSelectedFrameColor(UserDataManager.inst.defaultFrameColor);
                            _pageController.ChangePage(PAGE_TYPE.PAGE_SELECT_WHAT_IF);
                        };
                        _singleNextPage();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                // Normal Depth
                _singleNextPage = () =>
                {
                    _pageController.ChangePage(PAGE_TYPE.PAGE_SELECT_CONTENT);
                };
                _singleNextPage();
            }
        }

        _checkContentType = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //_pageController.ChangePage(PAGE_TYPE.PAGE_SELECT_CONTENT);
        if (_checkContentType)
        {
           _singleNextPage();
        }
        else
        {
            CheckContentPage();
        }
    }

    public override void OnPageEnable()
    {
        CustomLogger.Log(StringCacheManager.inst.DividerLine + " Session Starting " + StringCacheManager.inst.DividerLine);
    }

    public override void ApplyAdminData()
    {
        base.ApplyAdminData();

        _eventLogo.gameObject.SetActive(false);

        if (!string.IsNullOrEmpty(AdminManager.Instance.BasicSetting.Config.StartMediaVideo_path))
        {
            if (AdminManager.Instance.BasicSetting.Config.StartMediaImage_data != null)
            {
                // 비디오 이미지 동시 플레이
            }
            else
            {
                // 비디오 ONLY 플레이
                _bgImage.sprite = AdminManager.Instance.BasicSetting.Config.StartMediaImage_data;
            }
            _eventLogo.gameObject.SetActive(true);
        }
        else if (AdminManager.Instance.BasicSetting.Config.StartMediaImage_data != null)
        {
            _bgImage.sprite = AdminManager.Instance.BasicSetting.Config.StartMediaImage_data;
            _eventLogo.gameObject.SetActive(true);
        }
        else if (AdminManager.Instance.BasicSetting.Config.StartImage_data != null)
        {
            _bgImage.sprite = AdminManager.Instance.BasicSetting.Config.StartImage_data;
        }
        else if (AdminManager.Instance.BasicSetting.Config.BGImage_data != null)
        {
            _bgImage.sprite = AdminManager.Instance.BasicSetting.Config.BGImage_data;
        }
        else if (!string.IsNullOrEmpty(AdminManager.Instance.BasicSetting.Config.ColorCode))
        {
            _bgImage.color = ColorExtension.HexToColor(AdminManager.Instance.BasicSetting.Config.ColorCode);
        }
    }


    public override void OnPageDisable()
    {
    }

    protected override void OnPageReset()
    {
    }
}