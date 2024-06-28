using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToggleContent : Toggle
{
    Image cover;

    protected override void Awake()
    {
        base.Awake();
        cover = transform.GetChild(transform.childCount - 1).GetComponent<Image>();
        onValueChanged.AddListener((state) => CheckState());
    }

    //public override void OnPointerEnter(PointerEventData eventData)
    //{
    //    base.OnPointerEnter(eventData);
    //    cover.enabled = true;
    //}
    //public override void OnPointerExit(PointerEventData eventData)
    //{
    //    base.OnPointerExit(eventData);
    //    cover.enabled = false;
    //}

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        cover.enabled = true;
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        cover.enabled = false;
    }




    //public override void OnPointerUp(PointerEventData eventData)
    //{
    //    base.OnPointerUp(eventData);
    //    cover.enabled = false;
    //}

    public void CheckState()
    {
        if (isOn)
        {
            cover.enabled = true;
        }
        else
        {
            cover.enabled = false;
        }
    }

    public void ChangeState(bool state)
    {
        cover.enabled = state;
    }

    enum eImg
    {
        Bg = 0,
        Cover
    }
}
