using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Vivestudios.UI;

public class UC_Tip : UC_BaseComponent
{
    List<TextMeshProUGUI> _texts = new List<TextMeshProUGUI>();

    public override void InitComponent()
    {
        _texts.AddRange(GetComponentsInChildren<TextMeshProUGUI>());
    }
}
