using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vivestudios.UI;

public class UC_SelectPhoto : UC_BaseComponent
{
    [SerializeField]
    private GameObject _photoPrefab;

    protected List<Texture2D> _photoConverted = new List<Texture2D>();
    protected List<Texture2D> _photoOrigin = new List<Texture2D>();

    protected List<TextMeshProUGUI> _texts = new List<TextMeshProUGUI>();
    protected List<UC_SelectablePhoto> _selectables = new List<UC_SelectablePhoto>();
    protected Dictionary<int, UC_SelectablePhoto> _dictSelected = new Dictionary<int, UC_SelectablePhoto>();
    protected Dictionary<int, int> _dictSelectedOrder = new Dictionary<int, int>();

    public List<int> order = new List<int>();

    protected LayoutGroup _body;
    protected PHOTO_TYPE _photoType;

    [SerializeField]
    protected int _photoCount;
    [SerializeField]
    protected int _selectMax;

    [SerializeField]
    private TextMeshProUGUI _countText;

    public Action<PHOTO_TYPE, int, Texture2D> AddSelectPhoto;
    public Action<int, Texture2D> RemoveSelectPhoto;
    public Action<bool> ActivatePageBtn;

    public List<Texture2D> photoConverted { set { _photoConverted = value; } }

    public List<Texture2D> photoOrigin { set { _photoOrigin = value; } }

    public int photoCount { set { _photoCount = value; } }

    public int selectMax { set { _selectMax = value; } }

    bool _isFull = false;
    public bool isFull
    {
        get { return _isFull; }
        set { _isFull = value; }
    }

    private int _createdPhotoNum = 0;
    private bool _isInitialized = false;

    public void InitDictSelect()
    {
        if (_isInitialized == true)
            return;

        _dictSelected.Clear();
        _dictSelectedOrder.Clear();
        for (int i = 0; i < _selectMax; i++)
        {
            _dictSelected[i] = null;
            _dictSelectedOrder[i] = -1;
        }

        _isInitialized = true;
        order = new List<int>();
    }

    public PHOTO_TYPE photoType { set { _photoType = value; } }

    public override void InitComponent()
    {
        _body = GetComponentInChildren<LayoutGroup>();

        GameManager.OnGameResetAction += OnReset;
    }

    private void OnReset()
    {
        _createdPhotoNum = 0;
        _isInitialized = false;
    }

    public virtual void CreatePhoto(int num, Texture2D textures, PHOTO_TYPE type = PHOTO_TYPE.CONVERTED)
    {
        if (UserDataManager.inst.selectedContent == CONTENT_TYPE.AI_CARTOON)
        {
            if (_createdPhotoNum >= _photoConverted.Count * 2)
                return;
        }
        else
        {
            if (_createdPhotoNum >= _photoConverted.Count)
                return;
        }


        GameObject clone = Instantiate(_photoPrefab, _body.transform);
        UC_SelectablePhoto selectable = clone.GetComponent<UC_SelectablePhoto>();
        selectable.Setting();
        selectable.SetIndex(num);
        selectable.type = type;
        selectable.onValueChanged.AddListener((isOn) =>
        {
            SelectNumbering(selectable);
        });

        selectable.texture = textures;

        _createdPhotoNum++;
    }

    public void SelectNumbering(UC_SelectablePhoto selectable)
    {
        if (_dictSelected.ContainsValue(selectable))
        {
            // 사진이 이미 선택된경우
            foreach (var pair in _dictSelected)
            {
                if (pair.Value == selectable)
                {
                    RemoveSelectPhoto(pair.Key, pair.Value.texture);
                    _dictSelected[pair.Key] = null;
                    //_dictSelectedOrder[]
                    selectable.OnValueChanged();
                    isFull = false;
                    ActivatePageBtn(isFull);
                    break;
                }
            }
        }
        else
        {
            // 사진이 선택되어있지 않은 경우
            foreach (var pair in _dictSelected)
            {
                if (pair.Value == null)
                {
                    AddSelectPhoto(selectable.type, pair.Key, selectable.texture);
                    _dictSelected[pair.Key] = selectable;
                    //selectable.SetIndex(pair.Key);
                    selectable.OnValueChanged();
                    break;
                }
            }

            bool isFull = true;
            foreach (var pair in _dictSelected)
            {
                if (pair.Value == null)
                    isFull = false;
            }

            if (isFull)
            {
                _isFull = true;
                ActivatePageBtn(isFull);
            }
        }

        CountCheck();
    }

    private void OnEnable()
    {
        CountCheck();
    }

    private void CountCheck()
    {
        int selected = 0;
        foreach (var elem in _dictSelected.Values)
        {
            if (elem != null)
            {
                selected++;
            }
        }

        _countText.text = $"{selected}/{_selectMax}";
    }

    public List<int> SetOrder()
    {
        foreach (var pair in _dictSelected)
        {
            order.Add(pair.Value.Index);
        }

        return order;
    }
}
