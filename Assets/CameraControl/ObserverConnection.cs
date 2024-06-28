using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace CameraControl
{
    class ObserverConnection : IObserver
    {
        //private CameraModel _model;
        public Action Reconnection;

        public void Update(Observable from, CameraEvent e)
        {
            CameraEvent.Type eventType = e.GetEventType();
            //_model = (CameraModel)from;
            uint propertyID;

            switch (eventType)
            {
                case CameraEvent.Type.SHUT_DOWN:

                    Debug.Log("[IObserver] Camera is disconnected");
                    //GameManager.inst.SetCameraConnected(false);
                    Reconnection();
                    break;
            }
        }
    }
}
