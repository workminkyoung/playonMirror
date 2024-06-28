using MPUIKIT;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.Video;

public partial class UC_StyleContent : UC_SelectableContent, IPointerUpHandler
{
    [SerializeField]
    private TextMeshProUGUI _titleText = null;
    [SerializeField]
    private TextMeshProUGUI _descriptionText = null;
    [SerializeField]
    private TextMeshProUGUI _maxPlayerText = null;

    public override void InitComponent()
    {
        base.InitComponent();
    }

    public void SetTitle(string title)
    {
        title = title.Replace("\\n", "\n");
        _titleText.text = title;
    }

    public void SetDescription(string description)
    {
        description = description.Replace("\\n", "\n");
        _descriptionText.text = description;
    }

    public void SetMaxPlayer(string maxPlayer)
    {
        maxPlayer = maxPlayer.Replace("\\n", "\n");
        _maxPlayerText.text = maxPlayer;
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        Select(false);
    }
}
