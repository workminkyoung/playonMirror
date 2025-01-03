using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vivestudios.UI;
using System;
using UnityEngine.UI;
using FFmpegOut;
using RotaryHeart.Lib.SerializableDictionary;
using System.IO;

public class PC_Main : PC_BasePageController
{
    [SerializeField]
    [ReadOnly]
    private bool _isSkinFilterOn = true;
    public bool isSkinFilterOn { get { return _isSkinFilterOn; } }

    [SerializeField]
    private UP_Global _globalPage;
    public UP_Global globalPage { get { return _globalPage; } }

    // 변환 이미지, 원본 이미지 리스트 저장

    // 타입에 따른 촬영개수, 선택개수 저장
    //private bool _isLandscape = true;
    //public bool isLandscape { get { return _isLandscape; } }
    //public bool SetisLandscape { set { _isLandscape = value; } }

    [SerializeField]
    private List<Texture2D> _resultTextures;
    // 프린트 할때 이거 쓰셈
    public List<Texture2D> resultTextures { get { return _resultTextures; } }

    [SerializeField]
    private List<int> _resultIndexs;
    // 프린트 할때 이거 쓰셈
    public List<int> resultIndexs { get { return _resultIndexs; } }

    public Action OnShuffleAction;
    public Action<int> OnTimeUpdateAction;
    public Action OnTimeLimitDoneAction;

    private Coroutine TimeLimitCoroutine = null;

    // record data
    [SerializeField]
    private List<RenderTexture> _recordRenderTextures = new List<RenderTexture>();
    [SerializeField]
    private CaptureCamBase _captureCameras = new CaptureCamBase();
    public CaptureCamBase captureCameras { get { return _captureCameras; } }
    [SerializeField]
    private CaptureRawimageBase _captureRawimgs = new CaptureRawimageBase();
    public CaptureRawimageBase captureRawimgs { get { return _captureRawimgs; } }
    //private List<string> _recordPaths = new List<string>();
    //public List<string> recordPaths { get { return _recordPaths; } }
    //private List<int> _selectedOrder = new List<int>();
    //public List<int> selectedOrder { get { return _selectedOrder; } }
    //public void SetSelectOrder(List<int> order)
    //{
    //    _selectedOrder = order;
    //}

    // Set cartoon type reference data
    [SerializeField]
    private Texture2D[] _cartoonReferences;

    public Action OnFrameUpdateAction;

    //private bool _isPaymentOn = true;
    public Action<bool> OnPaymentChangeAction;


    private bool _isCameraCheckTextOn = false;
    public bool isCameraCheckTextOn { get { return _isCameraCheckTextOn; } }

    //// ------- Save Image video Path

    //[SerializeField]
    //private List<string> _recordVidPath = new List<string>();

    //public string imagePath { set { _imagePath = value; } }
    //public string videoPath { set { _videoPath = value; } }
    //public List<string> recordVidPath { set { _recordVidPath = value; } }
    //// --------------

    private bool _timeLimitDone = false;
    public bool timeLimitDone { get { return _timeLimitDone; } }

    //private bool _isChildPlaying = false;
    //public bool isChildPlaying { get { return _isChildPlaying; } }
    //public bool SetIsChildPlaying { set { _isChildPlaying = value; } }

    public GameObject stickerContainerPrefab;
    public Action StickerUpdateAction;

    #region LogFile
    private static string logDirectoryPath;
    private static string logFilePath;
    #endregion

    public void SkinFilterOn(bool isOn)
    {
        _isSkinFilterOn = isOn;
    }

    public void ShuffleContents()
    {
        OnShuffleAction?.Invoke();
    }

    //TODO : RESET
    public void SetRecordTexture(bool isSpout = false)
    {
        if (PhotoDataManager.inst.isLandscape)
        {
            _captureCameras[CAMERA_VIEW_TYPE.RECORD].targetTexture = _recordRenderTextures[1];
        }
        else
        {
            _captureCameras[CAMERA_VIEW_TYPE.RECORD].targetTexture = _recordRenderTextures[0];
        }

        if(ConfigData.config.camType == (int)CAMERA_TYPE.WEBCAM)
        {
            if(UserDataManager.inst.isChromaKeyOn)
            {
                _captureRawimgs[CAMERA_VIEW_TYPE.RECORD].texture = ChromaKeyModule.inst.resultRT;
            }
            else
            {
                _captureRawimgs[CAMERA_VIEW_TYPE.RECORD].texture = CameraManager.Instance.webCamTexture;
            }
        }
        else if(ConfigData.config.camType == (int)CAMERA_TYPE.NDI)
        {
            _captureRawimgs[CAMERA_VIEW_TYPE.RECORD].texture = NDIManager.Instance.ndiTexture;
        }
        else
        {

            //dslr
        }
    }


