using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Config
{
    public int camType;
    public int popupTime;
    public int priceStep;

    //camera setting
    public int wbIdx;
    public int isoIdx;
    public int avIdx;
    public int tvIdx;

    //0 : KICC, 1 : KSNET
    public string _comment;
    public int paymentMethod;
    public string KsnetCatID;

    public ChromaKeyConfig chromaKey;
}
public static class ConfigData
{
    public static Config config;
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