using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vivestudios.UI;

public class UC_DownloadLoading : UC_BaseComponent
{
    [SerializeField]
    private RectTransform _icon;

    public override void InitComponent()
    {
    }

    public override void SetActivate(bool state)
    {
        base.SetActivate(state);

        if (state)
        {
            _icon.DOLocalRotate(new Vector3(0, 0, 360), 1).SetLoops(-1, LoopType.Restart);
        }
        else
        {
            _icon.DOKill();
        }
    }
}
