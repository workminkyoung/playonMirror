using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RotaryHeart.Lib.SerializableDictionary;
using BasicData;

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
        public ConfigEntryDic Config { get; set; }
        public FontSetEntryDic FontSet { get; set; }
        public List<CategoryEntry> Category { get; set; }
        public BubbleTableEntryDic BubbleTable { get; set; }

        [Serializable]
        public class ConfigEntryDic : SerializableDictionaryBase<string, ConfigEntry> { }
        [Serializable]
        public class FontSetEntryDic : SerializableDictionaryBase<string, ConfigEntry> { }
        [Serializable]
        public class BubbleTableEntryDic : SerializableDictionaryBase<string, BubbleTableEntry> { }

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
    public class CategoryEntry
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
        public ConfigEntry Config { get; set; }
        public FilterTableEntryDic FilterTable { get; set; }

        [Serializable]
        public class FilterTableEntryDic : SerializableDictionaryBase<string, FilterTableEntry> { }
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
        public ConfigEntry Config { get; set; }
        public ContentsEntryDic Contents { get; set; }
        public ContentsDetailEntryDic ContentsDetail { get; set; }

        [Serializable]
        public class ContentsEntryDic : SerializableDictionaryBase<string, ContentsEntry> { }
        [Serializable]
        public class ContentsDetailEntryDic : SerializableDictionaryBase<string, ContentsDetailEntry> { }
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
        public ConfigEntry Config { get; set; }
        public DeviceEntryDic Device { get; set; }

        [Serializable]
        public class DeviceEntryDic : SerializableDictionaryBase<string, DeviceEntry> { }
    }

    [Serializable]
    public class ConfigEntry
    {
        public string BTBG { get; set; }
        public string CABG { get; set; }
        public string CAMenu { get; set; }
        public string PRMenu { get; set; }
        public string WFMenu { get; set; }
        public string BGImage { get; set; }
        public string EndImage { get; set; }
        public string Printing { get; set; }
        public string ColorCode { get; set; }
        public string OtherMenu { get; set; }
        public string PayConfirm { get; set; }
        public string StartImage { get; set; }
        public string DefaultUsed { get; set; }
        public string FrameSelect { get; set; }
        public string OptionalUse { get; set; }
        public string VideoVolume { get; set; }
        public string Age14TermUse { get; set; }
        public string ContentsMenu { get; set; }
        public string PhotoStandby { get; set; }
        public string Age14TermUsed { get; set; }
        public string PlayShotMovie { get; set; }
        public string PaymentTermUse { get; set; }
        public string PaymentTermUsed { get; set; }
        public string PrintErrorImage { get; set; }
        public string StartMediaVideo { get; set; }
        public string MarketingTermUse { get; set; }
        public string MultiLanguageUse { get; set; }
        public List<string> PaymentTermImage { get; set; }
        public List<string> ServiceTermImage { get; set; }
        public string ServieErrorImage { get; set; }
        public string ShootingPRSelect { get; set; }
        public string MarketingTermUsed { get; set; }
        public List<string> MarketingTermImage { get; set; }
        public List<string> PersonalPolicyImage { get; set; }
    }

    [Serializable]
    public class DeviceEntry
    {
        public string Key { get; set; }
        public string TID { get; set; }
        public string Deploy { get; set; }
        public string MailList { get; set; }
        public string MachineID { get; set; }
        public string StoreName { get; set; }
        public string PrinterModel { get; set; }
    }

}

namespace ChromakeyFrameData
{
    [Serializable]
    public class ChromakeyFrame
    {
        public ConfigEntryDic Config;
        public List<CategoryEntry> Category;
        public ChromakeyToneTableEntryDic ChromakeyToneTable;
        public ChromakeyFrameTableDic ChromakeyFrameTable;

        [Serializable]
        public class ConfigEntryDic : SerializableDictionaryBase<string, ConfigEntry> { }
        [Serializable]
        public class ChromakeyToneTableEntryDic : SerializableDictionaryBase<string, ChromakeyToneTableEntry> { }
        [Serializable]
        public class ChromakeyFrameTableDic : SerializableDictionaryBase<string, ChromakeyToneTableEntry> { }
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