    //TODO : RESET
    public void SetRecordCamera()
    {
        RECORD_CAMERA_TEXTURE type = RECORD_CAMERA_TEXTURE.PORTRAIT;
        if (PhotoDataManager.inst.isLandscape)
            type = RECORD_CAMERA_TEXTURE.LANDSCAPE;

        _captureCameras[CAMERA_VIEW_TYPE.RECORD].targetTexture = _recordRenderTextures[(int)type];
        ffmpegManager.instance.Setting(_captureCameras[CAMERA_VIEW_TYPE.RECORD]);
    }

    //public void AddRecordPath(string path)
    //{
    //    _recordPaths.Add(path);
    //    _recordVidPath.Add(path);
    //}
    //public void SetRecordPath(List<string> paths)
    //{
    //    _recordPaths = paths;
    //}

    public void SetResultTextures(List<Texture2D> result)
    {
        _resultTextures = result;
    }

    public void SetResultIndexs(List<int> result)
    {
        _resultIndexs = result;
    }

    // set cartoon reference data

    public void ResetGame()
    {
        ffmpegManager.Instance.Setting(_captureCameras[CAMERA_VIEW_TYPE.RECORD]);

        //_selectedFrameColor = FRAME_COLOR_TYPE.FRAME_WHITE;
        //_selectedLut = LUT_EFFECT_TYPE.LUT_DEFAULT;
        _isSkinFilterOn = false;

        //_photoConverted = new List<Texture2D>();
        //_photoOrigin = new List<Texture2D>();
        //_selectedPhoto = new Dictionary<Texture2D, PHOTO_TYPE>();
        //_selectedPicDic = new Dictionary<int, UC_SelectablePic>();

        _timeLimitDone = false;
        _isSkinFilterOn = true;
        _resultTextures = new List<Texture2D>();
        _resultIndexs = new List<int>();

        Resources.UnloadUnusedAssets();

        ShuffleContents();
        StopTimeLimit();

        //GameManager.inst.OnGameResetAction?.Invoke();


        //videoPath = string.Empty;
        //imagePath = string.Empty;
        //recordVidPath = new List<string>();

        #region ONLY FOR DEBUG
        if (Debug.isDebugBuild)
        {
            GameManager.inst.SetPaymentOn(false);
            _globalPage.PaymentTextOn(true);
        }
        else
        {
            GameManager.inst.SetPaymentOn(true);
            _globalPage.PaymentTextOn(false);
        } 

#if UNITY_EDITOR
        GameManager.inst.SetPaymentOn(false);
        _globalPage.PaymentTextOn(true);
#endif
        #endregion

        OnPaymentChangeAction?.Invoke(GameManager.inst.isPaymentOn);

        if (PhotoPaperCheckModule.GetRemainPhotoPaper() <= 1)
        {
            globalPage.EmptyPhotoPaperAlertOn(true);
            MailingModule.inst.SendMail(MAIL_TYPE.REMAIN_PAPER);
        }
        else
        {
            globalPage.EmptyPhotoPaperAlertOn(false);
        }

        ChangePage(PAGE_TYPE.PAGE_AOD);
    }
    protected override void Awake()
    {
        base.Awake();

        InitializeLogHandler();
        InitializeGlobalExceptionHandling();
        GameManager.OnGameResetAction += ResetGame;

        for (int i = 0; i < _cartoonReferences.Length; i++)
        {
            Cartoon cartoon = new Cartoon();
            cartoon.type = (CARTOON_TYPE)i;
            cartoon.model = TextData.cartoonModel[i];
            cartoon.reference = _cartoonReferences[i];

            CartoonManager.Instance.SetCartoon(cartoon);
        }

        //SetCameraData();
        ffmpegManager.Instance.Setting(_captureCameras[CAMERA_VIEW_TYPE.RECORD]);

        if (LogDataManager.inst == null)
        {
            LogDataManager.inst = LogDataManager.Instance;
        }

        _globalPage.ErrorOpenAction += () => 
        { 
            if(!GameManager.inst.isCameraConnected || !GameManager.inst.isPaymentReaderConnected)
            {
                MailingModule.inst.SendMail(MAIL_TYPE.ERROR);
            }else if (!GameManager.inst.isDiffusionSuccess)
            {
                MailingModule.inst.SendMail(MAIL_TYPE.DIFFUSION_ERROR);
            }
            else if (!GameManager.inst.isQRUploadSuccess)
            {
                MailingModule.inst.SendMail(MAIL_TYPE.QR_ERROR);
            }
        };

        //TODO : Config load manager로 정리한후 아래코드 변경(cam type확인필요)
        DSLRManager.Instance.OnLoadPreview += (preview) =>
        {
            if(UserDataManager.inst.isChromaKeyOn)
            {
                ChromaKeyModule.inst.SetCamImg(preview);
                _captureRawimgs[CAMERA_VIEW_TYPE.RECORD].texture = ChromaKeyModule.inst.resultRT;
            } 
            else
            {
                _captureRawimgs[CAMERA_VIEW_TYPE.RECORD].texture = preview;
            }
            
        };
    }

