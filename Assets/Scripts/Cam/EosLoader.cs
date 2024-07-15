using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using EDSDKLib;
using CameraControl;
using Debug = UnityEngine.Debug;

public class EosLoader : MonoBehaviour
{
    #region EDSDK load
    //event handler
    EDSDKLib.EDSDK.EdsPropertyEventHandler handlePropertyEvent = new EDSDKLib.EDSDK.EdsPropertyEventHandler(CameraEventListener.HandlePropertyEvent);
    EDSDKLib.EDSDK.EdsObjectEventHandler handleObjectEvent = new EDSDKLib.EDSDK.EdsObjectEventHandler(CameraEventListener.HandleObjectEvent);
    EDSDKLib.EDSDK.EdsStateEventHandler handleStateEvent = new EDSDKLib.EDSDK.EdsStateEventHandler(CameraEventListener.HandleStateEvent);

    //sdk init data
    private GCHandle handle;
    private IntPtr eosCamera = IntPtr.Zero;
    private CameraModel cameraModel = null;
    private CameraController cameraController = null;
    private List<ActionListener> _actionListenerList = new List<ActionListener>();
    private ActionSource _actionSource = new ActionSource();
    private List<IObserver> _observerList = new List<IObserver>();

    //check sdk
    private bool _isConnected = false;
    //private Coroutine _coroutineCheckCamList;
    private bool isSDKLoaded = false;
    private bool isCamSet = false;

    //
    #endregion

    public bool IsConnected { get { return _isConnected; } }

    public void Initialize()
    {
        if (ConfigData.config.camType == 2)
        {
            StartCoroutine(CheckSDK());
        }
    }

    IEnumerator CheckSDK()
    {
        int checkCount = 0;
        bool isChecked = false;
        while(checkCount < 5)
        {
            if (SettingSDK())
            {
                isChecked = true;
                break;
            }
            checkCount++;
            UnityEngine.Debug.Log("Currently Checking Fail.. : " + checkCount);
            FreeSDK();

            yield return new WaitForSeconds(1);

        }

        if (isChecked)
        {
            UnityEngine.Debug.Log("SDK Checked!");
            AfterLoadSDK();
        }
        else
        {
            UnityEngine.Debug.Log("SDK Check fail");
            GameManager.inst.SetCameraConnected(false);
        }
    }

    bool SettingSDK()
    {
        isSDKLoaded = false;
        // Load SDK
        uint err = EDSDK.EdsInitializeSDK();
        if (err == EDSDK.EDS_ERR_OK)
        {
        }
        else
        {
            //
            UnityEngine.Debug.Log("Failed to load SDK");
            return false;
        }

        // Get camera list
        IntPtr cameraList = IntPtr.Zero;
        err = EDSDK.EdsGetCameraList(out cameraList);
        if (err == EDSDK.EDS_ERR_OK)
        {
            int count = 0;
            err = EDSDK.EdsGetChildCount(cameraList, out count);
            if (count == 0)
            {
                err = EDSDK.EDS_ERR_DEVICE_NOT_FOUND;
                UnityEngine.Debug.Log("device not found");
                return false;
            }

        }
        else
        {
            UnityEngine.Debug.Log("Failed to load camera list");
            return false;
        }
        // Get first camera
        if (err == EDSDK.EDS_ERR_OK)
        {
            err = EDSDK.EdsGetChildAtIndex(cameraList, 0, out eosCamera);
        }

        //release camera list
        if (cameraList != IntPtr.Zero)
        {
            EDSDK.EdsRelease(cameraList);
            cameraList = IntPtr.Zero;
        }
        //create camera model
        if (err == EDSDK.EDS_ERR_OK)
        {
            cameraModel = new CameraModel(eosCamera);
        }

        if (err != EDSDK.EDS_ERR_OK)
        {
            UnityEngine.Debug.Log("Cannot detect camera");
            return false;
        }

        if (err == EDSDK.EDS_ERR_OK)
        {
            cameraController = new CameraController(ref cameraModel);
            handle = GCHandle.Alloc(cameraController);
            IntPtr ptr = GCHandle.ToIntPtr(handle);


            if (err == EDSDK.EDS_ERR_OK)
            {
                err = EDSDK.EdsSetPropertyEventHandler(eosCamera, EDSDK.PropertyEvent_All, handlePropertyEvent, ptr);

            }
            if (err == EDSDK.EDS_ERR_OK)
            {
                err = EDSDK.EdsSetObjectEventHandler(eosCamera, EDSDK.ObjectEvent_All, handleObjectEvent, ptr);

            }
            if (err == EDSDK.EDS_ERR_OK)
            {
                err = EDSDK.EdsSetCameraStateEventHandler(eosCamera, EDSDK.StateEvent_All, handleStateEvent, ptr);

            }
            //if (err == EDSDK.EDS_ERR_OK)
            //{
            //    err = EDSDK.EdsSetCameraStateEventHandler(eosCamera, EDSDK.StateEvent_AfResult, FocusStateEventHandler, ptr);

            //}


            //open session
            //cameraController.Run();
            OpenSession();
            UnityEngine.Debug.Log("Camera connected");
            GameManager.inst.SetCameraConnected(true);
            isSDKLoaded = true;
        }

        return true;
    }

