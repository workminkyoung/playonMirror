using MPUIKIT;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Vivestudios.UI;

public class UP_DecoSelectSticker : UP_DecoratePageBase
{
    [SerializeField]
    private RectTransform _categoryContent;
    [SerializeField]
    private RectTransform _stickerContent;
    [SerializeField]
    private UC_CategoryToggle _allCategoryToggle;
    [SerializeField]
    private List<UC_CategoryToggle> _categoryToggles = new List<UC_CategoryToggle>();
    [SerializeField]
    private List<UC_StickerThumbnail> _stickerThumbnails = new List<UC_StickerThumbnail>();
    [SerializeField]
    private Button _nextBtn, _prevBtn;
    [SerializeField]
    private GameObject _stickerContainer;
    [SerializeField]
    private RectTransform _stickerArea;

    [SerializeField]
    private List<GameObject> _createdStickers = new List<GameObject>();

    [Header("Prefabs")]
    [SerializeField]
    private GameObject _categoryTogglePrefab;
    [SerializeField]
    private GameObject _stickerThumbnailPrefab;
    [SerializeField]
    private GameObject _controllerableStickerPrefab;

    [Header("DEBUG")]
    [SerializeField]
    private List<string> _categories;
    [SerializeField]
    private List<int> _groups;

    private Dictionary<int, GameObject> _stickerAreas = new Dictionary<int, GameObject>();

    private bool isStickerCreated = false;

    public override void InitPage ()
    {
        CreateCategories();
        CreateStickerAreas();

    }

    public override void BindDelegates ()
    {
        base.BindDelegates();

        _prevBtn.onClick.AddListener(OnClickPrev);
        _nextBtn.onClick.AddListener(OnClickNext);

        BindCategoryToggles();
    }

    private void OnClickPrev ()
    {
        switch(UserDataManager.inst.selectedContent)
        {
            case CONTENT_TYPE.AI_CARTOON:
                _pageController.ChangePage(PAGE_TYPE.PAGE_DECO_SELECT_PICS_CARTOON);
                break;
            case CONTENT_TYPE.AI_PROFILE:
                _pageController.ChangePage(PAGE_TYPE.PAGE_DECO_SELECT_PICS_PROFILE);
                break;
            case CONTENT_TYPE.AI_BEAUTY:
                _pageController.ChangePage(PAGE_TYPE.PAGE_DECO_SELECT_PICS_BEAUTY);
                break;
            case CONTENT_TYPE.WHAT_IF:
                _pageController.ChangePage(PAGE_TYPE.PAGE_DECO_SELECT_PICS_WHAT_IF);
                break;
        }
    }

    private void OnClickNext ()
    {
        (_pageController as PC_Main).stickerContainerPrefab = _stickerContainer;
        (_pageController as PC_Main).StickerUpdateAction?.Invoke();

        (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_DECO_SELECT_EFFECT);
    }


    public override void OnPageEnable ()
    {
        _allCategoryToggle.Select(true);

        if(isStickerCreated == false)
        {
            CreateStickers();
        }
    }

    public override void OnPageDisable ()
    {
    }

    protected override void OnPageReset ()
    {
        foreach(var elem in _createdStickers)
        {
            Destroy(elem.gameObject);
        }

        _createdStickers.Clear();
    }

    private void CreateCategories ()
    {
        _categories = ResourceCacheManager.inst.stickerOptions.Select(option => option.category).Distinct().ToList();
        for(int i = 0; i < _categories.Count; i++)
        {
            int index = i;
            UC_CategoryToggle newCategory = GameObject.Instantiate(_categoryTogglePrefab).GetComponent<UC_CategoryToggle>();
            newCategory.transform.SetParent(_categoryContent);
            (newCategory.transform as RectTransform).anchoredPosition3D = Vector3.zero;
            newCategory.transform.localScale = Vector3.one;
            newCategory.SetText(_categories[i]);
            newCategory.pointerClickAction += () => OnClickCategory(_categories[index]);
            _categoryToggles.Add(newCategory);
        }
    }

    private void BindCategoryToggles ()
    {
        _allCategoryToggle.pointerClickAction += () => OnClickCategory("all");
    }

    private void OnClickCategory (string category)
    {
        if(category == "all")
        {
            foreach(var elem in _categoryToggles)
            {
                elem.Select(false);
            }
        }
        else
        {
            foreach(var elem in _categoryToggles)
            {
                if(elem.category != category)
                {
                    elem.Select(false);
                }
            }
            _allCategoryToggle.Select(false);
        }

        UpdateCategory(category);
    }

