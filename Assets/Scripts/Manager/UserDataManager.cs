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
    private FRAME_RATIO_TYPE _frameRatioType = FRAME_RATIO_TYPE.HORIZONTAL;
    [SerializeField]
    private GENDER_TYPE _selectedGender = GENDER_TYPE.FEMALE;
    [SerializeField]
    private FRAME_TYPE _selectedFrameType = FRAME_TYPE.FRAME_1;

    // Content Key Data
    [SerializeField]
    private string _selectedContentKey;//CA, PR, BT...
    [SerializeField]
    private string _selectedSubContentKey;//CA00001...

    // Frame Key Data
    [SerializeField]
    private string _selectedFrameKey;
    [SerializeField]
    private string _selectedLutKey = string.Empty;
    [SerializeField]
    private string _selectedFrameColorKey;// = FRAME_COLOR_TYPE.FRAME_WHITE;
    [SerializeField]
    private string _defaultFrameColorKey = string.Empty;// = FRAME_COLOR_TYPE.FRAME_WHITE;

    // Chromakey Data
    [SerializeField]
    private bool _isChromaKeyOn = false;
    [SerializeField]
    private int _selectedChromaKeyNum = 0;

    // Result Data
    [SerializeField]
    private int _selectedProfilePicNum = 0;
    [SerializeField]
    private int _curPicAmount;
    [SerializeField]
    private int _curPrice;

    [SerializeField]
    private FrameData.DefinitionEntry _selectedFrameDefinition;

    public CONTENT_TYPE selectedContent => _selectedContent;
    public FRAME_TYPE selectedFrameType => _selectedFrameType;
    public FRAME_RATIO_TYPE frameRatioType => _frameRatioType;
    public GENDER_TYPE selectedGender => _selectedGender;

    // Content Key Data
    public string selectedSubContentKey => _selectedSubContentKey;
    public string selectedContentKey => _selectedContentKey;
    public int curPrice => _curPrice;

    // Frame Key Data
    public string selectedFrameKey => _selectedFrameKey;
    public string selectedFrameColor => _selectedFrameColorKey;
    public string defaultFrameColor => _defaultFrameColorKey;
    public FrameData.DefinitionEntry selectedFrameDefinition => _selectedFrameDefinition;

    // Chromakey Data
    public int selectedChromaKeyNum => _selectedChromaKeyNum;
    public bool isChromaKeyOn => _isChromaKeyOn;

    // Result Data
    public int selectedProfilePicNum => _selectedProfilePicNum;
    public int curPicAmount => _curPicAmount;
    public string selectedLutKey => _selectedLutKey;

    protected override void Init()
    {
        GameManager.OnGameResetAction += ResetUserData;
    }

    public void ResetUserData()
    {
        //_selectedLut = LUT_EFFECT_TYPE.LUT_DEFAULT;
        _selectedFrameColorKey = _defaultFrameColorKey;
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

    public void SelectFrameKey(string key)
    {
        _selectedFrameKey = key;
    }

    public void SelectFrameType(FRAME_TYPE type)
    {
        _selectedFrameType = type;
    }

    public void SetFrameRatioType(FRAME_RATIO_TYPE type)
    {
        _frameRatioType = type;
    }

    public void SetLutEffect(string lut)
    {
        _selectedLutKey = lut;
    }

    public void SetSelectedFrameColor(string key)
    {
        _selectedFrameColorKey = key;
    }

    public void SetDefaultFrameColor(string key)
    {
        _defaultFrameColorKey = key;
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

    public void SetSelectedFrameDefinition(FrameData.DefinitionEntry entry)
    {
        _selectedFrameDefinition = entry;
    }
}
