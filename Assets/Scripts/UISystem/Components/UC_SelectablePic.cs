using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UC_SelectablePic : UC_SelectableContent
{
    [SerializeField]
    private TextMeshProUGUI _picNum;
    public Texture2D thumbnailTex 
    { 
        get 
        { 
            if(_thumbnailImg.sprite)
            {
                return _thumbnailImg.sprite.texture;
            }
            else
            {
                return null;
            } 
            
        } 
    }

    protected override void OnDisable()
    {

    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        pointerDownAction?.Invoke();
    }

    public void SetNum(int num)
    {
        _picNum.text = num.ToString();
    }

    public override void Select(bool isSelected)
    {
        base.Select(isSelected);

        _picNum?.gameObject.SetActive(isSelected);
    }

    public void SetThumbnail(Texture2D tex)
    {
        if(tex != null)
        {
            _thumbnailImg.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }
        else
        {
            _thumbnailImg.sprite = null;
        }
    }
}
