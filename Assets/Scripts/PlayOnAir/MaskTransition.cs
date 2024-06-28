using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MaskTransition : MonoBehaviour
{
    public Action OnTransitionEnd;
    private List<RawImage> listRaw = new List<RawImage>();
    private List<Image> listImg = new List<Image>();
    int recordIdx = 0;

    public void Setting()
    {
        listRaw.AddRange(GetComponentsInChildren<RawImage>());
        listImg.AddRange(GetComponentsInChildren<Image>());
    }

    public void Init()
    {
        recordIdx = 0;
    }

    public void ResetUI()
    {
        listImg[(int)eImg.Line].gameObject.SetActive(false);
        listImg[(int)eImg.Mask].gameObject.SetActive(true);
        listImg[(int)eImg.Mask].rectTransform.sizeDelta = listRaw[0].rectTransform.sizeDelta;
    }

    public void SetCartoon(Texture2D cartoon)
    {
        listRaw[(int)eRaw.Cartoon].texture = cartoon;
    }
    public void SetReal(Texture2D real)
    {
        listRaw[(int)eRaw.Real].texture = real;
    }

    public void StartTransition()
    {
        listImg[(int)eImg.Line].gameObject.SetActive(true);
        StartCoroutine(Real2Cartoon());
    }

    IEnumerator Real2Cartoon()
    {
        listImg[(int)eImg.Mask].gameObject.SetActive(true);
        float t = 0;
        float duration = 3;
        yield return new WaitForEndOfFrame();
        Vector2 originSize = listImg[(int)eImg.Mask].rectTransform.sizeDelta;
        recordIdx++;

        while (t < duration)
        {
            t += Time.deltaTime;
            float curWidth = UtilityExtensions.Remap(t, 0, duration, 0, originSize.x);
            listImg[(int)eImg.Mask].rectTransform.sizeDelta = originSize - Vector2.right * curWidth;
            yield return null;
        }

        listImg[(int)eImg.Mask].rectTransform.sizeDelta = new Vector2(0, originSize.y);
        listImg[(int)eImg.Line].gameObject.SetActive(false);
        yield return new WaitForSeconds(3);
        OnTransitionEnd();
    }

    enum eRaw
    {
        Cartoon = 0,
        Real
    }

    enum eImg
    {
        Mask = 0,
        Line
    }
}