    bool ReconnectSDK()
    {
        isSDKLoaded = false;

        // Get camera list
        IntPtr cameraList = IntPtr.Zero;
        uint err = EDSDK.EdsGetCameraList(out cameraList);
        if (err == EDSDK.EDS_ERR_OK)
        {
            int count = 0;
            err = EDSDK.EdsGetChildCount(cameraList, out count);
            if (count == 0)
            {
                err = EDSDK.EDS_ERR_DEVICE_NOT_FOUND;
                UnityEngine.Debug.Log("device not found");
                return false;
            }

        }
        else
        {
            UnityEngine.Debug.Log("Failed to load camera list");
            return false;
        }
        // Get first camera
        if (err == EDSDK.EDS_ERR_OK)
        {
            err = EDSDK.EdsGetChildAtIndex(cameraList, 0, out eosCamera);
        }

        //release camera list
        if (cameraList != IntPtr.Zero)
        {
            EDSDK.EdsRelease(cameraList);
            cameraList = IntPtr.Zero;
        }
        //create camera model
        if (err == EDSDK.EDS_ERR_OK)
        {
            cameraModel = new CameraModel(eosCamera);
        }

        if (err != EDSDK.EDS_ERR_OK)
        {
            UnityEngine.Debug.Log("Cannot detect camera");
            return false;
        }

        if (err == EDSDK.EDS_ERR_OK)
        {
            cameraController = new CameraController(ref cameraModel);
            handle = GCHandle.Alloc(cameraController);
            IntPtr ptr = GCHandle.ToIntPtr(handle);


            if (err == EDSDK.EDS_ERR_OK)
            {
                err = EDSDK.EdsSetPropertyEventHandler(eosCamera, EDSDK.PropertyEvent_All, handlePropertyEvent, ptr);

            }
            if (err == EDSDK.EDS_ERR_OK)
            {
                err = EDSDK.EdsSetObjectEventHandler(eosCamera, EDSDK.ObjectEvent_All, handleObjectEvent, ptr);

            }
            if (err == EDSDK.EDS_ERR_OK)
            {
                err = EDSDK.EdsSetCameraStateEventHandler(eosCamera, EDSDK.StateEvent_All, handleStateEvent, ptr);

            }

            //open session
            //cameraController.Run();
            OpenSession();
            UnityEngine.Debug.Log("Camera connected");
            GameManager.inst.SetCameraTempConnected(true);
            GameManager.inst.SetCameraConnected(true);
            isSDKLoaded = true;
        }

        return true;
    }

    public void ErrorOnCamera()
    {
        GameManager.inst.SetCameraConnected(false);
    }

