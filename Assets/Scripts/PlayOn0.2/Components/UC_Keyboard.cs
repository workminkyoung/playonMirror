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

public class UC_Keyboard : MonoBehaviour
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
    //, int outLineWidth = 6, string OutlineColor = "#FF4B4B"

    void Start()
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
            Debug.Log(i );//+ _buttons[i].GetComponentInChildren<TextMeshProUGUI>().text
            _buttonColorOptions[i].SetTextOptions(_buttonOriginTextColor, _buttonOriginTextFont, _buttonPressedTextColor, _buttonPressedTextFont);
            _buttonColorOptions[i].SetBackgroundOptions(_buttonOriginBGColor, _buttonPressedBGColor);
        }

        for (int i = 0; i < _buttons.Count; i++)
        {
            //_btns[i].colors  = btnColor;
            TextMeshProUGUI keyButton = _buttons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (keyButton != null)
            {
                _buttons[i].onClick.AddListener(() => OnKeyClick(keyButton.text));
            }
        }
        _buttonDeleteInputWord.onClick.AddListener(()=> DeleteCharBeforeCursor());
        _buttonInitInputField.onClick.AddListener(() => InitInputField());
        _buttonGetInputValue.onClick.AddListener(() => GetInputValue());
        _buttonExitKeyboard.onClick.AddListener(() => ExitKeyboard());
    }


    void OnKeyClick(string key)
    {
        _inputField.text += key;
        _displayInputField = _inputField.text;
        Debug.Log(_inputField.text);
    }

    void DeleteCharBeforeCursor()
    {
        string text = _inputField.text;
        int cursorPosition = text.Length;
        if (cursorPosition > 0)
        {
            text = text.Remove(cursorPosition - 1, 1);

            // 텍스트를 업데이트하고 커서 위치를 하나 앞으로 이동
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
        // Coupon number check 하는 코드 작성
        Debug.Log(_inputField.text);
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

    public void ErrorNotification(bool RaiseError) //INSPECTOR 로
    {
        if (RaiseError)
        {
            _inputFieldMPImage.OutlineWidth = _errorNotificationOutLineWidth;
            _inputFieldMPImage.OutlineColor = _errorNotificationOutlineColor;
        };
    }
}
