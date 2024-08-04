using System;
using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager>
{
    [SerializeField]
    private UP_Global _globalPage;

    [SerializeField]
    private bool _isPaymentOn;
    [SerializeField]
    private bool _isChildPlaying;
    [SerializeField]
    private bool _isCameraConnected = true;
    [SerializeField]
    private bool _isCameraTempConnected = true;
    [SerializeField]
    private bool _isPaymentReaderConnected = true;
    [SerializeField]
    private bool _isInternetReachable = true;
    [SerializeField]
    private bool _isDiffusionSuccess = true;
    [SerializeField]
    private bool _isQRUploadSuccess = true;

    public UP_Global globalPage => _globalPage;
    public bool isPaymentOn => _isPaymentOn;
    public bool isChildPlaying => _isChildPlaying;
    public bool isCameraConnected => _isCameraConnected;
    public bool isCameraTempConnected => _isCameraTempConnected;
    public bool isPaymentReaderConnected => _isPaymentReaderConnected;
    public bool isInternetReachable => _isInternetReachable;
    public bool isDiffusionSuccess => _isDiffusionSuccess;
    public bool isQRUploadSuccess => _isQRUploadSuccess;

    public static Action OnGameLateInitAction;
    public static Action OnGameResetAction;

    public void SetPaymentOn(bool paymentOn)
    {
        _isPaymentOn = paymentOn;
    }

    public void SetChildPlaying(bool childPlaying)
    {
        _isChildPlaying = childPlaying;
    }

    public void SetCameraConnected(bool isCameraConnected)
    {
        _isCameraConnected = isCameraConnected;
    }
    public void SetCameraTempConnected(bool isCameraConnected)
    {
        _isCameraTempConnected = isCameraConnected;
    }

    public void SetPaymentReaderConnected(bool paymentReaderConnected)
    {
        _isPaymentReaderConnected = paymentReaderConnected;
    }

    public void SetDiffusionState(bool isSuccessed)
    {
        _isDiffusionSuccess = isSuccessed;
    }

    public void SetQRUploadState(bool isSuccessed)
    {
        _isQRUploadSuccess = isSuccessed;
    }

    public void SetGlobalPage(UP_Global page)
    {
        _globalPage = page;
    }

    public void ResetGame()
    {
        OnGameResetAction?.Invoke();
    }

    private void Update()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            _isInternetReachable = false;
        }
        else
        {
            _isInternetReachable = true;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void LateInit()
    {
        OnGameLateInitAction?.Invoke();
        OnGameResetAction?.Invoke();
    }

    #region Initiallize
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateGameManagerGameObject()
    {
        GameObject gameManagerObj = new GameObject("GameManager");
        gameManagerObj.AddComponent<GameManager>();
    }
    protected override void Init()
    {
        Cursor.visible = false;
        CreateManagers();
        AddComponents();
    }
    private void CreateManagers()
    {
        CreateManager<ConfigLoadManager>();
        InstantiateManager<ResourceCacheManager>();
        InstantiateManager<StringCacheManager>();
        InstantiateManager<ChromaKeyModule>();

        CreateManager<UserDataManager>();
        CreateManager<PhotoDataManager>();
        CreateManager<LogDataManager>();
        CreateManager<DSLRManager>();
        CreateManager<ProfileModule>();
        CreateManager<StorageManager>();
        CreateManager<MailingModule>();
        CreateManager<AdminManager>();

        switch (ConfigData.config.paymentMethod)
        {
            case 0: // KICC
                CreatePaymentModule<KiccModule>();
                break;
            case 1: // KSNET
                CreatePaymentModule<KsnetModule>();
                break;
        }

        if (ConfigData.config.camType == 0)
        {
            CreateManager<CameraManager>();
        }
    }

    private void InstantiateManager<T>() where T : SingletonBehaviour<T>
    {
        GameObject gameObj = null;
        try
        {
            gameObj = Instantiate(Resources.Load(typeof(T).ToString())) as GameObject;
        }
        catch (Exception e)
        {
            gameObj = new GameObject(typeof(T).ToString());
            gameObj.AddComponent<T>();
            gameObj.transform.parent = inst.transform;
        }
        finally
        {
            gameObj.transform.parent = inst.transform;
        }
    }
    private void CreateManager<T>() where T : SingletonBehaviour<T>
    {
        if (SingletonBehaviour<T>.inst == null)
        {
            SingletonBehaviour<T> manager = new GameObject().AddComponent<T>();
            manager.gameObject.name = manager.GetType().Name;
            manager.transform.parent = inst.transform;
        }
    }

    private void CreatePaymentModule<T>() where T : PaymentModule
    {
        if ((PaymentModule.inst == null))
        {
            PaymentModule manager = new GameObject().AddComponent<T>();
            manager.gameObject.name = manager.GetType().Name;
            manager.transform.parent = inst.transform;
        }
    }

    private void AddComponents()
    {
    }
    #endregion
}