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

    private bool _pageInitDone = false;

    public TextMeshProUGUI timeText { get => _timeText; set => _timeText = value; }
    public int MaxTime { get => _maxTime; set => _maxTime = value; }

    public override void InitPage ()
    {
        base.InitPage();

        // TODO : Set MaxTime
        _maxTime = ConfigData.config.frameSelectTime;
        _pageInitDone = true;
        
    }

    private void InitContents ()
    {
        for(int i = 0; i < _contents.Length; i++)
        {
            if(ChromaKeyModule.inst.options.Count > i)
            {
                _contents[i].gameObject.SetActive(true);
                switch(AdminManager.Instance.Language)
                {
                    case LANGUAGE_TYPE.KOR:
                        _contents[i].SetNameText(ChromaKeyModule.inst.options[i].name_kor);
                        break;
                    case LANGUAGE_TYPE.ENG:
                        _contents[i].SetNameText(ChromaKeyModule.inst.options[i].name_eng);
                        break;
                    case LANGUAGE_TYPE.CHN:
                        _contents[i].SetNameText(ChromaKeyModule.inst.options[i].name_chn);
                        break;
                }
                
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
        _frameAreaDic[UserDataManager.inst.selectedFrameType].SetPics(ChromaKeyModule.inst.options[UserDataManager.inst.selectedChromaKeyNum].images.ToList().GetRange(0, _frameAreaDic[UserDataManager.inst.selectedFrameType].GetPicCount()));
        _frameAreaDic[UserDataManager.inst.selectedFrameType].SetLutEffect(string.Empty);
        _frameAreaDic[UserDataManager.inst.selectedFrameType].UpdateFrame();
    }

    public override void BindDelegates ()
    {
        _prevBtn.onClick.AddListener(OnClickPrev);
        _nextBtn.onClick.AddListener(OnClickNext);
        _descriptBtn.onClick.AddListener(OnClickDescript);

        (_pageController as PC_Main).OnFrameUpdateAction += UpdateFrame;
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

        _frameAreaDic[UserDataManager.inst.selectedFrameType].SetPics(null);
        _frameAreaDic[UserDataManager.inst.selectedFrameType].UpdateFrame();
    }

    public override void OnPageEnable ()
    {

        _contents[UserDataManager.inst.selectedChromaKeyNum].OnPointerDown(null);
        _contents[UserDataManager.inst.selectedChromaKeyNum].OnPointerClick(null);

        if(this.GetType().GetInterfaces().Contains(typeof(IPageTimeLimit)))
        {
            StartTimer();
        }

        if(_pageInitDone == true)
        {
            ChromaKeyModule.inst.UpdateOption(UserDataManager.inst.selectedContent);
            InitContents();

            FrameEnable();
            UpdateFrame();
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
