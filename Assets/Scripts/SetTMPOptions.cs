using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ButtonColorChangeTMP : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    public Color onClickColor = new Color32(206, 51, 137, 255);  // 클릭 중일 때의 색상
    [SerializeField]
    public int fontSize = 52;
    //[SerializeField]
    //public FontStyles fontStyle = FontStyles.Bold;

    private Color originalColor = new Color32(0, 0, 0, 255);           // 원래 색상
    private int originalFontSize;
    private int pressedFontSize;
    private TextMeshProUGUI buttonText;  
    void Start()
    {
        // 하위에 있는 TextMeshProUGUI 컴포넌트를 찾음
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        
        if (buttonText != null)
        {
            originalColor = buttonText.color;  // 원래 색상 저장
        }
        buttonText.fontSize = fontSize;
        //buttonText.fontStyle = fontStyle;
    }
    public void SetOriginOptions(Color color, int size)
    {
        originalColor = color;
        originalFontSize = size;
    }

    public void SetPressedOptions(Color color, int size)
    {
        onClickColor = color;
        pressedFontSize = size;
    }

    // 버튼이 눌렸을 때 호출되는 함수
    public void OnPointerDown(PointerEventData eventData)
    {
        if (buttonText != null)
        {
            buttonText.color = onClickColor;  // 클릭 중 색상 변경
            buttonText.fontSize = pressedFontSize;
        }
    }

    // 버튼이 눌림에서 해제되었을 때 호출되는 함수
    public void OnPointerUp(PointerEventData eventData)
    {
        if (buttonText != null)
        {
            buttonText.color = originalColor;  // 원래 색상으로 복원
            buttonText.fontSize = originalFontSize;
        }
    }
}
