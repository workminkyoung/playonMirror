using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RotaryHeart.Lib.SerializableDictionary;
using ShootingScreenData;

[System.Serializable]
public class ConfigDefaultData
{
    public int id;
    public Result machine_config;
}

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
        public FilterTableEntry DefaultFilter;

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
        public string UseBilateral;
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
        public bool Use;
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
        public string TipGuideImage;
        public Sprite TipGuideImage_data;
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
        // 페이지별 대기 시간
        public string Printing;
        public int Printing_data;
        public string ContentsMenu;
        public int ContentsMenu_data;
        public string CAMenu;
        public int CAMenu_data;
        public string PRMenu;
        public int PRMenu_data;
        public string WFMenu;
        public int WFMenu_data;
        public string BTBG;//basic 배경테마 선택페이지
        public int BTBG_data;
        public string CABG;//catoon 배경테마 선택페이지
        public int CABG_data;
        public string FrameSelect;
        public int FrameSelect_data;
        public string PayConfirm;//결제정보 확인 화면
        public int PayConfirm_data;
        public string PhotoStandby;//촬영 전 유의사항 페이지
        public int PhotoStandby_data;
        public string ShootingPRSelect;//프로필 결과물 선택 페이지
        public int ShootingPRSelect_data;
        public string OtherMenu;//꾸미기 화면
        public int OtherMenu_data;

        // 촬영 시간
        public int CAShootTime;
        public int PRShootTime;
        public int WFShootTime;
        public int BTShootTime;
        public int CCShootTime;
        public int ShootWarningTime;
        public int PaymentFailTime;

        // QR 인화 사용여부 데이터
        public bool DefaultUsed; // 기본상태
        public bool OptionalUse; // QR 인화 체크박스 사용여부

        // 약관 관리
        public bool Age14TermUse;
        public bool Age14TermUsed;
        public bool PaymentTermUse;
        public bool PaymentTermUsed;

        public string BGImage;
        public Sprite BGImage_data;
        public string EndImage;
        public Sprite EndImage_data;
        public string ColorCode;
        public string StartImage;
        public Sprite StartImage_data;
        public string VideoVolume;
        public string PlayShotMovie;
        public string PromotionImage;
        public Texture2D PromotionImage_data;
        public string PromotionVideo;
        public string PromotionVideo_path;
        public string PrintErrorImage;
        public Sprite PrintErrorImage_data;
        public string StartMediaVideo;
        public string StartMediaVideo_path;
        public string StartMediaImage;
        public Sprite StartMediaImage_data;
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

        public string[] mailList_data;
    }

}

namespace ChromakeyFrameData
{
    [Serializable]
    public class ChromakeyFrame
    {
        public ConfigEntryDic Config;
        public List<CategoryEntry> Category;
        public ChromakeyTableEntryDic ChromakeyToneTable;
        public ChromakeyTableEntryDic ChromakeyFrameTable;
        public OrderedChromakeyTableEntryDic OrderedChromakeyToneTable;
        public OrderedChromakeyTableEntryDic OrderedChromakeyFrameTable;

        [Serializable]
        public class ConfigEntryDic : SerializableDictionaryBase<string, ConfigEntry> { }
        [Serializable]
        public class ChromakeyTableEntryDic : SerializableDictionaryBase<string, ChromakeyToneTableEntry> { }
        [Serializable]
        public class OrderedChromakeyTableEntryDic : SerializableDictionaryBase<int, ChromakeyToneTableEntry> { }
        //[Serializable]
        //public class ChromakeyFrameTableDic : SerializableDictionaryBase<string, ChromakeyToneTableEntry> { }
        //[Serializable]
        //public class OrderedChromakeyFrameTableDic : SerializableDictionaryBase<int, ChromakeyToneTableEntry> { }
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
        //public List<Texture2D> Image_data = new List<Texture2D>();
        public ImageOrderedDic orderedImage;
    }

    [Serializable]
    public class ImageOrderedDic : SerializableDictionaryBase<int, Texture2D> { }
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
    public class FrameData
    {
        public Theme Theme;
        public DefinitionCode Definition;
        //public DefinitionEntryDic Definition;
        public FrameDefinitionEntryDic DefinitionTuple;
        public CommonCode Common;
        public CommonTupleEntryDic CommonTuple;
        public FrameCode ServiceFrame;
    }

