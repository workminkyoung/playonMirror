using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RotaryHeart.Lib.SerializableDictionary;

[System.Serializable]
public class ConfigDefaultData
{
    public int id { get; set; }
    public ConfigDefaultSet config_default_set { get; set; }
}

[System.Serializable]
public class ConfigDefaultSet
{
    public Result result { get; set; }
}

[Serializable]
public class Result
{
    public object BubbleData { get; set; }
    public object FilterData { get; set; }
    public object ServiceData { get; set; }
    public object BasicSetting { get; set; }
    public object ChromakeyFrame { get; set; }
    public object ShootingScreen { get; set; }
}

namespace BubbleData
{
    [Serializable]
    public class BubbleData
    {
        public object Config { get; set; }
        public object FontSet { get; set; }
        public object Category { get; set; }
        public object BubbleTable { get; set; }
    }

    [Serializable]
    public class ConfigEntry
    {
        public string Key { get; set; }
        public string value1 { get; set; }
        public string value2 { get; set; }
        public string value3 { get; set; }
        public string value4 { get; set; }
        public string value5 { get; set; }
        public string value6 { get; set; }
        public string value7 { get; set; }
        public string value8 { get; set; }
        public string value9 { get; set; }
        public string value10 { get; set; }
    }

    [Serializable]
    public class FontSetEntry
    {
        public string Key { get; set; }
        public string Korean { get; set; }
        public string Chinese { get; set; }
        public string English { get; set; }
    }

    [Serializable]
    public class BubbleTableEntry
    {
        public string Key { get; set; }
        public string Kind { get; set; }
        public string Group { get; set; }
        public string Image { get; set; }
        public string Korean { get; set; }
        public string Margin { get; set; }
        public string Chinese { get; set; }
        public string English { get; set; }
        public string FontSet { get; set; }
        public string Category { get; set; }
        public string FontSize { get; set; }
        public string Sequence { get; set; }
        public string TextArea { get; set; }
        public string FontColor { get; set; }
        public string ImageType { get; set; }
        public string Thumbnail { get; set; }
        public string StartScale { get; set; }
        public string ThumbnailScale { get; set; }
    }
}

namespace FilterData
{
    [Serializable]
    public class FilterData
    {
        public object Config { get; set; }
        public object FilterTable { get; set; }
    }

    [Serializable]
    public class ConfigEntry
    {
        public string Kernal { get; set; }
        public string SigmaB { get; set; }
        public string SigmaS { get; set; }
        public string Sorting { get; set; }
        public string UsePage { get; set; }
        public string BilateralDefaultCheck { get; set; }
    }

    [Serializable]
    public class FilterTableEntry
    {
        public string Key { get; set; }
        public string File { get; set; }
        public string Used { get; set; }
        public string Korean { get; set; }
        public string Chinese { get; set; }
        public string English { get; set; }
        public string Sequence { get; set; }
        public string Thumbnail { get; set; }
    }

}

namespace ServiceData
{
    [Serializable]
    public class ServiceData
    {
        public object Config { get; set; }
        public object Contents { get; set; }
        public object ContentsDetail { get; set; }

    }

    [Serializable]
    public class ConfigEntry
    {
        public string Sorting { get; set; }
    }

    [Serializable]
    public class ContentsEntry
    {
        public string Key { get; set; }
        public string Use { get; set; }
        public string Sorting { get; set; }
        public string Sequence { get; set; }
        public string Thumbnail { get; set; }
        public string Korean_Title { get; set; }
        public string Chinese_Title { get; set; }
        public string English_Title { get; set; }
        public string Korean_People { get; set; }
        public string Chinese_People { get; set; }
        public string English_People { get; set; }
        public string Korean_SubText { get; set; }
        public string Chinese_SubText { get; set; }
        public string English_SubText { get; set; }
        public string ShootGuideImage { get; set; }
    }

    [Serializable]
    public class ContentsDetailEntry
    {
        public string Key { get; set; }
        public string Use { get; set; }
        public string Model { get; set; }
        public string Gender { get; set; }
        public string Category { get; set; }
        public string Property { get; set; }
        public string Sequence { get; set; }
        public string Thumbnail { get; set; }
        public string References { get; set; }
        public string KoreanTitle { get; set; }
        public string ChineseTitle { get; set; }
        public string EnglishTitle { get; set; }
        public string KoreanSubtext { get; set; }
        public string ChineseSubtext { get; set; }
        public string EnglishSubtext { get; set; }
    }
}

namespace BasicData
{
    [Serializable]
    public class BasicSetting
    {
        public object Config { get; set; }
        public object Device { get; set; }
    }

    [Serializable]
    public class ConfigEntry
    {
        public string Key { get; set; }
        public string value1 { get; set; }
        public string value2 { get; set; }
        public string value3 { get; set; }
        public string value4 { get; set; }
        public string value5 { get; set; }
        public string value6 { get; set; }
        public string value7 { get; set; }
        public string value8 { get; set; }
        public string value9 { get; set; }
        public string value10 { get; set; }
    }

    [Serializable]
    public class CategoryEntry
    {
        public string Key { get; set; }
        public string Korean { get; set; }
        public string Chinese { get; set; }
        public string English { get; set; }
    }

    [Serializable]
    public class ChromakeyToneTableEntry
    {
        public string Key { get; set; }
        public string Color { get; set; }
        public string Korean { get; set; }
        public string Chinese { get; set; }
        public string English { get; set; }
        public string Sequence { get; set; }
        public string Thumbnail { get; set; }
    }

    [Serializable]
    public class ChromakeyFrameTableEntry
    {
        public string Key { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string Image3 { get; set; }
        public string Image4 { get; set; }
        public string Image5 { get; set; }
        public string Image6 { get; set; }
        public string Image7 { get; set; }
        public string Image8 { get; set; }
        public string Korean { get; set; }
        public string Chinese { get; set; }
        public string English { get; set; }
        public string Category { get; set; }
        public string Sequence { get; set; }
        public string Thumbnail { get; set; }
    }
}

namespace ShootingScreenData
{
    [Serializable]
    public class ShootScreenEntryDic : SerializableDictionaryBase<string, string> { }
}