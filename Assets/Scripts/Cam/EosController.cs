using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using CameraControl;
using EDSDKLib;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class EosController : MonoBehaviour
{
    
    public CameraController _controller ;

    public ActionSource _actionSource ;

    private List<IObserver> _observerList = new List<IObserver>();

    private BatteryLevelLabel batterylevel;
    private PreviewViewer previewViewer;

    public void Initialize(ref CameraController controller, ref ActionSource actionSource)
    {
        batterylevel = new BatteryLevelLabel();
        previewViewer = new PreviewViewer();
        this._controller = controller;
        this._actionSource = actionSource;
        CameraEvent e;
        
        _observerList.Add((IObserver)batterylevel);
        _observerList.Add((IObserver)previewViewer);
        
        previewViewer.SetActionSource(ref _actionSource);

        _observerList.ForEach(observer => _controller.GetModel().Add(ref observer));
        e = new CameraEvent(CameraEvent.Type.PROPERTY_CHANGED, (IntPtr)EDSDKLib.EDSDK.PropID_BatteryLevel);
        _controller.GetModel().NotifyObservers(e);

        if (!_controller.GetModel().isTypeDS)
        {
            _actionSource.FireEvent(ActionEvent.Command.REMOTESHOOTING_START, IntPtr.Zero);
        }

        MainThreadDispatcher mainThreadDispatcher = GetComponent<MainThreadDispatcher>();
        mainThreadDispatcher.Setting();
    }

    public void CameraSetting()
    {
        SetCameraWB();
        SetCameraISO();
        SetCameraAV();
        SetCameraTV();
    }

    public void CameraOn()
    {
        _actionSource.FireEvent(ActionEvent.Command.START_EVF, IntPtr.Zero);
        //_actionSource.FireEvent(ActionEvent.Command.EVF_AF_ON, IntPtr.Zero);
        //Debug.Log("Camera on");
    }

    public void CameraOff()
    {
        _actionSource.FireEvent(ActionEvent.Command.END_EVF, IntPtr.Zero);
        //Debug.Log("Camera off");
    }

    public void CameraAFON()
    {
        _actionSource.FireEvent(ActionEvent.Command.EVF_AF_ON, IntPtr.Zero);
        //_actionSource.FireEvent(ActionEvent.Command.REMOTESHOOTING_START, IntPtr.Zero);
    }

    public void CameraShoot()
    {
        _actionSource.FireEvent(ActionEvent.Command.TAKE_PICTURE, IntPtr.Zero);
        //_actionSource.FireEvent(ActionEvent.Command.REMOTESHOOTING_START, IntPtr.Zero);
    }

    //----------------------------------카메라 세팅----------------------------------------//

    public void SetCameraWB()
    {
        var model = _controller.GetModel();
        EDSDK.EdsPropertyDesc _desk = _controller.GetModel().WhiteBalanceDesc;
        uint key = (uint)_desk.PropDesc[ConfigData.config.wbIdx];
        ActionEvent e = new ActionEvent(ActionEvent.Command.SET_WHITE_BALANCE, (IntPtr)key);
        uint data  = (uint)e.GetArg();

        _controller.ActionPerformed(e);

        Debug.Log("Set WHITE BALANCE : " );
    }

    public void SetCameraISO()
    {
        EDSDK.EdsPropertyDesc _desk = _controller.GetModel().IsoDesc;
        uint key = (uint)_desk.PropDesc[ConfigData.config.isoIdx];
        ActionEvent e = new ActionEvent(ActionEvent.Command.SET_ISO_SPEED, (IntPtr)key);
        uint data = (uint)e.GetArg();

        _controller.ActionPerformed(e);

        Debug.Log("Set iso speed ");
    }

    //조리개
    public void SetCameraAV()
    {
        EDSDK.EdsPropertyDesc _desk = _controller.GetModel().AvDesc;
        uint key = (uint)_desk.PropDesc[ConfigData.config.avIdx];
        ActionEvent e = new ActionEvent(ActionEvent.Command.SET_AV, (IntPtr)key);
        uint data = (uint)e.GetArg();

        _controller.ActionPerformed(e);

        Debug.Log("Set iso speed ");
    }

    //셔터스피드
    public void SetCameraTV()
    {
        EDSDK.EdsPropertyDesc _desk = _controller.GetModel().TvDesc;
        uint key = (uint)_desk.PropDesc[ConfigData.config.tvIdx];
        ActionEvent e = new ActionEvent(ActionEvent.Command.SET_TV, (IntPtr)key);
        uint data = (uint)e.GetArg();

        _controller.ActionPerformed(e);

        Debug.Log("Set shutter speed ");
    }




    private void OnDestroy()
    {
        _actionSource.FireEvent(ActionEvent.Command.END_ROLLPITCH, IntPtr.Zero);
        _actionSource.FireEvent(ActionEvent.Command.END_EVF, IntPtr.Zero);
        if (!_controller.GetModel().isTypeDS)
        {
            _actionSource.FireEvent(ActionEvent.Command.REMOTESHOOTING_STOP, IntPtr.Zero);
        }
        _actionSource.FireEvent(ActionEvent.Command.PRESS_OFF, IntPtr.Zero);
        _actionSource.FireEvent(ActionEvent.Command.EVF_AF_OFF, IntPtr.Zero);
        _observerList.ForEach(observer => _controller.GetModel().Remove(ref observer));
    }
}
