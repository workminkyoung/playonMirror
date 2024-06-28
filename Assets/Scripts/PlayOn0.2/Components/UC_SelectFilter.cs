using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vivestudios.UI;

public class UC_SelectFilter : UC_BaseComponent
{
    [SerializeField]
    private Toggle _skinToggle;
    [SerializeField]
    private List<GameObject> _content = new List<GameObject>();
    [SerializeField]
    private List<GameObject> _skinToggles = new List<GameObject>();
    [SerializeField]
    private List<UC_Filter> _filterToggles = new List<UC_Filter>();

    public Action<bool> SendBilateral;

    private CONTENT_TYPE _type;

    public CONTENT_TYPE type { set { _type = value; } }

    public Func<Dictionary<Texture2D, PHOTO_TYPE>> GetPhotos;

    private Dictionary<Texture2D, PHOTO_TYPE> _selectedPhoto = new Dictionary<Texture2D, PHOTO_TYPE>();

    private Dictionary<Texture2D, Texture2D> _bilateralPhoto = new Dictionary<Texture2D, Texture2D>();

    public Dictionary<Texture2D, Texture2D> bilateralPhoto { get { return _bilateralPhoto; } }

    bool _toggleOn = false;

    public override void InitComponent()
    {
        //throw new System.NotImplementedException();
        _filterToggles.AddRange(GetComponentsInChildren<UC_Filter>());
        _skinToggle.onValueChanged.AddListener((state) =>
        {
            if(_toggleOn)
                SendBilateral(state);
        });

        for(int i = 0; i < _filterToggles.Count; i++)
        {
            int index = i;
            _filterToggles[i].Select(i == 0);
            _filterToggles[i].OnToggleValueChanged += (isOn) => OnToggleSelected(index, isOn);
        }
    }

    private void OnToggleSelected(int index, bool isOn)
    {
        if (isOn)
        {
            for (int i = 0; i < _filterToggles.Count; i++)
            {
                _filterToggles[i].toggle.isOn = i == index;
            }
        }
    }

    public override void SetActivate(bool state)
    {
        //for (int i = 0; i < _filterToggles.Count; i++)
        //{
        //    _filterToggles[i].SetActivate(state);
        //}
        _skinToggle.isOn = false;
        _toggleOn = false;

        base.SetActivate(state);
        if (state)
        {
            _selectedPhoto = GetPhotos();
            if (_type == CONTENT_TYPE.AI_CARTOON)
            {
                _content[(int)CONTENT.FILTER].SetActive(true);
                _content[(int)CONTENT.SKIN].SetActive(true);
                //현재 들고있는 파일에따라 on/off 설정

                CheckPhotoHasReal();
            }
            else
            {
                _content[(int)CONTENT.FILTER].SetActive(true);
                _content[(int)CONTENT.SKIN].SetActive(false);
            }

        }

    }

    void CheckPhotoHasReal()
    {
        bool hasReal = false;
        foreach (var pair in _selectedPhoto)
        {
            Debug.Log("dict [ " + pair.Key + ", " + pair.Value + "]");
            if (pair.Value == PHOTO_TYPE.REAL)
            {
                hasReal = true;
            }
        }

        _toggleOn = hasReal;
        SetSkinEnable(hasReal);
    }
    
    void SetSkinEnable(bool state)
    {
        if (state)
        {
            _skinToggles[(int)SKIN.OFF].SetActive(false);

            foreach (var pair in _selectedPhoto)
            {
                if (pair.Value == PHOTO_TYPE.REAL)
                {
                    //Texture2D skinFilterd;
                    //BilateralFilterModule.BilateralFilter(pair.Key, out skinFilterd);

                    //_bilateralPhoto[pair.Key] = skinFilterd;
                }
                else
                {
                    _bilateralPhoto[pair.Key] = null;
                }
            }
        }
        else
            _skinToggles[(int)SKIN.OFF].SetActive(true);
    }

    enum CONTENT
    {
        FILTER,
        SKIN
    }

    enum SKIN
    {
        ON,
        OFF
    }
}
