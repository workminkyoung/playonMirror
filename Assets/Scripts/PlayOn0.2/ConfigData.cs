using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Config
{
    public bool localUrlCheck;
    public string serverUrl;

    public int camType;

    public int photoTime;//for saving
    public int photoTime_cartoon;
    public int photoTime_profile;
    public int photoTime_beauty;

    //cartoon resize data
    //public int reSizeWidth;
    //public int reSizeHeight;

    //wait times
    public int shootWarningTime;
    public int waitPrintTime;
    public int contentSelectTime;
    public int genderSelectTime;
    public int profileSelectTime;
    public int cartoonStyleSelectTime;
    public int profilePicSelectTime;

    public int frameSelectTime;
    public int paymentPageTime;
    public int paymentFailTime;
    public int cautionPageTime;
    public int decoPageTime;
    public int loadingTime;
    public int loadingTimeBeauty;

    //camera setting
    public int wbIdx;
    public int isoIdx;
    public int avIdx;
    public int tvIdx;

    //pay setting
    public bool policyDefaultCheck;
    public bool childDefaultCheck;
    public int firstPrintAmount;

    //sticker setting
    public int stickerMaxCount;

    public ChromaKeyConfig chromaKey;
    //0 : KICC, 1 : KSNET
    public MailConfig mailConfig;
    public PriceConfig priceConfigCartoon;
    public PriceConfig priceConfigProfile;
    public PriceConfig priceConfigBeauty;
    public PriceConfig priceConfigWhatIf;

    public string _comment;
    public int paymentMethod;
    public string KsnetCatID;
}
public static class ConfigData
{
    public static Config config;
}

[Serializable]
public class MailConfig
{
    public string[] mailingList;
    public string storeName;
}

[Serializable]
public class PriceConfig
{
    public int priceNum;
    public int[] originalPrices;
    public int[] discountPrices;
}

[Serializable]
public class ChromaKeyConfig
{
    public bool isOn;
    public float d;
    public float t;
    public int blur;
    public float alphaPow;
    public float alphaEdge;
    public string color;    
}