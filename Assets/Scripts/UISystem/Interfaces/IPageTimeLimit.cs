using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface IPageTimeLimit
{
    public TextMeshProUGUI timeText { get; set; }
    public int MaxTime { get; set; }
}
