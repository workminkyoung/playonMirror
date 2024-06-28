using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandlePointer : Selectable
{
    List<Image> images = new List<Image>();

    public void Setting()
    {
        images.AddRange(UtilityExtensions.GetComponentsOnlyInChildren_NonRecursive<Image>(transform));
    }

    public void Init()
    {
        for (int i = 0; i < images.Count; i++)
        {
            images[i].enabled = false;
        }
    }

    public void SetState(bool isEnable)
    {
        for (int i = 0; i < images.Count; i++)
        {
            images[i].enabled = false;
        }

        if (isEnable)
            images[(int)eImg.On].enabled = true;
        else
            images[(int)eImg.Off].enabled = true;
    }

    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    if (images[(int)eImg.Off].enabled)
    //        return;
    //    images[(int)eImg.On].enabled = false;
    //    images[(int)eImg.Enter].enabled = true;
    //}

    //public void OnPointerUp(PointerEventData eventData)
    //{
    //    if (images[(int)eImg.Off].enabled)
    //        return;
    //    images[(int)eImg.On].enabled = true;
    //    images[(int)eImg.Enter].enabled = false;
    //}

    //public override void OnPointerEnter(PointerEventData eventData)
    //{
    //    base.OnPointerEnter(eventData);

    //}

    //public override void OnPointerExit(PointerEventData eventData)
    //{
    //    base.OnPointerExit(eventData);
    //}

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (images[(int)eImg.Off].enabled)
            return;
        images[(int)eImg.On].enabled = false;
        images[(int)eImg.Enter].enabled = true;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        if (images[(int)eImg.Off].enabled)
            return;
        images[(int)eImg.On].enabled = true;
        images[(int)eImg.Enter].enabled = false;
    }


    enum eImg
    {
        Off = 0,
        On, 
        Enter
    }
}
