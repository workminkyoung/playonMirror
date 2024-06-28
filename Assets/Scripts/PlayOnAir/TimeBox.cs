using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeBox : MonoBehaviour
{
    string redColorCode = "#FF5959";
    string grayColorCode = "#282828";
    Color redColor;
    Color grayColor;

    List<Image> listImg = new List<Image>();
    List<TextMeshProUGUI> listText = new List<TextMeshProUGUI>();

    public void Setting()
    {
        listImg.AddRange(GetComponentsInChildren<Image>());
        listText.AddRange(GetComponentsInChildren<TextMeshProUGUI>());

        ColorUtility.TryParseHtmlString(redColorCode, out redColor);
        ColorUtility.TryParseHtmlString(grayColorCode, out grayColor);
    }

    public void Init()
    {
        SetNormal();
        listText[(int)eText.Time].text = "0";// ConfigData.config.photoTime.ToString();
        listText[(int)eText.Shoot].text = "0/" + 4;
    }

    public void SetNormal()
    {
        listImg[(int)eImg.Box].color = grayColor;
        listImg[(int)eImg.Icon].color = Color.white;
        listText[(int)eText.Time].color = Color.white;
    }

    public void SetWarning()
    {
        listImg[(int)eImg.Box].color = redColor;
        listImg[(int)eImg.Icon].color = redColor;
        listText[(int)eText.Time].color = redColor;
    }

    public void SetTime(int time)
    {
        listText[(int)eText.Time].text = time.ToString();
    }
    public void SetTime(string time)
    {
        listText[(int)eText.Time].text = time;
    }

    public void SetShootCount(int count)
    {
        listText[(int)eText.Shoot].text = count + "/" + 4;
    }

    enum eImg
    {
        Box = 0,
        Icon
    }

    enum eText
    {
        Time = 0,
        Shoot
    }
}
