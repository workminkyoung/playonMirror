using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vivestudios.UI;

public class PC_BasePageController : MonoBehaviour
{
    [SerializeField]
    private UP_BasePage[] _pages = null;
    protected Dictionary<PAGE_TYPE, UP_BasePage> _pageDic = new Dictionary<PAGE_TYPE, UP_BasePage>();

    protected virtual void Awake()
    {
        _pages = GetComponentsInChildren<UP_BasePage>(true);

        foreach (var elem in _pages)
        {
            elem.SetPageController(this);
            elem.AwakePage();
        }

        ChangePage(_pageDic.First().Key);
    }

    protected virtual void GetPages()
    {

    }

    public void AddPage(UP_BasePage page)
    {
        if (!_pageDic.ContainsKey(page.pageType))
        {
            _pageDic.Add(page.pageType, page);
        }
    }

    public void ChangePage(PAGE_TYPE type)
    {
        if (type == PAGE_TYPE.PAGE_GLOBAL)
            return;
        if (!_pageDic.ContainsKey(type))
            return;
        foreach (PAGE_TYPE elem in _pageDic.Keys)
        {
            if (elem == PAGE_TYPE.PAGE_GLOBAL)
                continue;
            if (elem == type)
                continue;
            _pageDic[elem].EnablePage(false);
        }
        _pageDic[type].EnablePage(true);
    }
}
