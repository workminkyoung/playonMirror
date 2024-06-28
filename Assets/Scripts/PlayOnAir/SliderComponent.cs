using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderComponent : MonoBehaviour
{
    Text targetText;
    Image targetImg;
    [SerializeField]
    Sprite off, on;
    [SerializeField]
    Color colorOff, colorOn;

    private void Awake()
    {
        targetImg = GetComponent<Image>();
        targetText = GetComponent<Text>();
    }

    public void SetActive(bool state)
    {
        if (state)
        {
            if (targetImg != null)
                targetImg.sprite = on;
            if (targetText != null)
                targetText.color = colorOn;
        }
        else
        {
            if (targetImg != null)
                targetImg.sprite = off;
            if (targetText != null)
                targetText.color = colorOff;
        }
    }
}

