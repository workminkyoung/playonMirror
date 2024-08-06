using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Vivestudios.UI;

public abstract class UP_BaseSelectContent : UP_BasePage
{
    [SerializeField]
    [ReadOnly]
    protected UC_SelectableContent[] _contents = null;
    [SerializeField]
    protected List<Transform> _contentParents = new List<Transform>();
    [SerializeField]
    protected List<Transform> _shuffledContentParents;

    private Coroutine _timerCoroutine = null;

    [SerializeField]
    protected int _maxTime;
    [SerializeField]
    protected TextMeshProUGUI _timeText;

    public override void InitPage()
    {
        _contents = GetComponentsInChildren<UC_SelectableContent>(true);
        for (int i = 0; i < _contents.Length; i++)
        {
            _contentParents.Add(_contents[i].transform.parent);
        }

        _shuffledContentParents = _contentParents.ToList();
    }
    public override void BindDelegates()
    {
        for (int i = 0; i < _contents.Length; i++)
        {
            int index = i;
            _contents[i].pointerClickAction += () => OnClickContent(index);
        }

        (_pageController as PC_Main).OnShuffleAction += ShuffleContents;
    }

    protected virtual void OnClickContent(int index) { }

    protected virtual void ShuffleContents()
    {
        ShuffleList(_shuffledContentParents);

        for (int i = 0; i < _contents.Length; i++)
        {
            _contents[i].transform.SetParent(_shuffledContentParents[i]);
            (_contents[i].transform as RectTransform).anchoredPosition = Vector2.zero;
        }
    }

    private List<T> ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int random = UnityEngine.Random.Range(0, i);

            T temp = list[i];
            list[i] = list[random];
            list[random] = temp;
        }

        return list;
    }

    public void StartTimer()
    {
        ResetTimer();
        _timerCoroutine = StartCoroutine(TimerRoutine());
    }

    public void ResetTimer()
    {
        if (_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
        }
    }

    private IEnumerator TimerRoutine()
    {
        _timeText.text = _maxTime.ToString();

        int time = 0;

        while (time < _maxTime)
        {
            yield return new WaitForSecondsRealtime(1);
            time++;

            _timeText.text = (_maxTime - time).ToString();

            if (_maxTime - time == 5)
            {
                (_pageController as PC_Main)?.globalPage?.OpenTimerToast(5);
            }
        }

        (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_AOD);
    }

    protected virtual void OnEnable()
    {
        if (!_pageController)
            return;
        if (this.GetType().GetInterfaces().Contains(typeof(IPageTimeLimit)))
        {
            StartTimer();
        }
    }

    protected virtual void OnDisable()
    {
        if (this.GetType().GetInterfaces().Contains(typeof(IPageTimeLimit)))
        {
            ResetTimer();
        }

        (_pageController as PC_Main)?.globalPage?.CloseToast();
    }
}
