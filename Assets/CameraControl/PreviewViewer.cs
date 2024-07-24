
using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace CameraControl
{
    class PreviewViewer : IObserver
    {
        private CameraModel _model;

        private bool _active;

        private bool m_bDrawZoomFrame;

        private EDSDKLib.EDSDK.EdsRect vRect;

        private EDSDKLib.EDSDK.EdsFocusInfo m_focusInfo;

        private ActionSource _actionSource;
        public void SetActionSource(ref ActionSource actionSource) { _actionSource = actionSource; }

        public PreviewViewer()
        {
            _active = false;
        }

        private delegate void _Update(Observable from, CameraEvent e);

        private bool require = true;
        public void Update(Observable from, CameraEvent e)
        {
            CameraEvent.Type eventType = e.GetEventType();
            _model = (CameraModel)from;
            uint propertyID;
            
            switch (eventType)
            {
                case CameraEvent.Type.EVFDATA_CHANGED:
                    IntPtr evfDataSetPtr = e.GetArg();

                    EVFDataSet evfDataSet = (EVFDataSet)Marshal.PtrToStructure(evfDataSetPtr, typeof(EVFDataSet));

                    this.OnDrawImage(evfDataSet);
                    
                    propertyID = EDSDKLib.EDSDK.PropID_FocusInfo;

                    _actionSource.FireEvent(ActionEvent.Command.GET_PROPERTY, (IntPtr)propertyID);

                    _actionSource.FireEvent(ActionEvent.Command.DOWNLOAD_EVF, IntPtr.Zero);

                    break;

                case CameraEvent.Type.PROPERTY_CHANGED:
                    propertyID = (uint)e.GetArg();
                    

                    if (propertyID == EDSDKLib.EDSDK.PropID_Evf_OutputDevice)
                    {
                        
                        uint device = _model.EvfOutputDevice;

                        // PC live view has started.
                        if (!_active && (device & EDSDKLib.EDSDK.EvfOutputDevice_PC) != 0)
                        {
                            
                            _active = true;
                            // Start download of image data.
                            _actionSource.FireEvent(ActionEvent.Command.DOWNLOAD_EVF, IntPtr.Zero);
                        }

                        // PC live view has ended.
                        if (_active && (device & EDSDKLib.EDSDK.EvfOutputDevice_PC) == 0)
                        {
                            CustomLogger.Log("PC live view has ended.");
                            _active = false;
                        }
                    }
            

                    else if (propertyID == EDSDKLib.EDSDK.PropID_Evf_AFMode)
                    {
                        m_bDrawZoomFrame = _model.EvfAFMode != 2 && _model.isTypeDS;
                    }

                    break;
            }
        }

        private void OnDrawImage(EVFDataSet evfDataSet)
        {
            IntPtr evfStream;
            UInt64 streamLength;

            EDSDKLib.EDSDK.EdsGetPointer(evfDataSet.stream, out evfStream);
            EDSDKLib.EDSDK.EdsGetLength(evfDataSet.stream, out streamLength);
            
            vRect = _model.VisibleRect;
            
            //Debug.Log((int)streamLength);
            //Debug.Log((int)streamLength);
            byte[] data = new byte[(int)streamLength];
            Marshal.Copy(evfStream, data, 0, (int)streamLength);
             
            MainThreadDispatcher.InvokeOnMainThread(data);

        }
    }
}