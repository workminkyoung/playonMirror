using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Loading : MonoBehaviour
{
    Action OnEndEvent;
    const string loadText = "AI가 최고의 이미지를 만들고 있습니다. ";
    const float loadTime = 10;
    const float loadTimeSpeedy = 1;
    public string pageName;

    private List<Image> listImg = new List<Image>();
    private TextMeshProUGUI textLoad;
    private Coroutine loading;
    private bool isLoaded = false;

    public void Setting()
    {
        listImg.AddRange(GetComponentsInChildren<Image>());
        textLoad = GetComponentInChildren<TextMeshProUGUI>();
        gameObject.SetActive(false);
    }

    public void StartLoading()
    {
        isLoaded = false;
        gameObject.SetActive(true);
        loading = StartCoroutine(Load());
    }

    public void StopLoading()
    {
        isLoaded = true;
        gameObject.SetActive(false);
        if(loading != null)
            StopCoroutine(loading);
    }

    public void SetLoadState(bool loadState, Action endAction = null)
    {
        OnEndEvent = endAction;
        isLoaded = loadState;
    }

    IEnumerator Load()
    {
        float t = 0;
        float remap = 0;
        while (!isLoaded)
        {
            if (t < loadTime)
            {
                t += Time.deltaTime;
                remap = UtilityExtensions.Remap(t, 0, loadTime, 0, 99);
                remap = Mathf.FloorToInt(remap);
                textLoad.text = string.Format("{0}({1}%)", loadText, remap.ToString());
            }

            if(Input.GetMouseButtonDown(0))
                DataLog.Instance.AddComponent(pageName, DataLog.inst.boundary);

            listImg[(int)eImg.Hole].rectTransform.Rotate(Vector3.forward, 1);
            yield return null;
        }

        float preTime = remap;
        t = 0;
        while(remap < 99)
        {
            t += Time.deltaTime;
            remap = UtilityExtensions.Remap(t, 0, loadTimeSpeedy, preTime, 99);
            remap = Mathf.FloorToInt(remap);
            textLoad.text = string.Format("{0}({1}%)", loadText, remap.ToString());
            yield return null;
        }

        textLoad.text = string.Format("{0}({1}%)", loadText, 100);
        OnEndEvent?.Invoke();
        StopLoading();
    }

    enum eImg
    {
        BG = 0,
        Hole,
        Back
    }
}
