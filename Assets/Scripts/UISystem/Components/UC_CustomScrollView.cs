using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UC_CustomScrollView : ScrollRect
{
    protected override void OnEnable()
    {
        base.OnEnable();

        content.anchoredPosition = Vector2.zero;
    }
}
