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
    }

    public override void OnPageDisable()
    {
    }

    protected override void OnPageReset()
    {
    }
}