    private void OnDestroy()
    {
        DestroyCameraAction();

        if (handle.IsAllocated)
            handle.Free();

        GC.KeepAlive(handlePropertyEvent);
        GC.KeepAlive(handleObjectEvent);
        GC.KeepAlive(handleStateEvent);

        EDSDK.EdsRelease(eosCamera);
        EDSDK.EdsTerminateSDK();
    }

    public void FreeSDK()
    {
        DestroyCameraAction();

        if (handle.IsAllocated)
            handle.Free();

        GC.KeepAlive(handlePropertyEvent);
        GC.KeepAlive(handleObjectEvent);
        GC.KeepAlive(handleStateEvent);

        EDSDK.EdsRelease(eosCamera);
        EDSDK.EdsTerminateSDK();
    }

    void AfterLoadSDK()
    {
        EDSDK.EdsVolumeInfo outVolumeInfo;
        uint err = EDSDK.EdsGetVolumeInfo(eosCamera, out outVolumeInfo);
        if (outVolumeInfo.StorageType != (uint)EDSDK.EdsStorageType.Non)
        {
            UnityEngine.Debug.Log("Camera has storage");
        }
        else
        {
            UnityEngine.Debug.Log("Camera has no storage");
        }

        _actionListenerList.Add((ActionListener)cameraController);
        _actionListenerList.ForEach(actionListener => _actionSource.AddActionListener(ref actionListener));

        InitializeCameraEvent();
        _isConnected = true; 
    }

    #region Camera Event
    public void InitializeCameraEvent()
    {
        PreviewViewer previewViewer = new PreviewViewer();
        ObserverConnection observerConnection = new ObserverConnection();
        CameraEvent cameraEvent;

        //set preview observer
        _observerList = new List<IObserver>
        {
            previewViewer,
            observerConnection
        };
        previewViewer.SetActionSource(ref _actionSource);
        observerConnection.Reconnection = () =>
        {
            Debug.Log("[ ERROR ][reconnection] 임시 카메라 끊김");
            GameManager.inst.SetCameraTempConnected(false);
            //GameManager.inst.SetCameraConnected(false);

            //terminateSDK에서 crash 발생
            //FreeSDK();
            //Initialize();
            FreeSDKNotTerminate();
            StartCoroutine(CheckSDKNotTerminate());
        };
        _observerList.ForEach(observer => cameraController.GetModel().Add(ref observer));

        cameraEvent = new CameraEvent(CameraEvent.Type.PROPERTY_CHANGED, (IntPtr)EDSDKLib.EDSDK.PropID_BatteryLevel);
        cameraController.GetModel().NotifyObservers(cameraEvent);

        if (!cameraController.GetModel().isTypeDS)
        {
            _actionSource.FireEvent(ActionEvent.Command.REMOTESHOOTING_START, IntPtr.Zero);
        }

        //preview activate
        MainThreadDispatcher mainThreadDispatcher = gameObject.AddComponent<MainThreadDispatcher>();
        mainThreadDispatcher.Setting();
    }

    void FreeSDKNotTerminate()
    {
        DestroyCameraAction();

        if (handle.IsAllocated)
            handle.Free();

        GC.KeepAlive(handlePropertyEvent);
        GC.KeepAlive(handleObjectEvent);
        GC.KeepAlive(handleStateEvent);

        EDSDK.EdsRelease(eosCamera);
    }

    IEnumerator CheckSDKNotTerminate()
    {
        int checkCount = 0;
        bool isChecked = false;
        while (checkCount < 5)
        {
            if (ReconnectSDK())
            {
                isChecked = true;
                break;
            }
            checkCount++;
            UnityEngine.Debug.Log("[ ERROR ][reconnection] Currently Checking Fail.. : " + checkCount);
            FreeSDKNotTerminate();

            yield return new WaitForSeconds(3);
        }


        if (isChecked)
        {
            UnityEngine.Debug.Log("[ ERROR FIX ][reconnection] SDK Checked!");
            AfterLoadSDK();
            GameManager.inst.SetCameraTempConnected(true);
        }
        else
        {
            UnityEngine.Debug.Log("[ ERROR ][reconnection] SDK Check fail");
            GameManager.inst.SetCameraConnected(false);
        }
    }

