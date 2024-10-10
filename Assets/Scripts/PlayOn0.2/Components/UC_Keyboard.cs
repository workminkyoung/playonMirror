using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MPUIKIT;
using UnityEngine.UI;
using DG.Tweening;
using I18N.CJK;
using UnityEditor;
using Unity.VisualScripting;
using Newtonsoft.Json;
using System.Collections.Generic;
using Vivestudios.UI;
using System;

public class UC_Keyboard : UC_BaseComponent
{
    List<ButtonColorChangeTMP> _buttonColorOptions = new List<ButtonColorChangeTMP>();
    List<Button> _buttons = new List<Button>();
    TMP_InputField _inputField;
    
    Button _buttonInitInputField;
    Button _buttonGetInputValue;
    Button _buttonExitKeyboard;


    private MPImage _inputFieldMPImage;
    public Color _buttonOriginTextColor;
    public Color _buttonOriginBGColor;
    public Color _buttonPressedTextColor;
    public Color _buttonPressedBGColor;
    public TMP_FontAsset _buttonTextFont;
    public int _buttonOriginTextFont;
    public int _buttonPressedTextFont;
    public int _normalNotificationOutLineWidth = 0;
    public Color _normalNotificationOutlineColor;
    public int _errorNotificationOutLineWidth = 6;
    public Color _errorNotificationOutlineColor;

    [SerializeField]
    private GameObject Keyboard;
    [SerializeField]
    private Button _buttonDeleteInputWord;
    [SerializeField]
    private GameObject InputField;
    [SerializeField]
    private GameObject CheckInputValueButton;
    [SerializeField]
    private GameObject ExitButton;

    public string _displayInputField = "";

    public override void InitComponent()
    {
        _buttonColorOptions.AddRange(GetComponentsInChildren<ButtonColorChangeTMP>());
        _buttons.AddRange(Keyboard.GetComponentsInChildren<Button>());

        _buttonInitInputField = InputField.GetComponentInChildren<Button>();
        _inputField = InputField.GetComponentInChildren<TMP_InputField>();

        _buttonGetInputValue = CheckInputValueButton.GetComponent<Button>();

        _inputFieldMPImage = InputField.GetComponent<MPImage>();

        _buttonExitKeyboard = ExitButton.GetComponentInChildren<Button>();


        for (int i = 0; i < _buttonColorOptions.Count; i++)
        {
            _buttonColorOptions[i].Initsetting();
            _buttonColorOptions[i].SetTextOptions(_buttonOriginTextColor, _buttonOriginTextFont, _buttonPressedTextColor, _buttonPressedTextFont);
            _buttonColorOptions[i].SetBackgroundOptions(_buttonOriginBGColor, _buttonPressedBGColor);
        }

        for (int i = 0; i < _buttons.Count; i++)
        {
            TextMeshProUGUI keyButton = _buttons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (keyButton != null)
            {
                _buttons[i].onClick.AddListener(() => OnKeyClick(keyButton.text));
            }
        }
        _buttonDeleteInputWord.onClick.AddListener(() => DeleteCharBeforeCursor());
        _buttonInitInputField.onClick.AddListener(() => InitInputField());
        _buttonGetInputValue.onClick.AddListener(() => GetInputValue());
        _buttonExitKeyboard.onClick.AddListener(() => ExitKeyboard());
    }

    void OnKeyClick(string key)
    {
        _inputField.text += key;
        _displayInputField = _inputField.text;
    }

    void DeleteCharBeforeCursor()
    {
        string text = _inputField.text;
        int cursorPosition = text.Length;
        if (cursorPosition > 0)
        {
            text = text.Remove(cursorPosition - 1, 1);

            _inputField.text = text;
            _inputField.caretPosition = cursorPosition - 1;
        }
    }

    void InitInputField()
    {
        _inputField.text = "";
        _displayInputField = "";
    }

    void GetInputValue()
    {
        var data = new Dictionary<string, string>
        {
            { "coupon_number", _inputField.text },
            { "uuid", LogDataManager.inst.GetGuid }
        };
        string json = JsonConvert.SerializeObject(data);
        string url = ApiCall.inst.CouponAPIUrl;
        ApiCall.Instance.Post(url, json, GetResponse);
    } 

    public void UseKeyboard()
    {
        gameObject.SetActive(true);
    }

    public void ExitKeyboard()
    {
        InitInputField();
        gameObject.SetActive(false);
    }

    bool ValidateResponse()
    {
        bool _isValidate = true;
        CouponValidataResponse _response = UserDataManager.Instance.getvalidataResponse;
        if (_response.is_used)
        {
            GameManager.Instance.globalPage.OpenToast("이미 사용된 쿠폰입니다", 3);
            _isValidate = false;
        }
        if (_response.is_expired)
        {
            GameManager.Instance.globalPage.OpenToast("사용 기간이 만료되었습니다", 3);
            _isValidate = false;
        }
        if (_response.is_matched_uuid) // TODO : || _response.is_empty
        {
            GameManager.Instance.globalPage.OpenToast("쿠폰번호를 다시 입력해주세요", 3);
            _isValidate = false;
        }
        return _isValidate;
    }

    public void ErrorNotification(bool RaiseError) //TODO : 어디에 어떻게 적용할지 논의 필요함
    {
        if (RaiseError)
        {
            _inputFieldMPImage.OutlineWidth = _errorNotificationOutLineWidth;
            _inputFieldMPImage.OutlineColor = _errorNotificationOutlineColor;
        }
        else
        {
            _inputFieldMPImage.OutlineWidth = _normalNotificationOutLineWidth;
            _inputFieldMPImage.OutlineColor = _normalNotificationOutlineColor;
        }
    } 

    void GetResponse(string result)
    {
        if (result.Contains("is_used"))
        {
            CouponValidataResponse response = JsonConvert.DeserializeObject<CouponValidataResponse>(result);
            UserDataManager.Instance.SetCouponValidata(response);
            if (ValidateResponse())
            { 
                UserDataManager.inst.SetCouponInfo(_inputField.text);
                ExitKeyboard();
            }
        }
        else // TODO : 422 없앨 예정
        {
            GameManager.Instance.globalPage.OpenToast("쿠폰번호를 다시 입력해주세요", 3);
        }
    }
}