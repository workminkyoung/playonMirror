using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RotaryHeart.Lib.SerializableDictionary;
using BasicData;
using System.IO;

[System.Serializable]
public class ConfigDefaultData
{
    public int id;
    public ConfigDefaultSet config_default_set;
}

[System.Serializable]
public class ConfigDefaultSet
{
    public Result result;
}

[Serializable]
public class Result
{
    public object BubbleData;
    public object FilterData;
    public object ServiceData;
    public object BasicSetting;
    public object ChromakeyFrame;
    public object ShootingScreen;
}

namespace BubbleData
{
    [Serializable]
    public class BubbleData
    {
        public ConfigEntryDic Config;
        public FontSetEntryDic FontSet;
        public List<CategoryEntry> Category;
        public BubbleTableEntryDic BubbleTable;

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
        public string Key;
        public string value1;
        public string value2;
        public string value3;
        public string value4;
        public string value5;
        public string value6;
        public string value7;
        public string value8;
        public string value9;
        public string value10;
    }

    [Serializable]
    public class FontSetEntry
    {
        public string Key;
        public string Korean;
        public string Chinese;
        public string English;
    }

    [Serializable]
    public class CategoryEntry
    {
        public string Key;
        public string Korean;
        public string Chinese;
        public string English;
    }

    [Serializable]
    public class BubbleTableEntry
    {
        public string Key;
        public string Kind;
        public string Group;
        public string Image;
        public string Korean;
        public string Margin;
        public string Chinese;
        public string English;
        public string FontSet;
        public string Category;
        public string FontSize;
        public string Sequence;
        public string TextArea;
        public string FontColor;
        public string ImageType;
        public string Thumbnail;
        public string StartScale;
        public string ThumbnailScale;
    }
}

namespace FilterData
{
    [Serializable]
    public class FilterData
    {
        public ConfigEntry Config;
        public FilterTableEntryDic FilterTable;

        [Serializable]
        public class FilterTableEntryDic : SerializableDictionaryBase<string, FilterTableEntry> { }
    }

    [Serializable]
    public class ConfigEntry
    {
        public string Kernal;
        public string SigmaB;
        public string SigmaS;
        public string Sorting;
        public string UsePage;
        public string BilateralDefaultCheck;
    }

    [Serializable]
    public class FilterTableEntry
    {
        public string Key;
        public string File;
        public string Used;
        public string Korean;
        public string Chinese;
        public string English;
        public string Sequence;
        public string Thumbnail;
    }

}

namespace ServiceData
{
    [Serializable]
    public class ServiceData
    {
        public ConfigEntry Config;
        public ContentsEntryDic Contents;
        public ContentsDetailEntryDic ContentsDetail;

        [Serializable]
        public class ContentsEntryDic : SerializableDictionaryBase<string, ContentsEntry> { }
        [Serializable]
        public class ContentsDetailEntryDic : SerializableDictionaryBase<string, ContentsDetailEntry> { }
    }

    [Serializable]
    public class ConfigEntry
    {
        public string Sorting;
    }

    [Serializable]
    public class ContentsEntry
    {
        public string Key;
        public CONTENT_TYPE ContentType;
        public string Use;
        public string Sorting;
        public string Sequence;
        public string ImageThumbnail;
        public Texture2D ImageThumbnail_data;
        public string VideoThumbnail;
        public string VideoThumbnail_path;
        public string GuideImage;
        public Texture2D GuideImage_data;
        public string BGGuideImage;
        public Texture2D BGGuideImage_data;
        public string People_Icon;
        public string Korean_Title;
        public string Chinese_Title;
        public string English_Title;
        public string Korean_People;
        public string Chinese_People;
        public string English_People;
        public string Korean_SubText;
        public string Chinese_SubText;
        public string English_SubText;
        public string ShootGuideImage;
        public Texture2D ShootGuideImage_data;
    }

    [Serializable]
    public class ContentsDetailEntry
    {
        public string Key;
        public string Use;
        public string Model;
        public string Gender;
        public string Category;
        public string Property;
        public string Sequence;
        public string Thumbnail;
        public Texture2D Thumbnail_data;
        public string References;
        public string KoreanTitle;
        public string ChineseTitle;
        public string EnglishTitle;
        public string KoreanSubtext;
        public string ChineseSubtext;
        public string EnglishSubtext;
    }
}

namespace BasicData
{
    [Serializable]
    public class BasicSetting
    {
        public ConfigEntry Config;
        public DeviceEntryDic Device;

        [Serializable]
        public class DeviceEntryDic : SerializableDictionaryBase<string, DeviceEntry> { }
    }

    [Serializable]
    public class ConfigEntry
    {
        public string BTBG;
        public string CABG;
        public string CAMenu;
        public string PRMenu;
        public string WFMenu;
        public string BGImage;
        public string EndImage;
        public string Printing;
        public string ColorCode;
        public string OtherMenu;
        public string PayConfirm;
        public string StartImage;
        public string DefaultUsed;
        public string FrameSelect;
        public string OptionalUse;
        public string VideoVolume;
        public string Age14TermUse;
        public string ContentsMenu;
        public string PhotoStandby;
        public string Age14TermUsed;
        public string PlayShotMovie;
        public string PaymentTermUse;
        public string PaymentTermUsed;
        public string PrintErrorImage;
        public string StartMediaVideo;
        public string MarketingTermUse;
        public string MultiLanguageUse;
        public List<string> PaymentTermImage;
        public List<string> ServiceTermImage;
        public string ServieErrorImage;
        public string ShootingPRSelect;
        public string MarketingTermUsed;
        public List<string> MarketingTermImage;
        public List<string> PersonalPolicyImage;
    }

    [Serializable]
    public class DeviceEntry
    {
        public string Key;
        public string TID;
        public string Deploy;
        public string MailList;
        public string MachineID;
        public string StoreName;
        public string PrinterModel;
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
        public string Key;
        public string value1;
        public string value2;
        public string value3;
        public string value4;
        public string value5;
        public string value6;
        public string value7;
        public string value8;
        public string value9;
        public string value10;
    }

    [Serializable]
    public class CategoryEntry
    {
        public string Key;
        public string Korean;
        public string Chinese;
        public string English;
    }

    [Serializable]
    public class ChromakeyToneTableEntry
    {
        public string Key;
        public string Color;
        public string Korean;
        public string Chinese;
        public string English;
        public string Sequence;
        public string Thumbnail;
    }

    [Serializable]
    public class ChromakeyFrameTableEntry
    {
        public string Key;
        public string Image1;
        public string Image2;
        public string Image3;
        public string Image4;
        public string Image5;
        public string Image6;
        public string Image7;
        public string Image8;
        public string Korean;
        public string Chinese;
        public string English;
        public string Category;
        public string Sequence;
        public string Thumbnail;
    }
}

namespace ShootingScreenData
{

    [Serializable]
    public class ShootScreenEntry
    {
        public string Key;
        public ShootScreenEntryDic url;
        public ShootScreenEntryDic ratio;
        public ShootScreenEntryDic korean;
        public ShootScreenEntryDic chinese;
        public ShootScreenEntryDic english;
        public string ConversionTime;
        public string ConversionVideo;

    }

    [Serializable]
    public class ShootScreenEntryDic : SerializableDictionaryBase<string, string> { }

    [Serializable]
    public class ShootScreenDic : SerializableDictionaryBase<string, ShootScreenEntry> { }
}