    public void CameraSetting()
    {
        if (isCamSet)
            return;

        SetCameraWB();
        SetCameraISO();
        SetCameraAV();
        SetCameraTV();
        isCamSet = true;
    }

    public void OpenSession()
    {
        Debug.Log(DateTime.Now.ToString("HH:mm:ss.fff") + " CALL OPEN SESSION");
        cameraController.Run();
        //cameraController.OpenSession();
        //Debug.Log("EOS OPEN SESSION");
        Debug.Log(DateTime.Now.ToString("HH:mm:ss.fff") + " OPEN SESSION");

        CameraAutoFocusON();
    }

    public void CloseSession()
    {
        _actionSource.FireEvent(ActionEvent.Command.CLOSING, IntPtr.Zero);
        Debug.Log("EOS CLOSE SESSION");
    }

    public void CameraEVFOn()
    {
        //_actionSource.FireEvent(ActionEvent.Command.EVF_AF_ON, IntPtr.Zero);
        _actionSource.FireEvent(ActionEvent.Command.START_EVF, IntPtr.Zero);
        //_actionSource.FireEvent(ActionEvent.Command.EVF_AF_ON, IntPtr.Zero);
        Debug.Log("EOS START EVF");
    }

    public void CameraEVFOff()
    {
        _actionSource.FireEvent(ActionEvent.Command.END_EVF, IntPtr.Zero);
        //_actionSource.FireEvent(ActionEvent.Command.EVF_AF_OFF, IntPtr.Zero);
        Debug.Log("EOS STOP EVF");
    }
    /**/

    public void CameraAutoFocusON()
    {
        uint key = (uint)0;
        //ActionEvent e = new ActionEvent(ActionEvent.Command.SET_AF_MODE, (IntPtr)key);
        //cameraController.ActionPerformed(e);

        //UnityEngine.Debug.Log("Set to Auto focus : ");

        uint err = EDSDK.EdsSetPropertyData(eosCamera, EDSDK.PropID_AFMode, 0, sizeof(uint), key);
        Debug.Log(DateTime.Now.ToString("HH:mm:ss.fff") + " Try To Change Focus to Auto");

        if (err != EDSDK.EDS_ERR_OK)
        {
            Debug.Log("EOS SET FOCUS ERROR");
        }
        else
        {
            CheckCameraFocusMode();
            Debug.Log(DateTime.Now.ToString("HH:mm:ss.fff") + " Successfuly Changed Focus to Auto");
        }
    }

    public void CameraManualON()
    {
        uint key = (uint)3;
        //ActionEvent e = new ActionEvent(ActionEvent.Command.SET_AF_MODE, (IntPtr)key);
        //cameraController.ActionPerformed(e);

        //UnityEngine.Debug.Log("Set to Manual focus : ");
        //CheckCameraFocusMode();

        uint err = EDSDK.EdsSetPropertyData(eosCamera, EDSDK.PropID_AFMode, 0, sizeof(uint), key);
        Debug.Log(DateTime.Now.ToString("HH:mm:ss.fff") + " Try To Change Focus to Manual");

        if (err != EDSDK.EDS_ERR_OK)
        {
            Debug.Log("EOS SET FOCUS ERROR");
        }
        else
        {
            CheckCameraFocusMode();
            Debug.Log(DateTime.Now.ToString("HH:mm:ss.fff") + " Successfuly Changed Focus to Manual");
        }
    }

    public void CheckCameraFocusMode()
    {
        uint data;
        uint err = EDSDK.EdsGetPropertyData(eosCamera, EDSDK.PropID_AFMode, 0, out data);

        if(err == EDSDKLib.EDSDK.EDS_ERR_OK)
        {
            Debug.Log("FOCUS MODE : " + data);
        }
        else
        {
            Debug.Log("Error Get FOCUS MODE");
        }
    }

