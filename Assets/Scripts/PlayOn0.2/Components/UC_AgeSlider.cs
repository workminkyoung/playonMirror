using MPUIKIT;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vivestudios.UI;

public class UC_AgeSlider : UC_BaseComponent
{
    [SerializeField]
    private Image _sliderBG;
    [SerializeField]
    private Sprite _spriteSliderOn;
    [SerializeField]
    private Sprite _spriteSliderOff;
    //config로 바꿔서 넣어두기
    [SerializeField]
    private int[] _sliderSteps;
    [SerializeField]
    private TextMeshProUGUI _textIndex;
    [SerializeField]
    private Color _textOn, _textOff;

    private Slider _slider;
    private MPImage _mpHandle;
    private RectTransform _defaultHandle;
    private int _index;

    public Slider slider { get { return _slider; } }
    public int index { set 
        { 
            _index = value;
            GetComponentInChildren<TextMeshProUGUI>().text = (_index + 1).ToString("00");
        } }
    public int[] sliderSteps {  set { _sliderSteps = value; } }

    public override void InitComponent()
    {
        //_textNum = UtilityExtensions.GetComponentOnlyInChildren_NonRecursive<TextMeshProUGUI>(transform);
        _slider = GetComponentInChildren<Slider>();
        _mpHandle = GetComponentInChildren<MPImage>();
        _defaultHandle = _slider.handleRect;

        _slider.onValueChanged.AddListener(OnSliderChange);
    }

    public override void SetActivate(bool state)
    {
        if (_slider == null)
            return;

        if (state)
        {
            _sliderBG.sprite = _spriteSliderOn;
            _mpHandle.gameObject.SetActive(true);
            _textIndex.color = _textOn;

            //_slider.handleRect = _mpHandle.rectTransform;
        }
        else
        {
            _sliderBG.sprite = _spriteSliderOff;
            _mpHandle.gameObject.SetActive(false);
            _textIndex.color = _textOff;

            //_slider.handleRect = _defaultHandle;
        }

        _slider.enabled = state;
    }

    public void InitSlider()
    {
        _slider.value = 2;
        OnSliderChange(2);
    }
    
    void OnSliderChange(float value)
    {
        AgeData age = new AgeData();

        int reValue = _sliderSteps[(int)value];
        

        if (reValue >= 0)
        {
            age.target = reValue;// (int)value;
            age.source = 20;
        }
        else
        {
            age.source = reValue * -1;// (int)value * -1;
            age.target = 20;
        }

        CustomLogger.Log($"value is {value}, revalue is {reValue}");
        Dictionary<string, object> sliderData = new Dictionary<string, object>();
        sliderData.Add(_index.ToString(), age);

        string json = JsonConvert.SerializeObject(sliderData);
        UDPClient.Instance.SendData(json);
    }

}
public class AgeData
{
    public int source = 0;
    public int target = 0;
}
