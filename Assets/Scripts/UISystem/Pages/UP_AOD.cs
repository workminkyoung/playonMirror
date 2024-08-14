using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Vivestudios.UI;

public class UP_AOD : UP_BasePage, IPointerClickHandler
{
    [SerializeField]
    private Button _privacyPolicyBtn;
    [SerializeField]
    private Button _termsOfUseBtn;
    [SerializeField]
    private Image _bgImage;

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

    public void OnPointerClick(PointerEventData eventData)
    {
        _pageController.ChangePage(PAGE_TYPE.PAGE_SELECT_CONTENT);
    }

    public override void OnPageEnable()
    {
        CustomLogger.Log(StringCacheManager.inst.DividerLine + " Session Starting " + StringCacheManager.inst.DividerLine);
    }

    public override void ApplyAdminData()
    {
        base.ApplyAdminData();


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
        }
        else if (AdminManager.Instance.BasicSetting.Config.StartMediaImage_data != null)
        {
            _bgImage.sprite = AdminManager.Instance.BasicSetting.Config.StartMediaImage_data;
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
