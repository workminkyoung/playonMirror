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
    private PROFILE_TYPE[] _profileContents;
    [SerializeField]
    private GENDER_TYPE[] _genderType;

    public override void InitPage()
    {
        base.InitPage();

        _maxTime = ConfigData.config.genderSelectTime;
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
            _pageController.ChangePage(PAGE_TYPE.PAGE_SELECT_CONTENT);
        });
    }

    public override void OnPageDisable()
    {

    }

    public override void OnPageEnable()
    {
        //for (int i = 0; i < _contents.Length; i++)
        //{
        //    (_contents[i] as UC_ProfileContent).Select(false);
        //}
    }

    protected override void OnPageReset()
    {

    }

    protected override void OnClickContent(int index)
    {
        UserDataManager.inst.SelectProfile(_profileContents[index]);
        UserDataManager.inst.SelectContentCode(_profileContents[index]);
        UserDataManager.inst.SetGender(_genderType[index]);
        (_pageController as PC_Main)?.globalPage?.OpenAIProfileAlert(() =>
        {
            _pageController.ChangePage(PAGE_TYPE.PAGE_SELECT_FRAME);
        });
    }
}