    public void SetCameraData()
    {
        if (ConfigData.config.camType == (int)CAMERA_TYPE.WEBCAM)
            _captureRawimgs[CAMERA_VIEW_TYPE.RECORD].texture = CameraManager.Instance.webCamTexture;
        else if (ConfigData.config.camType == (int)CAMERA_TYPE.NDI)
            _captureRawimgs[CAMERA_VIEW_TYPE.RECORD].texture = NDIManager.Instance.ndiTexture;
        else
        {
            //dslr
        }
    }

    public void UpdateFrame()
    {
        OnFrameUpdateAction?.Invoke();
    }

    public void StartTimeLimit(int limitTime = 0)
    {
        if (TimeLimitCoroutine != null)
        {
            return;
        }

        TimeLimitCoroutine = StartCoroutine(TimeLimitRoutine(limitTime));
    }

    public void StopTimeLimit()
    {
        if (TimeLimitCoroutine != null)
        {
            StopCoroutine(TimeLimitCoroutine);
        }

        TimeLimitCoroutine = null;
    }

    private IEnumerator TimeLimitRoutine(int limitTime = 0)
    {
        int time = 0;
        if (limitTime == 0)
        {
            limitTime = AdminManager.Instance.BasicSetting.Config.OtherMenu_data;//ConfigData.config.decoPageTime;
        }

        while (time <= limitTime)
        {
            OnTimeUpdateAction?.Invoke(limitTime - time);

            yield return new WaitForSecondsRealtime(1);
            time++;
        }

        _timeLimitDone = true;
        TimeLimitCoroutine = null;
        OnTimeLimitDoneAction?.Invoke();
    }

