using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UC_Frame : MonoBehaviour
{
    [SerializeField]
    List<RawImage> _rawimage = new List<RawImage>();
    [SerializeField]
    List<Image> _images = new List<Image>();
    [SerializeField]
    private Image _logoSmall;
    TextMeshProUGUI _date;

    public List<RawImage> rawImages { get { return _rawimage; } }
    public List<Image> Images { get { return _images; } }
    public TextMeshProUGUI Date { get {  return _date; } }

    Dictionary<int, RawImage> _dictRawNum = new Dictionary<int, RawImage>();

    int _index;
    public int Index {  get { return _index; } }

    Texture2D _origin, _bilater;

    public virtual void Setting()
    {
        //_rawimage.AddRange(GetComponentsInChildren<RawImage>());
        //_images.AddRange(GetComponentsInChildren<Image>());
        _date = GetComponentInChildren<TextMeshProUGUI>();
    }

    public virtual void SetActivate(bool state)
    {
        gameObject.SetActive(state);
        if(_images.Count > 1)
            _images[(int)IMG.COVER]?.gameObject.SetActive(true);
        _date.text = DateTime.Now.ToString("yyyy.MM.dd");
    }

    public virtual void SetTexture(int index, Texture2D texture)
    {
        
        if(index < _rawimage.Count)
        {
            _index = index;
            _rawimage[index].texture = texture;
            _origin = texture;
        }
    }
    public virtual void SetTexture(int index, Texture texture)
    {

        if (index < _rawimage.Count)
        {
            _index = index;
            _rawimage[index].texture = texture;
            //_origin = texture;
        }
    }

    public virtual void RemoveTexture(int index)
    {
        if (index <= _rawimage.Count - 1)
        {
            _rawimage[index].texture = null;
        }
    }

    public void RemoveCover()
    {
        if (_images.Count > 1)
            _images[(int)IMG.COVER].gameObject.SetActive(false);
    }

    public void ChangeColor(Color color)
    {
        _images[(int)IMG.BG].color = color;

        if ((color.r + color.g + color.b) / 3 < 0.5f)
        {
            // black
            _date.color = Color.white;
            _logoSmall.color = Color.white;
        }
        else
        {
            // white
            _date.color = Color.black;
            _logoSmall.color = Color.black;
        }
    }

    public virtual void SetLut(Material matPrefab, Texture2D lut)
    {
        for (int i = 0; i < _rawimage.Count; i++)
        {
            Material mat = Instantiate(matPrefab);
            mat.SetTexture("_MainTex", _rawimage[i].texture);
            mat.SetTexture("_LutTex", lut);

            _rawimage[i].material = mat;
        }
    }

    public void SetBilater()
    {
        //BilateralFilterModule.BilateralFilter(_origin, out _bilater);
    }

    enum IMG
    {
        BG,
        COVER
    }
}
