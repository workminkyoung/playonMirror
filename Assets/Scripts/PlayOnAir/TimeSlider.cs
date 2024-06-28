using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeSlider : MonoBehaviour
{
    List<SliderComponent> sliderComponents = new List<SliderComponent>();
    RawImage rawImage;
    Slider _slider;
    HandlePointer handlePointer;

    public Slider slider { get { return _slider; } }

    public void Setting()
    {
        rawImage = GetComponentInChildren<RawImage>();
        _slider = GetComponent<Slider>();
        sliderComponents.AddRange(GetComponentsInChildren<SliderComponent>());
        handlePointer = GetComponentInChildren<HandlePointer>();
        handlePointer.Setting();
        SetActive(false);
    }

    public void SetActive(bool state)
    {
        for (int i = 0; i < sliderComponents.Count; i++)
        {
            sliderComponents[i].SetActive(state);
        }
        rawImage.gameObject.SetActive(state);
        _slider.interactable = state;
        handlePointer.SetState(state);
    }
}
