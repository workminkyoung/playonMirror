using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vivestudios.UI;
using UnityEngine.UI;
using MPUIKIT;
using TMPro;

public class UP_SelectAIResult : UP_BasePage
{
    [SerializeField]
    private List<MPImage> _mpimages = new List<MPImage>();
    [SerializeField]
    private Image _btnBG;
    [SerializeField]
    private Image _btnCheck;
    [SerializeField]
    private TextMeshProUGUI _btnText;

    [SerializeField]
    private Image _timerImg;
    [SerializeField]
    private TextMeshProUGUI _timerText;

    private Button _btnNext;
    private List<UC_ToggleSelect> _toggleSelects = new List<UC_ToggleSelect>();
    private UC_ToggleSelect _selected;
    private int _selectedIndex = 0;

    private Coroutine TimerCoroutine = null;

    private const int TIME_LIMIT = 15;
    private readonly Color BTN_BG_DEFAULT_COLOR = new Color(0.81f, 0.2f, 0.54f);
    private readonly Color BTN_BG_DISABLE_COLOR = new Color(0.88f, 0.88f, 0.88f);
    private readonly Color BTN_TXT_DEFAULT_COLOR = new Color(0.98f, 0.98f, 0.98f);
    private readonly Color BTN_TXT_DISABLE_COLOR = new Color(0.74f, 0.74f, 0.74f);

    private readonly Color TIMER_DEFAULT_COLOR = Color.black;
    private readonly Color TIMER_EMERGENCY_COLOR = Color.red;

    public override void InitPage()
    {
        _btnNext = GetComponentInChildren<Button>();
        _toggleSelects.AddRange(GetComponentsInChildren<UC_ToggleSelect>());

        for (int i = 0; i < _toggleSelects.Count; i++)
        {
            _toggleSelects[i].InitComponent();
        }

        _btnNext.onClick.AddListener(() =>
        {
            //TODO : get selected toggle and set to data

            for (int i = 0; i < _toggleSelects.Count; i++)
            {
                if (_toggleSelects[i].toggle.isOn)
                {
                    _selected = _toggleSelects[i];
                    _selectedIndex = i;
                }
            }

            if (_selected == null)
                return;

            PhotoDataManager.inst.SetSelectedAIProfile(PhotoDataManager.inst.photoOrigin[_selectedIndex]);
            (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_LOADING);

            StopCoroutine(TimerCoroutine);
        });
    }

    public override void BindDelegates()
    {
        for (int i = 0; i < _toggleSelects.Count; i++)
        {
            int index = i;
            _toggleSelects[i].OnToggleValueChanged += (isOn) => OnToggleSelected(index, isOn);
        }
    }

    private void OnToggleSelected(int index, bool isOn)
    {
        if (isOn)
        {
            for (int i = 0; i < _toggleSelects.Count; i++)
            {

                _toggleSelects[i].toggle.isOn = i == index;
            }

            _btnBG.color = BTN_BG_DEFAULT_COLOR;
            _btnCheck.color = BTN_TXT_DEFAULT_COLOR;
            _btnText.color = BTN_TXT_DEFAULT_COLOR;
        }
        else
        {
            _btnBG.color = BTN_BG_DISABLE_COLOR;
            _btnCheck.color = BTN_TXT_DISABLE_COLOR;
            _btnText.color = BTN_TXT_DISABLE_COLOR;
        }
    }

    public override void EnablePage(bool isEnable)
    {
        base.EnablePage(isEnable);
        if (isEnable)
        {
            _selected = null;
            for (int i = 0; i < _mpimages.Count; i++)
            {
                _mpimages[i].sprite = UtilityExtensions.ConvertToSprite(PhotoDataManager.inst.photoOrigin[i]);
            }

            for (int i = 0; i < _toggleSelects.Count; i++)
            {

                _toggleSelects[i].toggle.isOn = false;
            }
            _btnBG.color = BTN_BG_DISABLE_COLOR;
            _btnCheck.color = BTN_TXT_DISABLE_COLOR;
            _btnText.color = BTN_TXT_DISABLE_COLOR;
        }
        else
        {

        }
    }

    private void OnEnable()
    {
        if (!_pageController)
            return;

        TimerCoroutine = StartCoroutine(TimerRoutine());
    }

    private IEnumerator TimerRoutine()
    {
        int time = 0;

        _timerText.color = TIMER_DEFAULT_COLOR;
        _timerImg.color = TIMER_DEFAULT_COLOR;
        _timerText.text = $"{TIME_LIMIT - time}초";

        while (0 < TIME_LIMIT - time)
        {
            _timerText.text = $"{TIME_LIMIT - time}초";

            if (TIME_LIMIT - time <= 3)
            {
                _timerText.color = TIMER_EMERGENCY_COLOR;
                _timerImg.color = TIMER_EMERGENCY_COLOR;
            }

            time++;

            yield return new WaitForSecondsRealtime(1);
        }


        if (!_selected)
        {
            PhotoDataManager.inst.SetSelectedAIProfile(PhotoDataManager.inst.photoOrigin[0]);
        }
        else
        {
            PhotoDataManager.inst.SetSelectedAIProfile(PhotoDataManager.inst.photoOrigin[_selectedIndex]);
        }

        (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_LOADING);
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
