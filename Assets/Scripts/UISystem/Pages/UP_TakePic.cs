using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vivestudios.UI;

public class UP_TakePic : UP_BasePage, IPageTimeLimit
{
    [SerializeField]
    private TextMeshProUGUI _timeText;
    [SerializeField]
    private int _maxTime;
    [SerializeField]
    private TextMeshProUGUI _picCountText;


    [SerializeField]
    private RawImage _previewRawImg;
    [SerializeField]
    private RectTransform _guidGrid;

    [SerializeField]
    private RectTransform _profileTransform;
    [SerializeField]
    private RectTransform _timeMachineTransform;
    [SerializeField]
    private UC_AgeSlider[] _ageSliders;


    private int _maxPicCount = 0;
    private int _curPicCount = 0;

    private Coroutine TimeRimitCoroutine = null;

    private readonly Vector2 HORIZONTAL_GRID_SIZE = new Vector2(1280, 960);
    private readonly Vector2 VERTICAL_GRID_SIZE = new Vector2(768, 960);

    public TextMeshProUGUI timeText { get => _timeText; set => _timeText = value; }
    public int MaxTime { get => _maxTime; set => _maxTime = value; }

    public override void InitPage()
    {
    }

    public override void BindDelegates()
    {
    }

    private void OnEnable()
    {
        if (!_pageController)
            return;

        switch (UserDataManager.inst.selectedContent)
        {
            case CONTENT_TYPE.AI_CARTOON:
                _profileTransform.gameObject.SetActive(false);
                _timeMachineTransform.gameObject.SetActive(false);
                break;
            case CONTENT_TYPE.AI_PROFILE:
                _profileTransform.gameObject.SetActive(true);
                _timeMachineTransform.gameObject.SetActive(false);
                break;
            case CONTENT_TYPE.AI_TIME_MACHINE:
                _profileTransform.gameObject.SetActive(false);
                _timeMachineTransform.gameObject.SetActive(true);
                break;
        }

        OpenCamera();
        SetGuideGrid();
        // TODO : MaxPicCount 셋팅, 가이드 그리드 셋팅, 프리뷰 셋팅
    }

    private void Update()
    {
        if (_previewRawImg.texture != null)
        {
        }
    }

    private void OpenCamera()
    {
        if (ConfigData.config.camType == (int)CAMERA_TYPE.WEBCAM)
        {
            CameraManager.Instance.FindWebcam();

            _previewRawImg.texture = CameraManager.Instance.webCamTexture;
        }
        else if (ConfigData.config.camType == (int)CAMERA_TYPE.NDI)
        {
            _previewRawImg.texture = NDIManager.Instance.ndiTexture;
        }
        else
        {

        }
    }

    private void SetPreview()
    {

    }

    private void SetGuideGrid()
    {
        if (UserDataManager.inst.selectedFrame == FRAME_TYPE.FRAME_4 ||
            UserDataManager.inst.selectedFrame == FRAME_TYPE.FRAME_1)
        {
            _guidGrid.sizeDelta = VERTICAL_GRID_SIZE;
        }
        else
        {
            _guidGrid.sizeDelta = HORIZONTAL_GRID_SIZE;
        }
    }

    private IEnumerator TimeLimitRoutine()
    {
        yield return null;
    }

    public override void OnPageEnable()
    {
    }

    public override void OnPageDisable()
    {
    }

    protected override void OnPageReset()
    {
    }
}
