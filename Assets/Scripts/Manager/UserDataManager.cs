using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;
using Vivestudios.UI;

public class UserDataManager : SingletonBehaviour<UserDataManager>
{
    [SerializeField]
    private CONTENT_TYPE _selectedContent = CONTENT_TYPE.AI_CARTOON;
    [SerializeField]
    private FRAME_TYPE _selectedFrame = FRAME_TYPE.FRAME_1;
    [SerializeField]
    private LUT_EFFECT_TYPE _selectedLut = LUT_EFFECT_TYPE.LUT_DEFAULT;
    [SerializeField]
    private FRAME_COLOR_TYPE _selectedFrameColor = FRAME_COLOR_TYPE.FRAME_WHITE;
    [SerializeField]
    private FRAME_RATIO_TYPE _frameRatioType = FRAME_RATIO_TYPE.HORIZONTAL;
    [SerializeField]
    private GENDER_TYPE _selectedGender = GENDER_TYPE.FEMALE;

    [SerializeField]
    private bool _isChromaKeyOn = false;
    [SerializeField]
    private int _selectedChromaKeyNum = 0;

    [SerializeField]
    private string _selectedContentKey;
    [SerializeField]
    private string _selectedSubContentKey;
    //[SerializeField]
    //private PROFILE_TYPE _selectedProfileType = PROFILE_TYPE.PR00001;
    [SerializeField]
    private int _selectedProfilePicNum = 0;

    [SerializeField]
    private int _curPicAmount;
    [SerializeField]
    private int _curPrice;

    //private Dictionary<Enum, string> _selectedContentCodeDict = new Dictionary<Enum, string>();
    [SerializeField]
    private string _selectedContentCode;

    public CONTENT_TYPE selectedContent => _selectedContent;
    public FRAME_TYPE selectedFrame => _selectedFrame;
    public FRAME_RATIO_TYPE frameRatioType => _frameRatioType;
    public LUT_EFFECT_TYPE selectedLut => _selectedLut;
    public FRAME_COLOR_TYPE selectedFrameColor => _selectedFrameColor;
    //public PROFILE_TYPE selectedProfileType => _selectedProfileType;
    public GENDER_TYPE selectedGender => _selectedGender;

    public bool isChromaKeyOn => _isChromaKeyOn;
    public int selectedChromaKeyNum => _selectedChromaKeyNum;

    //public int selectedSubContentNum => _selectedSubContentNum;
    public string selectedSubContentKey => _selectedSubContentKey;
    public string selectedContentKey => _selectedContentKey;
    public int selectedProfilePicNum => _selectedProfilePicNum;
    public int curPicAmount => _curPicAmount;
    public int curPrice => _curPrice;

    public string selectedContentCode => _selectedContentCode;


    protected override void Init()
    {
        GameManager.OnGameResetAction += ResetUserData;
    }

    public void ResetUserData()
    {
        _selectedLut = LUT_EFFECT_TYPE.LUT_DEFAULT;
        _selectedFrameColor = FRAME_COLOR_TYPE.FRAME_WHITE;
        _frameRatioType = FRAME_RATIO_TYPE.HORIZONTAL;
        _curPicAmount = Mathf.Min( ConfigData.config.firstPrintAmount, PhotoPaperCheckModule.GetRemainPhotoPaper());
        _selectedChromaKeyNum = 0;
        _isChromaKeyOn = false;
    }

    public void SelectContent(CONTENT_TYPE type)
    {
        _selectedContent = type;
        if (_selectedContent == CONTENT_TYPE.AI_TIME_MACHINE ||
            _selectedContent == CONTENT_TYPE.AI_PROFILE ||
            _selectedContent == CONTENT_TYPE.WHAT_IF)
        {
            SetFrameRatioType(FRAME_RATIO_TYPE.VERTICAL);
        }
        else
        {
            SetFrameRatioType(FRAME_RATIO_TYPE.HORIZONTAL);
        }
    }

    public void SelectFrame(FRAME_TYPE type)
    {
        _selectedFrame = type;
    }

    public void SelectContentCode(string contentCode)
    {
        _selectedContentCode = contentCode;
    }

    public void SetFrameRatioType(FRAME_RATIO_TYPE type)
    {
        _frameRatioType = type;
    }

    public void SetLutEffect(int index)
    {
        _selectedLut = (LUT_EFFECT_TYPE)index;
    }

    public void SetSelectedFrameColor(FRAME_COLOR_TYPE type)
    {
        _selectedFrameColor = type;
    }

    public void SelectSubContent(string subContentKey)
    {
        _selectedSubContentKey = subContentKey;
    }

    public void SelectContent(string contentKey)
    {
        _selectedContentKey = contentKey;
    }

    //public void SelectProfile(PROFILE_TYPE type)
    //{
    //    _selectedProfileType = type;
    //}

    public void SelectProfilePic(int picNum)
    {
        _selectedProfilePicNum = picNum;
    }

    public void SetPicAmount(int picAmount)
    {
        _curPicAmount = picAmount;
    }

    public void SetPrice(int price)
    {
        _curPrice = price;
    }

    public void SetGender(GENDER_TYPE type)
    {
        _selectedGender = type;
    }

    public void SetChromaKeyEnable(bool isEnable) 
    {
        _isChromaKeyOn = isEnable;
    }

    public void SetSelectedChromaKeyNum(int index)
    {
        _selectedChromaKeyNum = index;
    }
}