    [Serializable]
    public class Theme
    {
        public string Sorting;
        public ColorCodeEntryDic ColorCode;
        public OrderedColorCodeEntryDic OrderedColorCode;
    }
    [Serializable]
    public class ColorCodeEntry
    {
        public string key;
        public bool Use;
        public string korean;
        public string chinese;
        public string english;
        public string Sequence;
        public string Thumbnail;
        public Sprite Thumbnail_data;
    }

    [Serializable]
    public class CommonCode
    {
        public CommonEntryDic Code;
    }

    [Serializable]
    public class CommonEntry
    {
        public string Code;
        public string Service;
        public FRAME_TYPE FrameType;

        public string originPrice;
        public string discountPrice;
        public int[] originPrice_datas;
        public int[] discountPrice_datas;

        public string ThumbnailUnselect;
        public Sprite ThumbnailUnselect_data;
        public string ThumbnailSelect;
        public Sprite ThumbnailSelect_data;

        public string Dim1;
        public string Dim2;
        public string Dim3;
        public string Dim4;
        public string Dim5;
        public string Dim6;
        public string Dim7;
        public string Dim8;
        public List<Sprite> Dim_datas;
    }

    [Serializable]
    public class DefinitionCode
    {
        public DefinitionEntryDic Code;
    }

    [Serializable]
    public class DefinitionEntry
    {
        public string key;
        public FRAME_TYPE FrameType;

        // Background Image
        public string BgImage;
        public Sprite BgImage_data;

        // Layer Image
        public string LayerImage;
        public Sprite LayerImage_data;

        // Service type
        public string Service;

        // Date font and color
        public string Datefont;
        public string DateColor;
        public int DatefontSize;

        // Direction
        public string Direction;

        // Date format
        public string DateFormat;

        // Color code
        public string ColorCode;

        // QR and Date Rects
        public string QRRect_1;
        public string QRRect_2;
        public string DateRect_1;
        public string DateRect_2;

        // Picture Canvases
        public string PicCanvas1;
        public string PicCanvas2;
        public string PicCanvas3;
        public string PicCanvas4;
        public string PicCanvas5;
        public string PicCanvas6;
        public string PicCanvas7;
        public string PicCanvas8;

        public List<FrameRectTransform> picRects;
        public List<FrameRectTransform> dateRects;
        public List<FrameRectTransform> qrRects;
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
    public class FrameCode
    {
        public string Sorting;
        public FrameCodeEntryDic Code;
    }

    [Serializable]
    public class CodeEntry
    {
        public string SelectFrame1;
        public string SelectFrame2;
        public string SelectFrame3;
        public string SelectFrame4;
        public string SelectFrame5;
        public string SelectFrame6;
        public string SelectFrame7;
        public string SelectFrame8;
        public string SelectFrame9;
        public string SelectFrame10;
        public int DefaultSellAmount;

        public List<string> SelectFrames;
    }

    // Theme
    [Serializable]
    public class FrameCodeEntryDic : SerializableDictionaryBase<string, CodeEntry> { }
    [Serializable]
    public class ColorCodeEntryDic : SerializableDictionaryBase<string, ColorCodeEntry> { }
    [Serializable]
    public class OrderedColorCodeEntryDic : SerializableDictionaryBase<int, ColorCodeEntry> { }

    // Common
    [Serializable]
    public class CommonEntryDic : SerializableDictionaryBase<string, List<CommonEntry>> { }
    [Serializable]
    public class CommonTupleEntryDic : SerializableDictionaryBase<Tuple<string, string>, CommonEntry> { }
    //[Serializable]
    //public class FrameCommonEntryDic : SerializableDictionaryBase<string, CommonEntryDic> { }

    // Frame
    [Serializable]
    public class DefinitionEntryDic : SerializableDictionaryBase<string, List<DefinitionEntry>> { }
    [Serializable]
    public class FrameDefinitionServiceColorDic : SerializableDictionaryBase<Tuple<string, string>, DefinitionEntry> { }
    [Serializable]
    public class FrameDefinitionEntryDic : SerializableDictionaryBase<string, FrameDefinitionServiceColorDic> { }
}