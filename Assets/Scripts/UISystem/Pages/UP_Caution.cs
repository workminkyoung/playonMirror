using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vivestudios.UI;

public class UP_Caution : UP_BasePage
{
    [SerializeField]
    private Button _nextBtn;
    [SerializeField]
    private TextMeshProUGUI _btnTitleTimeText;
    [SerializeField]
    private RectTransform _btnEnableArea;
    [SerializeField]
    private RectTransform _btnDisbaleArea;

    [SerializeField]
    private RectTransform _contentAreaDefault;
    [SerializeField]
    private RectTransform _contentAreaProfile;
    [SerializeField]
    private RectTransform _contentAreaBeauty;

    [SerializeField]
    private TextMeshProUGUI _timerText;

    private int LIMIT_TIME = 100;

    private Coroutine _timerCoroutine = null;
    private Coroutine _btnTextCoroutine = null;

    public override void InitPage()
    {
        LIMIT_TIME = ConfigData.config.cautionPageTime;
    }

    public override void BindDelegates()
    {
        _nextBtn?.onClick.AddListener(() =>
        {
            switch (UserDataManager.Instance.selectedContent)
            {
                case CONTENT_TYPE.AI_CARTOON:
                    (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_SHOOT_CARTOON);
                    break;
                case CONTENT_TYPE.AI_PROFILE:
                    (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_SHOOT_PROFILE);
                    break;
                case CONTENT_TYPE.AI_TIME_MACHINE:
                    break;
                case CONTENT_TYPE.AI_BEAUTY:
                    (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_SHOOT_BEAUTY);
                    break;
                case CONTENT_TYPE.WHAT_IF:
                    (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_SHOOT_WHAT_IF);
                    break;
                default:
                    break;
            }

            if (_timerCoroutine != null)
            {
                StopCoroutine(_timerCoroutine);
            }
        });
    }

    private void OnEnable()
    {
        if (_pageController == null)
            return;

        _contentAreaDefault.gameObject.SetActive(UserDataManager.inst.selectedContent == CONTENT_TYPE.AI_CARTOON);
        _contentAreaProfile.gameObject.SetActive(UserDataManager.inst.selectedContent == CONTENT_TYPE.AI_PROFILE ||
                                                 UserDataManager.inst.selectedContent == CONTENT_TYPE.WHAT_IF);
        _contentAreaBeauty.gameObject.SetActive(UserDataManager.inst.selectedContent == CONTENT_TYPE.AI_BEAUTY);
        _timerCoroutine = StartCoroutine(TimerRoutine());
        _btnTextCoroutine = StartCoroutine(BtnTextRoutine());

    }

    private void OnDisable()
    {
        if (_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
            _timerCoroutine = null;
        }

        if (_btnTextCoroutine != null)
        {
            StopCoroutine(_btnTextCoroutine);
            _btnTextCoroutine = null;
        }
    }

    private IEnumerator BtnTextRoutine()
    {
        _btnDisbaleArea.gameObject.SetActive(true);
        _btnEnableArea.gameObject.SetActive(false);
        _nextBtn.interactable = false;

        int time = 3;
        while (time >= 1)
        {
            _btnTitleTimeText.text = time.ToString();
            yield return new WaitForSecondsRealtime(1);
            time--;
        }

        _btnDisbaleArea.gameObject.SetActive(false);
        _btnEnableArea.gameObject.SetActive(true);
        _nextBtn.interactable = true;
    }

    private IEnumerator TimerRoutine()
    {
        int time = 0;
        while (time < LIMIT_TIME)
        {
            if ((LIMIT_TIME - time) / 60 < 0)
            {
                _timerText.text = string.Format("{0}초", LIMIT_TIME - time);
            }
            else
            {
                _timerText.text = string.Format("{0}분{1}초", (int)((LIMIT_TIME - time) / 60), (LIMIT_TIME - time) % 60);
            }

            yield return new WaitForSecondsRealtime(1);
            time++;
        }

        switch (UserDataManager.Instance.selectedContent)
        {
            case CONTENT_TYPE.AI_CARTOON:
                (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_SHOOT_CARTOON);
                break;
            case CONTENT_TYPE.AI_PROFILE:
                (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_SHOOT_PROFILE);
                break;
            case CONTENT_TYPE.AI_TIME_MACHINE:
                break;
            case CONTENT_TYPE.AI_BEAUTY:
                (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_SHOOT_BEAUTY);
                break;
            case CONTENT_TYPE.WHAT_IF:
                (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_SHOOT_WHAT_IF);
                break;
            default:
                break;
        }
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
