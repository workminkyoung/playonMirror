using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorExtension
{
    public static Color HexToColor(string hexcode)
    {
        Color color;
        if (!hexcode.Contains("#"))
            hexcode = '#' + hexcode;
        ColorUtility.TryParseHtmlString(hexcode, out color);
        return color;
    }
}
