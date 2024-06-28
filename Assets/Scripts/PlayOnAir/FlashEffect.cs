using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class FlashEffect : MonoBehaviour
{
    Image image;
    const float flashOn = 0.15f;
    const float flashOff = 0.5f;

    public float flashTime { get { return flashOn + flashOff; } }

    public void Setting()
    {
        image = GetComponent<Image>();
    }

    public void Init()
    {
        image.color = new Color(1, 1, 1, 0);
    }

    public void StartFlashing(Action onEndEvent = null)
    {
        image.DOFade(1, flashOn).OnComplete(() =>
        {
            image.DOFade(0, flashOff).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                onEndEvent?.Invoke();
            });
        });
    }
}