    private void Update()
    {
        // Check Abnormal Quit
        MonitorMemoryUsage();
        MonitorFPS();

        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Application.Quit();
            }
            else if (Input.GetKeyDown(KeyCode.M))
            {
                Cursor.visible = Cursor.visible ? false : true;
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                //if (curSection == (int)eSection.Intro || curSection == (int)eSection.Final)
                //if (!isWaitHome)
                //{
                //    RecordUIManager.Instance.ForceStopRecord();
                //    SwitchSection(eSection.Intro);
                //}
                if (!_pageDic[PAGE_TYPE.PAGE_AOD].gameObject.activeInHierarchy)
                {
                    GameManager.inst.ResetGame();
                }
            }
#if UNITY_EDITOR
            else if (Input.GetKeyDown(KeyCode.R))
            {
                ResetPhotoPaper();
            }
            else if (Input.GetKeyDown(KeyCode.I))
            {
                _globalPage.OpenAlertPopup("잔여 인화지", $"현재 남은 인화지 수 입니다.\n{PhotoPaperCheckModule.GetRemainPhotoPaper()} 장");
            }
#endif
            else if (Input.GetKey(KeyCode.P))
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    ResetPhotoPaper();
                }

                if (Input.GetKeyDown(KeyCode.S))
                {
                    _globalPage.OpenAlertPopup("잔여 인화지", $"현재 남은 인화지 수 입니다.\n{PhotoPaperCheckModule.GetRemainPhotoPaper()} 장");
                }
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                GameManager.inst.SetPaymentOn(!GameManager.inst.isPaymentOn);
                globalPage.PaymentTextOn(!GameManager.inst.isPaymentOn);
                OnPaymentChangeAction?.Invoke(GameManager.inst.isPaymentOn);
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                _isCameraCheckTextOn = !_isCameraCheckTextOn;
                globalPage.CameraTextOn(_isCameraCheckTextOn, DSLRManager.Instance.CheckConnected());
            }
            else if (Input.GetKeyDown(KeyCode.V))
            {
                _globalPage.ToggleVersionText();
            }
            else if (Input.GetKey(KeyCode.LeftShift))
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    using (var proc = new System.Diagnostics.Process())
                    {
                        proc.StartInfo.FileName = Path.Combine(Application.dataPath, @"../", "snapai.exe");
                        proc.Start();
                    }
                    Application.Quit();
                }
                if (Input.GetKeyDown(KeyCode.K))
                {
                    _globalPage.OpenChromaKeySetting();
                }
            }
        }
    }

    #region Monoitoring

    /// <summary>
    /// 메모리 사용량 초과 감지
    /// </summary>
    void MonitorMemoryUsage()
    {
        long totalMemory = System.GC.GetTotalMemory(false);
        if (totalMemory > 40L * 1024 * 1024 * 1024) // 40GB 이상 사용 시 경고
        {
            CustomLogger.LogWarning($"메모리 사용량이 위험 수준입니다: {totalMemory / (1024 * 1024)} MB");
        }
    }

    void MonitorFPS()
    {
        float currentFPS = 1.0f / Time.deltaTime;
        if (currentFPS < 10) // FPS가 10 이하로 떨어질 경우 경고
        {
            CustomLogger.LogWarning($"FPS 저하 감지: {currentFPS}");
        }
    }

    void InitializeGlobalExceptionHandling()
    {
        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            CustomLogger.LogError($"Unhandled Exception: {args.ExceptionObject}");
        };

        Application.logMessageReceived += (condition, stackTrace, type) =>
        {
            if (type == LogType.Exception || type == LogType.Error)
            {
                CustomLogger.LogError($"Critical Log: {condition}, {stackTrace}");
            }
        };
    }

    void InitializeLogHandler()
    {
        // 로그 디렉토리 설정
        logDirectoryPath = Path.Combine(Application.persistentDataPath, "Logs");
        if (!Directory.Exists(logDirectoryPath))
        {
            Directory.CreateDirectory(logDirectoryPath);
        }

        // 날짜별 파일 이름 설정
        string date = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        logFilePath = Path.Combine(logDirectoryPath, $"Log_{date}.txt");

        // 기존 로그 파일 초기화
        File.WriteAllText(logFilePath, "=== Unity Log Start ===\n");

        // 로그 메시지 수신 핸들러 등록
        Application.logMessageReceived += HandleLog;
    }

    private void HandleLog(string condition, string stackTrace, LogType type)
    {
        string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{type}] {condition}\n";
        if (type == LogType.Exception || type == LogType.Error)
        {
            logEntry += $"StackTrace:\n{stackTrace}\n";
        }

        // 로그를 파일에 추가
        File.AppendAllText(logFilePath, logEntry);
    }

    #endregion

    private void ResetPhotoPaper()
    {
        globalPage.ResetPhotopaperPopupOn(true);

        //globalPage.OpenAlertPopup("인화지 수량 초기화", "인화지 수량이 초기화 되었습니다.\n수량에 따라 알림이 발송됩니다.", () =>
        //{
        //    globalPage.EmptyPhotoPaperAlertOn(false);
        //});
    }

    public void UpdatePhotoPaper()
    {
        bool mailSended = false;

        int _defaultAmount = AdminManager.inst.FrameData.ServiceFrame.Code[UserDataManager.Instance.selectedContentKey].DefaultSellAmount;

        float paperDivisionFactor = UserDataManager.inst.selectedFrameType == FRAME_TYPE.FRAME_8 ? _defaultAmount : 1;

        // for문으로 처리 => 100, 50, 정확한 숫자에 Mailing 목적 (부등호로 처리시 지속적으로 메일 발송)
        for (int i = 0; i < UserDataManager.inst.curPicAmount/paperDivisionFactor; i++)
        {
            PhotoPaperCheckModule.SetRemainPhotoPaper(PhotoPaperCheckModule.GetRemainPhotoPaper() - 1);

            if (PhotoPaperCheckModule.GetRemainPhotoPaper() == 100 ||
                PhotoPaperCheckModule.GetRemainPhotoPaper() == 50 ||
                PhotoPaperCheckModule.GetRemainPhotoPaper() <= 20)
            {
                if (mailSended == false)
                {
                    MailingModule.inst.SendMail(MAIL_TYPE.REMAIN_PAPER);
                    mailSended = true;
                }
            }
            if (PhotoPaperCheckModule.GetRemainPhotoPaper() <= 0)
            {
                globalPage.EmptyPhotoPaperAlertOn(true);
            }
        }
    }
    private void OnDestroy()
    {
        // 핸들러 해제
        Application.logMessageReceived -= HandleLog;
    }

    [Serializable]
    public class LutTexDicBase : SerializableDictionaryBase<LUT_EFFECT_TYPE, Texture2D> { }
    [Serializable]
    public class CaptureCamBase : SerializableDictionaryBase<CAMERA_VIEW_TYPE, Camera> { }
    [Serializable]
    public class CaptureRawimageBase : SerializableDictionaryBase<CAMERA_VIEW_TYPE, RawImage> { }
}