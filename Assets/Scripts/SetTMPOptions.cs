using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ButtonColorChangeTMP : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Color originalTextColor; 
    private int originalFontSize;
    private Color pressedTextColor;
    private int pressedFontSize;
    //private ColorBlock backgroundColor;
    private TextMeshProUGUI buttonText;
    private RawImage deleteButton;
    private Button button;

    void Start()
    {
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText == null)
        {
            deleteButton = GetComponentInChildren<RawImage>();
        }
        button = GetComponentInChildren<Button>();
    }
    public void SetTextOptions(Color originalColor, int originalSize, Color pressedColor, int pressedSize)
    {
        originalTextColor = originalColor;
        originalFontSize = originalSize;
        pressedTextColor = pressedColor;
        pressedFontSize = pressedSize;

        if (buttonText != null)
        {
            buttonText.fontSize = originalSize;
            buttonText.color = originalColor;
        }
        else
        {
            deleteButton.color = originalColor;
        }
    }

    public void SetBackgroundOptions(Color originalColor, Color pressedColor)
    {
        ColorBlock colorBlock = button.colors;
        colorBlock.normalColor = originalColor;
        colorBlock.pressedColor = pressedColor;
        button.colors = colorBlock;
    }

    // 버튼이 눌렸을 때 호출되는 함수
    public void OnPointerDown(PointerEventData eventData)
    {
        if (buttonText != null)
        {
            buttonText.color = pressedTextColor;  // 클릭 중 색상 변경
            buttonText.fontSize = pressedFontSize;
        }
        else
        {
            deleteButton.color = pressedTextColor;
        }
    }

    // 버튼이 눌림에서 해제되었을 때 호출되는 함수
    public void OnPointerUp(PointerEventData eventData)
    {
        if (buttonText != null)
        {
            buttonText.color = originalTextColor;  // 원래 색상으로 복원
            buttonText.fontSize = originalFontSize;
        }
        else
        {
            deleteButton.color = originalTextColor;
        }
    }
}