    public void CameraShoot()
    {
        _actionSource.FireEvent(ActionEvent.Command.TAKE_PICTURE, IntPtr.Zero);
    }

    // 라이브 뷰 모드에서 자동 초점 적용
    public void CameraEVFAutoFocusOn()
    {
        EDSDK.EdsSendCommand(eosCamera, EDSDK.CameraCommand_DoEvfAf, 1);
    }

    public void CameraEVFAutoFocusOff()
    {
        EDSDK.EdsSendCommand(eosCamera, EDSDK.CameraCommand_DoEvfAf, 0);
    }

    // 강제 촬영
    public void CameraForceShoot(Action OnEndShoot = null)
    {
        StartCoroutine(ShutterRoutine(OnEndShoot));
    }

    public void CameraFocus()
    {
        StartCoroutine(FocusRoutine());
    }

    public void CameraFocusNonAF()
    {
        StartCoroutine(ShutterNonAFRoutine());
    }

    IEnumerator ShutterRoutine(Action OnEndShoot = null)
    {
        Debug.Log(StringCacheManager.inst.PointLine + DateTime.Now.ToString("HH:mm:ss.fff") + " Start of Shooting");
        CameraAutoFocusON();
        uint err = EDSDK.EdsSendCommand(eosCamera, EDSDK.CameraCommand_PressShutterButton, (int)EDSDK.EdsShutterButton.CameraCommand_ShutterButton_Halfway);

        if(err == EDSDKLib.EDSDK.EDS_ERR_TAKE_PICTURE_AF_NG)
        {
            Debug.Log("FOCUS FAIL!");
            CameraManualON();

            yield return new WaitForSecondsRealtime(0.2f);
            uint shootErr = EDSDK.EdsSendCommand(eosCamera, EDSDK.CameraCommand_PressShutterButton, (int)EDSDK.EdsShutterButton.CameraCommand_ShutterButton_Completely_NonAF);
            if(shootErr != EDSDK.EDS_ERR_OK)
            {
                Debug.Log("Manual Shooting Error : " +  shootErr);
            }
            yield return new WaitForSecondsRealtime(0.2f);
            EDSDK.EdsSendCommand(eosCamera, EDSDK.CameraCommand_PressShutterButton, (int)EDSDK.EdsShutterButton.CameraCommand_ShutterButton_OFF);
            Debug.Log(StringCacheManager.inst.PointLine + DateTime.Now.ToString("HH:mm:ss.fff") + " End of Shooting");
        }
        else
        {
            Debug.Log("FOCUS SUCCESS");

            yield return new WaitForSecondsRealtime(0.2f);
            EDSDK.EdsSendCommand(eosCamera, EDSDK.CameraCommand_PressShutterButton, (int)EDSDK.EdsShutterButton.CameraCommand_ShutterButton_Completely);
            yield return new WaitForSecondsRealtime(0.2f);
            EDSDK.EdsSendCommand(eosCamera, EDSDK.CameraCommand_PressShutterButton, (int)EDSDK.EdsShutterButton.CameraCommand_ShutterButton_OFF);
            Debug.Log(StringCacheManager.inst.PointLine + DateTime.Now.ToString("HH:mm:ss.fff") + " End of Shooting");
        }

        OnEndShoot?.Invoke(); 
    }

    IEnumerator ShutterNonAFRoutine()
    {
        EDSDK.EdsSendCommand(eosCamera, EDSDK.CameraCommand_PressShutterButton, (int)EDSDK.EdsShutterButton.CameraCommand_ShutterButton_Halfway_NonAF);
        yield return new WaitForSecondsRealtime(0.2f);
        uint err = EDSDK.EdsSendCommand(eosCamera, EDSDK.CameraCommand_PressShutterButton, (int)EDSDK.EdsShutterButton.CameraCommand_ShutterButton_Completely_NonAF);
        yield return new WaitForSecondsRealtime(0.2f);
        EDSDK.EdsSendCommand(eosCamera, EDSDK.CameraCommand_PressShutterButton, (int)EDSDK.EdsShutterButton.CameraCommand_ShutterButton_OFF);
    }

