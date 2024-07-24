using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vivestudios.UI;

public class UP_SelectChromaKeyBackground : UP_DecoratePageBase, IPageTimeLimit
{
    private int _maxTime = 100;
    private Coroutine _timerCoroutine = null;

    [SerializeField]
    private Button _prevBtn, _nextBtn;
    [SerializeField]
    private Button _descriptBtn;
    [SerializeField]
    private string _popupTitle;
    [SerializeField]
    private string _popupDescription;
    [SerializeField]
    private Sprite _popupSprite;
    [SerializeField]
    private UC_SelectableContent[] _contents;


    public TextMeshProUGUI timeText { get => _timeText; set => _timeText = value; }
    public int MaxTime { get => _maxTime; set => _maxTime = value; }

    public override void InitPage ()
    {
        base.InitPage();

        // TODO : Set MaxTime
        _maxTime = ConfigData.config.frameSelectTime;
        InitContents();
    }

    private void InitContents ()
    {
        for(int i = 0; i < _contents.Length; i++)
        {
            if(ChromaKeyModule.inst.options.Length > i)
            {
                _contents[i].gameObject.SetActive(true);
                _contents[i].SetNameText(ChromaKeyModule.inst.options[i].name_kor);
                _contents[i].SetThumbnail(ChromaKeyModule.inst.options[i].thumbnail);
                int index = i;
                _contents[i].pointerClickAction += () => OnClickContent(index);
            }
            else
            {
                _contents[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnClickContent (int index)
    {
        UserDataManager.inst.SetSelectedChromaKeyNum(index);
        for(int i = 0; i < _contents.Length; i++)
        {
            if(i != index)
            {
                _contents[i].Select(false);
            }
        }

        UpdateTempFrame();
    }

    private void UpdateTempFrame ()
    {
        _frameAreaDic[UserDataManager.inst.selectedFrame].SetPics(ChromaKeyModule.inst.options[UserDataManager.inst.selectedChromaKeyNum].images.ToList().GetRange(0, _frameAreaDic[UserDataManager.inst.selectedFrame].GetPicCount()));
        _frameAreaDic[UserDataManager.inst.selectedFrame].SetLutEffect(null);
        _frameAreaDic[UserDataManager.inst.selectedFrame].UpdateFrame();
    }

    public override void BindDelegates ()
    {
        base.BindDelegates();

        _prevBtn.onClick.AddListener(OnClickPrev);
        _nextBtn.onClick.AddListener(OnClickNext);
        _descriptBtn.onClick.AddListener(OnClickDescript);

        (_pageController as PC_Main).OnTimeUpdateAction -= OnTimeUpdate;
        (_pageController as PC_Main).OnTimeLimitDoneAction -= OnTimeLimitDone;
    }

    private void OnClickDescript ()
    {
        (_pageController as PC_Main).globalPage.OpenConfirmPopup(_popupTitle, _popupDescription, _popupSprite);
    }

    public override void OnPageDisable ()
    {
        if(this.GetType().GetInterfaces().Contains(typeof(IPageTimeLimit)))
        {
            ResetTimer();
        }

        (_pageController as PC_Main)?.globalPage?.CloseToast();
        (_pageController as PC_Main)?.globalPage?.CloseConfirmPopup();

        _frameAreaDic[UserDataManager.inst.selectedFrame].SetPics(null);
        _frameAreaDic[UserDataManager.inst.selectedFrame].UpdateFrame();
    }

    public override void OnPageEnable ()
    {

        _contents[UserDataManager.inst.selectedChromaKeyNum].OnPointerDown(null);
        _contents[UserDataManager.inst.selectedChromaKeyNum].OnPointerClick(null);

        if(this.GetType().GetInterfaces().Contains(typeof(IPageTimeLimit)))
        {
            StartTimer();
        }
    }

    protected override void OnPageReset ()
    {

    }

    private void OnClickPrev ()
    {
        (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_SELECT_FRAME);
    }

    private void OnClickNext ()
    {
        (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_PAYMENT);
    }

    private IEnumerator TimerRoutine ()
    {
        _timeText.text = _maxTime.ToString();

        int time = 0;

        while(time < _maxTime)
        {
            yield return new WaitForSecondsRealtime(1);
            time++;

            _timeText.text = (_maxTime - time).ToString();

            if(_maxTime - time == 5)
            {
                (_pageController as PC_Main)?.globalPage?.OpenTimerToast(5);
            }
        }

        UserDataManager.inst.SetSelectedChromaKeyNum(0);
        (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_AOD);
    }

    public void StartTimer ()
    {
        ResetTimer();
        _timerCoroutine = StartCoroutine(TimerRoutine());
    }

    public void ResetTimer ()
    {
        if(_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
        }
    }
}
