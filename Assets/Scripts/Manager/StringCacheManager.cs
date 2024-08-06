using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;
using System;
using Vivestudios.UI;
using static UnityEditor.Progress;

public class StringCacheManager : SingletonBehaviour<StringCacheManager>
{
    [SerializeField]
    private ContentStringDicBase _contentKey;
    [SerializeField]
    private CartoonStringDicBase _cartoonTitles;
    [SerializeField]
    private CartoonStringDicBase _cartoonDescriptions;
    [SerializeField]
    private string[] _loadingTexts;
    [SerializeField]
    private string[] _loadingTextsWhatIf;

    [SerializeField]
    private ProfileStringDicBase _profileTitles;
    [SerializeField]
    private ProfileStringDicBase _profileDescriptions;
    [SerializeField]
    private List<string> _profileWhatIfName;
    [SerializeField]
    private ContentStringDicBase _contentTitles;
    [SerializeField]
    private ContentStringDicBase _contentDescriptions;
    [SerializeField]
    private ContentStringDicBase _contentPlayerNums;
    [SerializeField]
    private FilterStringDicBase _filterDescriptions;
    private string _dividerLine = "-----------------------------------------";
    private string _pointLine = "##### ";

    public string[] loadingTexts => _loadingTexts;
    public string[] loadingTextsWhatIf => _loadingTextsWhatIf;
    public List<string> ProfileWhatIfName => _profileWhatIfName;
    public string DividerLine => _dividerLine;
    public string PointLine => _pointLine;

    public string GetContentKey(CONTENT_TYPE type)
    {
        return _contentKey[type];
    }
    
    public CONTENT_TYPE GetContentType(string key)
    {
        foreach (var item in _contentKey)
        {
            if(item.Value == key)
            {
                return item.Key;
            }
        }
        return CONTENT_TYPE.AI_CARTOON;
    }

    public string GetCartoonTitle(CARTOON_TYPE type)
    {
        if (!_cartoonTitles.ContainsKey(type))
        {
            return string.Empty;
        }
        return _cartoonTitles[type];
    }

    public string GetCartoonDescription(CARTOON_TYPE type)
    {
        if (!_cartoonDescriptions.ContainsKey(type))
        {
            return string.Empty;
        }
        return _cartoonDescriptions[type];
    }

    public string GetProfileTitle(PROFILE_TYPE type)
    {
        if (!_profileTitles.ContainsKey(type))
        {
            return string.Empty;
        }
        return _profileTitles[type];
    }

    public string GetProfileDescription(PROFILE_TYPE type)
    {
        if (!_profileDescriptions.ContainsKey(type))
        {
            return string.Empty;
        }
        return _profileDescriptions[type];
    }

    public string GetContentTitle(CONTENT_TYPE type)
    {
        return _contentTitles[type];
    }

    public string GetContentDescription(CONTENT_TYPE type)
    {
        return _contentDescriptions[type];
    }

    public string GetContentPlayerNum(CONTENT_TYPE type)
    {
        return _contentPlayerNums[type];
    }

    public string GetFilterDescription(CONTENT_TYPE type)
    {
        return _filterDescriptions[type];
    }

    protected override void Init()
    {
    }

    [Serializable]
    private class CartoonStringDicBase : SerializableDictionaryBase<CARTOON_TYPE, string> { }
    [Serializable]
    private class ProfileStringDicBase : SerializableDictionaryBase<PROFILE_TYPE, string> { }
    [Serializable]
    private class ContentStringDicBase : SerializableDictionaryBase<CONTENT_TYPE, string> { }
    [Serializable]
    private class FilterStringDicBase : SerializableDictionaryBase<CONTENT_TYPE, string> { }
}
