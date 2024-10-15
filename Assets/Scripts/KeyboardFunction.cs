using TMPro;
using UnityEngine;

public class KeyboardFunction : MonoBehaviour
{
    [SerializeField]
    public TMP_InputField inputField;

    public void OnKeyClick(string key)
    {
        inputField.text += key;
    }

    public void OnBackspaceClick()
    {
        if (inputField.text.Length > 0)
        {
            inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
        }
    }

    public void OnEnterClick()
    {
        Debug.Log("Enter key pressed");
    }

    public void OnClearClick()
    {
        inputField.text = "";
    }
}