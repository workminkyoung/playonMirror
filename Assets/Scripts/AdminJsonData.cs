using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RotaryHeart.Lib.SerializableDictionary;

[System.Serializable]
public class ConfigDefaultData
{
    public int id;
    public Result machine_config;
}

//[System.Serializable]
//public class ConfigDefaultSet
//{
//    public Result result;
//}

[Serializable]
public class Result
{
    public object FrameData;
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
        public int Group;
        public string Image;
        public string Korean;
        public int Margin;
        public string Chinese;
        public string English;
        public string FontSet;
        public string Category;
        public float FontSize;
        public string Sequence;
        public string TextArea;
        public string FontColor;
        public string ImageType;
        public string Thumbnail;
        public float StartScale;
        public float ThumbnailScale;
        public Texture2D Image_data;
        public Sprite Image_sprite;
    }
}

namespace FilterData
{
    [Serializable]
    public class FilterData
    {
        public ConfigEntry Config;
        public FilterTableEntryDic FilterTable;
        public OrderedFilterTableDic OrderedFilterTable;

        [Serializable]
        public class FilterTableEntryDic : SerializableDictionaryBase<string, FilterTableEntry> { }
        [Serializable]
        public class OrderedFilterTableDic : SerializableDictionaryBase<int, FilterTableEntry> { }
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
        public Texture2D LutFile_data;
        public string Used;
        public string Korean;
        public string Chinese;
        public string English;
        public string Sequence;
        public string Thumbnail;
        public Sprite Thumbnail_data;
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
        public Sprite ImageThumbnail_data;
        public string VideoThumbnail;
        public string VideoThumbnail_path;
        public string GuideImage;
        public Sprite GuideImage_data;
        public string PopupGuideImage;
        public Sprite PopupGuideImage_data;
        public string BGGuideImage;
        public Sprite BGGuideImage_data;
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
        public Sprite ShootGuideImage_data;
    }

    [Serializable]
    public class ContentsDetailEntry
    {
        public string Key;
        public string Use;
        public string Model;
        public string Gender;
        public GENDER_TYPE Gender_type;
        public string Category;
        public string Property;
        public string Sequence;
        public string Thumbnail;
        public Sprite Thumbnail_data;
        public string References;
        public string Korean_Title;
        public string Chinese_Title;
        public string English_Title;
        public string Korean_SubText;
        public string Chinese_SubText;
        public string English_SubText;
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
        public Sprite BGImage_data;
        public string EndImage;
        public Sprite EndImage_data;
        public string Printing;
        public string ColorCode;
        public string OtherMenu;
        public string PayConfirm;
        public string StartImage;
        public Sprite StartImage_data;
        public string DefaultUsed;
        public string FrameSelect;
        public string OptionalUse;
        public string VideoVolume;
        public string Age14TermUse;
        public string ContentsMenu;
        public string PhotoStandby;
        public string Age14TermUsed;
        public string PlayShotMovie;
        public string PromotionImage;
        public Texture2D PromotionImage_data;
        public string PromotionVideo;
        public string PromotionVideo_path;
        public string PaymentTermUse;
        public string PaymentTermUsed;
        public string PrintErrorImage;
        public Sprite PrintErrorImage_data;
        public string StartMediaVideo;
        public string StartMediaVideo_path;
        public string MarketingTermUse;
        public string MultiLanguageUse;
        public string PersonalPolicyImage;
        public Sprite PersonalPolicyImage_data;
        public string ServiceTermImage;
        public Sprite ServiceTermImage_data;
        public string PaymentTermImage;
        public Sprite PaymentTermImage_data;
        public string MarketingTermImage;
        public Sprite MarketingTermImage_data;
        public string ServieErrorImage;
        public Sprite ServieErrorImage_data;
        public string ShootingPRSelect;
        public string MarketingTermUsed;
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
        public int Sequence;
        public string Category;
        public string Korean;
        public string English;
        public string Chinese;
        public string Thumbnail;
        public string Image1;
        public string Image2;
        public string Image3;
        public string Image4;
        public string Image5;
        public string Image6;
        public string Image7;
        public string Image8;

        public Sprite Thumbnail_data;
        public List<Texture2D> Image_data = new List<Texture2D>();
    }

    [Serializable]
    public class ChromakeyFrameTableEntry
    {
        public string Key;
        public int Sequence;
        public string Category;
        public string Korean;
        public string English;
        public string Chinese;
        public string Thumbnail;
        public string Image1;
        public string Image2;
        public string Image3;
        public string Image4;
        public string Image5;
        public string Image6;
        public string Image7;
        public string Image8;
        
        public Sprite Thumbnail_data;
        public List< Texture2D> Image_data = new List<Texture2D>();
    }
}

namespace ShootingScreenData
{
    [Serializable]
    public class ShootScreenEntry
    {
        public string Key;
        public ShootScreenEntryDic url;
        public URLDataDic url_datas;
        public List<string> url_orderdKey;
        public List<string> ratio;
        public List<string> korean;
        public List<string> chinese;
        public List<string> english;
        public string ConversionTime;
        public string ConversionImage;
        public Texture2D ConversionImage_data;
        public string ConversionVideo;
        public string ConversionVideo_path;

    }

    [Serializable]
    public class URLDataDic : SerializableDictionaryBase<string, Sprite> { }
    [Serializable]
    public class ShootScreenEntryDic : SerializableDictionaryBase<string, string> { }

    [Serializable]
    public class ShootScreenDic : SerializableDictionaryBase<string, ShootScreenEntry> { }
}

namespace FrameData
{
    [Serializable]
    public class FrameEntry
    {
        public string Key;
        public string Code;
        public string data;
        public Sprite ThumbnailUnselect;
        public Sprite ThumbnailSelect;
        public FrameDefinitionEntryDic FrameDefinitions;
    }

    [Serializable]
    public class FrameDefinitionEntry
    {
        public string Service;
        public string ColorCode;
        public string Direction;
        public string Thumbnaillink;
        public string ThumbnailSliced;
        public string Righname;
        public string PicConvert1;
        public string PicConvert2;
        public string PicConvert3;
        public string PicConvert4;
        public string PicConvert5;
        public string PicConvert6;
        public string PicConvert7;
        public string PicConvert8;
        public string DateColor;
        public string DateRect;
        public string DatefontSize;
        public string DateFont;
        public string Default;
        public int DefaultFont;
        public string QRRect;
        public string ShootRatio;
        public string OriginCropRatio;
        public string ShootingUrl;
        public string sellingPrice1;
        public string sellingPrice2;
        public string sellingPrice3;
        public int sellingPrice4;
        public int sellingPrice5;
        public int sellingPrice6;
        public int sellingPrice7;
        public int sellingPrice8;
        public List<FrameRectTransform> picRects;
        public FrameRectTransform dateRect;
        public FrameRectTransform qrRect;
    }

    [Serializable]
    public class FrameRectTransform
    {
        public Vector2 anchoredPosition;
        public Vector2 sizeDelta;
        public Vector2 anchorMin;
        public Vector2 anchorMax;
        public Vector2 pivot;
        public Vector3 rotation;
    }

    [Serializable]
    public class FrameEntryDic : SerializableDictionaryBase<string, FrameEntry> { }
    [Serializable]
    public class FrameDefinitionEntryDic : SerializableDictionaryBase<Tuple<string, string>, FrameDefinitionEntry> { }
}