    IEnumerator FocusRoutine()
    {
        EDSDK.EdsSendCommand(eosCamera, EDSDK.CameraCommand_PressShutterButton, (int)EDSDK.EdsShutterButton.CameraCommand_ShutterButton_Halfway);
        yield return new WaitForSecondsRealtime(0.5f);
        EDSDK.EdsSendCommand(eosCamera, EDSDK.CameraCommand_PressShutterButton, (int)EDSDK.EdsShutterButton.CameraCommand_ShutterButton_OFF);
    }

    //public void CameraForceShoot()
    //{
    //    EDSDK.EdsSendCommand(eosCamera, EDSDK.CameraCommand_PressShutterButton, (int)EDSDK.EdsShutterButton.CameraCommand_ShutterButton_Completely);
    //}
    //----------------------------------카메라 세팅----------------------------------------//

    //화이트밸런스
    public void SetCameraWB()
    {
        var model = cameraController.GetModel();
        EDSDK.EdsPropertyDesc _desk = cameraController.GetModel().WhiteBalanceDesc;
        uint key = (uint)_desk.PropDesc[ConfigData.config.wbIdx];
        ActionEvent e = new ActionEvent(ActionEvent.Command.SET_WHITE_BALANCE, (IntPtr)key);
        uint data = (uint)e.GetArg();

        cameraController.ActionPerformed(e);

        UnityEngine.Debug.Log("Set WHITE BALANCE : ");
    }

    //ISO
    public void SetCameraISO()
    {
        EDSDK.EdsPropertyDesc _desk = cameraController.GetModel().IsoDesc;
        uint key = (uint)_desk.PropDesc[ConfigData.config.isoIdx];
        ActionEvent e = new ActionEvent(ActionEvent.Command.SET_ISO_SPEED, (IntPtr)key);
        uint data = (uint)e.GetArg();

        cameraController.ActionPerformed(e);

        UnityEngine.Debug.Log("Set iso speed ");
    }

    //조리개
    public void SetCameraAV()
    {
        EDSDK.EdsPropertyDesc _desk = cameraController.GetModel().AvDesc;
        uint key = (uint)_desk.PropDesc[ConfigData.config.avIdx];
        ActionEvent e = new ActionEvent(ActionEvent.Command.SET_AV, (IntPtr)key);
        uint data = (uint)e.GetArg();

        cameraController.ActionPerformed(e);

        UnityEngine.Debug.Log("Set iso speed ");
    }

    //셔터스피드
    public void SetCameraTV()
    {
        EDSDK.EdsPropertyDesc _desk = cameraController.GetModel().TvDesc;
        uint key = (uint)_desk.PropDesc[ConfigData.config.tvIdx];
        ActionEvent e = new ActionEvent(ActionEvent.Command.SET_TV, (IntPtr)key);
        uint data = (uint)e.GetArg();

        cameraController.ActionPerformed(e);

        UnityEngine.Debug.Log("Set shutter speed ");
    }

    void DestroyCameraAction()
    {
        if (!isSDKLoaded)
            return;
        _actionSource.FireEvent(ActionEvent.Command.END_ROLLPITCH, IntPtr.Zero);
        _actionSource.FireEvent(ActionEvent.Command.END_EVF, IntPtr.Zero);
        if (!cameraController.GetModel().isTypeDS)
        {
            _actionSource.FireEvent(ActionEvent.Command.REMOTESHOOTING_STOP, IntPtr.Zero);
        }
        _actionSource.FireEvent(ActionEvent.Command.PRESS_OFF, IntPtr.Zero);
        _actionSource.FireEvent(ActionEvent.Command.EVF_AF_OFF, IntPtr.Zero);
        _observerList.ForEach(observer => cameraController.GetModel().Remove(ref observer));
    }
    #endregion
}
