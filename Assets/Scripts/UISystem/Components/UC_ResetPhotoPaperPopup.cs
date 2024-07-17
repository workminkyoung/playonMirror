using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vivestudios.UI;

public class UC_ResetPhotoPaperPopup : UC_BaseComponent
{
    [SerializeField]
    private Toggle[] _toggles;
    [SerializeField]
    private ToggleGroup _toggleGroup;

    [SerializeField]
    private Button _cancelBtn, _confirmBtn;

    public Action OnConfirmAction;
    public Action OnCancelAction;

    private int _selectedPhotopaperNum = 0;
    public int selectedPhotopaperNum => _selectedPhotopaperNum;

    public override void InitComponent()
    {
        for (int i = 0; i < _toggles.Length; i++)
        {
            int num = int.Parse(Regex.Replace(_toggles[i].GetComponentInChildren<TextMeshProUGUI>().text, @"\D", ""));
            int index = i;
            _toggles[i]?.onValueChanged.AddListener((value) =>
            {
                ToggleCheck(index, value, num);
            });
        }

        _cancelBtn?.onClick.AddListener(() => OnCancelAction?.Invoke());
        _confirmBtn?.onClick.AddListener(() => OnConfirmAction?.Invoke());
    }

    private void OnEnable()
    {
        _toggleGroup.allowSwitchOff = false;

        for (int i = 0; i < _toggles.Length; i++)
        {
            int index = i;

            if (index == 0)
            {
                _toggles[index].isOn = true;
                ToggleCheck(0, true, int.Parse(Regex.Replace(_toggles[index].GetComponentInChildren<TextMeshProUGUI>().text, @"\D", "")));
            }
            else
            {
                _toggles[index].isOn = false;
                ToggleCheck(index, false);
            }
        }

        _toggleGroup.allowSwitchOff = true;
    }

    private void ToggleCheck(int index, bool value, int num = 0)
    {
        _toggles[index].GetComponentInChildren<TextMeshProUGUI>().color = value ? Color.white : Color.black;
        if (value == true)
        {
            SetResetPhotopaperNum(num);
        }
    }

    private void SetResetPhotopaperNum(int num)
    {
        CustomLogger.Log($"Photo Paper Remain Will Set : {num}");
        _selectedPhotopaperNum = num;
    }
}