    private void CreateStickerAreas ()
    {
        _groups = ResourceCacheManager.inst.stickerOptions.Select(option => option.group).Distinct().ToList();
        for(int i = 0; i < _groups.Count; i++)
        {
            MPImage newStickerArea = new GameObject().AddComponent<MPImage>();
            newStickerArea.name = $"StickerArea_{_groups[i]}";
            newStickerArea.transform.SetParent(_stickerContent);

            newStickerArea.rectTransform.anchoredPosition3D = Vector3.zero;
            newStickerArea.rectTransform.localScale = Vector3.one;
            newStickerArea.rectTransform.sizeDelta = new Vector2(748, 180);

            newStickerArea.DrawShape = DrawShape.Rectangle;
            var rect = newStickerArea.Rectangle;
            if(rect.RectTransform == null)
            {
                rect.RectTransform = newStickerArea.rectTransform;
            }

            rect.CornerRadius = new Vector4(16, 16, 16, 16);
            newStickerArea.Rectangle = rect;
            newStickerArea.SetMaterialDirty();

            HorizontalLayoutGroup layoutGroup = newStickerArea.gameObject.AddComponent<HorizontalLayoutGroup>();
            layoutGroup.padding.left = 4;
            layoutGroup.padding.right = 0;
            layoutGroup.padding.top = 4;
            layoutGroup.padding.bottom = 0;
            layoutGroup.spacing = 17;
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = false;
            layoutGroup.childScaleWidth = true;
            layoutGroup.childScaleHeight = true;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childForceExpandHeight = false;

            _stickerAreas.Add(_groups[i], newStickerArea.gameObject);
        }
    }

    private void CreateStickers ()
    {
        foreach(var option in ResourceCacheManager.inst.stickerOptions)
        {
            UC_StickerThumbnail newSticker = GameObject.Instantiate(_stickerThumbnailPrefab).GetComponent<UC_StickerThumbnail>();
            newSticker.transform.SetParent(_stickerAreas[option.group].transform);
            newSticker.rectTransform.anchoredPosition3D = Vector3.zero;
            newSticker.transform.localScale = Vector3.one;
            newSticker.transform.localEulerAngles = Vector3.zero;

            newSticker.SetOption(option);
            newSticker.OnClickAction += OnClickStickerThumbnail;

            _stickerThumbnails.Add(newSticker);
        }

        isStickerCreated = true;
    }

    private void OnClickStickerThumbnail (StickerOptionBase option)
    {
        for(int i = _createdStickers.Count - 1; i > 0; i--)
        {
            if(_createdStickers[i] != null)
            {
                continue;
            }

            _createdStickers.RemoveAt(i);
        }

        if(_createdStickers.Count >= ConfigData.config.stickerMaxCount)
        {
            // 스티커 최대 갯수 초과
            (pageController as PC_Main).globalPage.OpenToast("스티커 최대 갯수에 도달했습니다", 3);
            return;
        }

        UC_StickerThumbnail newSticker = GameObject.Instantiate(_controllerableStickerPrefab).GetComponent<UC_StickerThumbnail>();
        newSticker.transform.SetParent(_stickerArea);
        newSticker.rectTransform.anchoredPosition3D = Vector3.zero;
        newSticker.transform.localScale = Vector3.one;
        newSticker.transform.localEulerAngles = Vector3.zero;

        newSticker.SetOption(option);

        _createdStickers.Add(newSticker.gameObject);
    }

    private void UpdateCategory (string category)
    {
        if(category == "all")
        {
            foreach(var elem in _stickerThumbnails)
            {
                elem.gameObject.SetActive(true);
            }
        }
        else
        {
            foreach(var elem in _stickerThumbnails)
            {
                elem.gameObject.SetActive(elem.category == category);
            }
        }

        foreach(var elem in _stickerAreas)
        {
            elem.Value.SetActive(AreAllChildrenDeactivated(elem.Value) == false);
        }
    }

    private bool AreAllChildrenDeactivated (GameObject parent)
    {
        foreach(Transform child in parent.transform)
        {
            if(child.gameObject.activeSelf)
            {
                // 하나라도 활성화되어 있으면 false를 반환
                return false;
            }
        }
        // 모든 자식이 비활성화 되어 있으면 true를 반환
        return true;
    }

    private void Update ()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            OnPageEnable();
        }
    }
}
